import { inject } from '@angular/core';
import {
	ActivatedRouteSnapshot,
	CanActivateChildFn,
	CanActivateFn,
	Router,
	RouterStateSnapshot,
} from '@angular/router';
import { matchArrays } from '../../shared/functions';
import { AppStorageService } from '../services';
import { UserState } from '../../shared/states/user.state';

export const rolePermissionsChildGuard: CanActivateChildFn = async (
	childRoute: ActivatedRouteSnapshot,
	state: RouterStateSnapshot,
) => {
	return checkRolesPermissions(
		childRoute.data.permissions,
		childRoute.data.roles,
	);
};

export const rolePermissionsGuard: CanActivateFn = async (
	route: ActivatedRouteSnapshot,
	state: RouterStateSnapshot,
) => {
	return checkRolesPermissions(route.data.permissions, route.data.roles);
};

function checkRolesPermissions(
	permissions: string[],
	roles: string[],
): boolean {
	if (!roles && !permissions) {
		return true;
	}
	const $user = inject(UserState);
	let valid = false;
	if (roles) {
		const user_roles = $user.roles() || [];
		const match = matchArrays(
			roles,
			user_roles.map((i) => i),
		);
		valid = match.length > 0;
	}
	// if (permissions && !valid) {
	// 	const user_permissions = $user.permissions() || [];
	// 	const match = matchArrays(permissions, user_permissions.map((i) => i.key));
	// 	valid = match.length > 0;
	// }
	if (!valid) {
		const router = inject(Router);
		router.navigate(['/app/error', 401], {
			queryParams: {
				message: 'No tiene permitido acceder a la pagina',
			},
		});
	}
	return valid;
}
