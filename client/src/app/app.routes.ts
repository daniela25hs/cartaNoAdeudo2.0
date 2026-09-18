import { Routes } from '@angular/router';
import {
	authGuard,
	isAuthenticatedGuard,
	rolePermissionsChildGuard,
} from './core/guards';
import { PublicLayoutComponent } from './layouts/public-layout/public-layout.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { ErrorComponent } from './pages/error/error.component';

// Estructura base: layout público (login + formularios de cuenta) y layout privado
// (dashboard + perfil, protegido por authGuard). Agrega los módulos de negocio como
// hijos de la ruta 'app'.
export const routes: Routes = [
	{
		path: '',
		component: PublicLayoutComponent,
		children: [
			{
				path: '',
				redirectTo: 'login',
				pathMatch: 'full',
			},
			{
				path: 'login',
				canActivate: [isAuthenticatedGuard],
				loadComponent: () =>
					import('./pages/login/login.component').then(
						(c) => c.LoginComponent,
					),
			},
			{
				// Formulario de activación / cambio de contraseña (patrón reutilizable).
				// El envío está deshabilitado hasta que la API exponga el endpoint
				// correspondiente — ver activacion-usuario.component.ts.
				path: 'usuarios/activacion-usuario',
				loadComponent: () =>
					import('./pages/activacion-usuario/activacion-usuario.component').then(
						(c) => c.ActivacionUsuarioComponent,
					),
				data: { mode: 'activar' },
			},
			{
				path: 'usuarios/cambiar-password',
				loadComponent: () =>
					import('./pages/activacion-usuario/activacion-usuario.component').then(
						(c) => c.ActivacionUsuarioComponent,
					),
				data: { mode: 'cambiar-password' },
			},
		],
	},
	{
		path: 'app',
		component: MainLayoutComponent,
		// TODO: reactivar cuando el login vuelva a validar credenciales.
		// canActivate: [authGuard],
		canActivateChild: [rolePermissionsChildGuard],
		children: [
			{
				path: '',
				loadComponent: () =>
					import('./pages/dashboard/dashboard.component').then(
						(c) => c.DashboardComponent,
					),
			},
			{
				path: 'perfil',
				loadComponent: () =>
					import('./pages/profile/profile.component').then(
						(c) => c.ProfileComponent,
					),
			},
			// Aquí se agregan los módulos de negocio a medida que se construyan.
			// Ver docs/client/GUIA_DESARROLLO_CLIENT.md.
		],
	},
	{
		path: 'error/:code',
		component: ErrorComponent,
	},
	{
		path: '**',
		redirectTo: 'app',
		pathMatch: 'full',
	},
];
