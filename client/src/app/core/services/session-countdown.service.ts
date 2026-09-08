import { computed, Injectable, signal } from '@angular/core';
import { IsNoE } from '../../shared/functions';
import { AppStorageService, AuthService } from '../services';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SessionCountdownService {
	private inactivityMin = signal(0);
	private maxInactivityMin = Number(environment.sessio_time_in_min) ?? 0;
	private intervalId: number | null = null;

	constructor(private _auth: AuthService) {}

	setTimer() {
		const token = AppStorageService.getAccessToken(); // ← storage, no llaveClient
		if (!token || this.maxInactivityMin <= 0) return;

		this.stopTimer();
		AppStorageService.lastActivity(Date.now());
		this.inactivityMin.set(0);
		this.intervalId = window.setInterval(() => this.loop(), 60_000);
	}

	stopTimer() {
		if (this.intervalId !== null) {
			window.clearInterval(this.intervalId);
			this.intervalId = null;
		}
	}

	sessionActive = computed(() => {
		if (this.maxInactivityMin <= 0) return true;
		this.inactivityMin(); // registra dependencia para re-evaluar en cada tick
		const lastActivity = AppStorageService.lastActivity();
		if (!lastActivity) return false;
		const expiration = lastActivity + this.maxInactivityMin * 60_000;
		return Date.now() <= expiration;
	});

	private loop() {
		this.inactivityMin.update((v) => v + 1);
		if (this.inactivityMin() >= this.maxInactivityMin) {
			this._auth.logOut();
			this.stopTimer();
		}
	}
}
