import { Injectable, Signal, computed, signal } from '@angular/core';
import { BaseStateService } from './base.state';

type AppStateType = {
	pageTitle: string
}
@Injectable({
	providedIn: 'root'
})
export class AppState extends BaseStateService<AppStateType> {

	isLoading = signal(true)

	get pageTitle() { return this.select('pageTitle') }

	constructor() {
		super({
			pageTitle: ''
		});
	}

	showLoading() {
		this.isLoading.set(true)
	}
	hideLoading() {
		this.isLoading.set(false)
	}
}

