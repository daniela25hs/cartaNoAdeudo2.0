import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter } from 'rxjs';
import { sideBarData } from '../../../../core/data';
import { SideBarItem } from '../../../../core/models';

@Component({
    selector: 'app-breadcrumb',
    templateUrl: './breadcrumb.component.html',
    styleUrls: ['./breadcrumb.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [RouterLink]
})
export class BreadcrumbComponent implements OnInit {
    listRoutes: SideBarItem[] = [];

    constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

    ngOnInit() {
        this.update();
        this.router.events
            .pipe(filter(e => e instanceof NavigationEnd))
            .subscribe(() => this.update());
    }

    private update() {
        const url = this.router.url.split('?')[0];
        const found = this.find(sideBarData.groups, url);

        const last = found[found.length - 1];
        if (last?.route !== url) {
            const label = this.getLeafBreadcrumb();
            if (label) {
                found.push({ text: label });
            }
        }

        this.listRoutes = found;
    }

    private find(items: any[], url: string): SideBarItem[] {
        let bestResult: SideBarItem[] = [];
        let bestLen = -1;

        for (const obj of items) {
            if (obj.route) {
                if ((obj.route === url || url.startsWith(obj.route + '/')) && obj.route.length > bestLen) {
                    bestLen = obj.route.length;
                    bestResult = [obj];
                }
            } else if (obj.items?.length) {
                const sub = this.find(obj.items, url);
                if (sub.length > 0) {
                    const leaf = sub[sub.length - 1];
                    const leafLen = leaf.route?.length ?? 0;
                    if (leafLen > bestLen) {
                        bestLen = leafLen;
                        bestResult = obj.text ? [{ text: obj.text }, ...sub] : sub;
                    }
                }
            }
        }

        return bestResult;
    }

    private getLeafBreadcrumb(): string | null {
        let route = this.activatedRoute;
        while (route.firstChild) {
            route = route.firstChild;
        }
        return route.snapshot.data?.['breadcrumb'] ?? null;
    }
}
