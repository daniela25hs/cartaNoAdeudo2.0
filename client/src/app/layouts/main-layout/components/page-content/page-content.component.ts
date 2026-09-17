import { Component, Input, OnInit, input, ChangeDetectionStrategy } from '@angular/core';
import { BreadcrumbComponent } from '../breadcrumb/breadcrumb.component';
import { AppState } from '../../../../shared/states/app.state';

@Component({
    selector: 'app-page-content',
    templateUrl: './page-content.component.html',
    styleUrls: ['./page-content.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [BreadcrumbComponent]
})
export class PageContentComponent implements OnInit {
	pageTitle = input('');
	subtitle = input('');
	constructor(private $app: AppState) { }

	ngOnInit(): void {
		this.$app.set('pageTitle', this.pageTitle())
	}
}
