import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { loadScripts } from '../../shared/functions';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent, TopMenuComponent } from './components';

@Component({
    selector: 'app-main-layout',
    templateUrl: './main-layout.component.html',
    styles: `
	.brand {
		width: 320px;
	}
	.brand-logo {
		padding: 3px !important;
	}
	`,
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
        SidebarComponent,
        TopMenuComponent,
        RouterOutlet,
    ]
})
export class MainLayoutComponent implements OnInit {
	is_loaded = false;
	constructor() { }

	ngOnInit(): void {
		this.init();
	}
	async init() {
		await loadScripts([
			'assets/js/app.min.js',
		]);
		this.is_loaded = true;
	}

}
