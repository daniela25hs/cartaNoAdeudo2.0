import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService, SessionCountdownService } from '../services';

export const authGuard: CanActivateFn = () => {
	const _auth = inject(AuthService);
	const _scd = inject(SessionCountdownService);

	if (!_auth.isAuthenticated()) {
		_auth.logOut();
		return false;
	}

	if (!_scd.sessionActive()) {
		_auth.logOut();
		return false;
	}

	return true;
};
