import { SideBarModel } from '../models';

// Menú lateral. Base mínima: Dashboard + Cerrar Sesión.
// Agrega aquí las entradas de tus módulos de negocio a medida que los construyas.
export const sideBarData: SideBarModel = {
	groups: [
		{
			text: 'General',
			items: [
				{
					text: 'Dashboard',
					icon: 'mdi mdi-view-dashboard-outline',
					route: '/app',
				},
			],
		},
		{
			text: 'Cerrar Sesión',
			items: [
				{
					text: 'Salir del Sistema',
					icon: 'fa fa-sign-out-alt',
					onClick: 'logout',
				},
			],
		},
	],
};
