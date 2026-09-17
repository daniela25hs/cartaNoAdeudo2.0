import { AfterViewInit, Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { sideBarData } from '../../../../core/data';
import { SideBarItem } from '../../../../core/models';
import { AuthService } from '../../../../core/services';
import { RemoveHostDirective } from '../../../../shared/directives';
import { PageTextsPipe } from '../../../../shared/pipes';
import { SidebarItemComponent } from '../../components';

declare const $: any;
@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [SidebarItemComponent, RemoveHostDirective, PageTextsPipe]
})
export class SidebarComponent implements OnInit, AfterViewInit {
	items = sideBarData;
	constructor(private _auth: AuthService) { }
	ngAfterViewInit(): void {
		$('.menu-header:not(:has(+ .menu-item))').remove() // esto es por que firefox no aplica el estilo correctamente que hace que oculte el menu
	}

	ngOnInit(): void {
		setTimeout(() => {
			$('.menu-item.has-sub:has(.active) .menu-link').click()
			$('.app-sidebar .menu > .menu-item > .menu-link').on('click', function(this: any) {
				var otherMenu = $('.app-sidebar .menu > .menu-item.has-sub.expand > .menu-link').not($(this));
				$(otherMenu).click();
			})
		}, 1000);
	}

	onClickItem(item: SideBarItem) {
		const action = item.onClick;
		switch (action) {
			case 'logout':
				this.logout();
				break;
			default:
				break;
		}
	}
	logout() {
		this._auth.logOut();
	}
}
