import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services';

export const isAuthenticatedGuard = () => {
	const _auth = inject(AuthService);
	const _router = inject(Router);

	if (_auth.isAuthenticated()) {
		_router.navigate(['/app']);
		return false;
	}
	return true;
};
