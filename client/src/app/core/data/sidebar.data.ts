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
				{
					text: 'Solicitar carta',
					icon: 'mdi mdi-file-document-plus-outline',
					route: '/app/cartas',
				},
				{
					text: 'Firmantes',
					icon: 'mdi mdi-account-key-outline',
					route: '/app/firmantes',
				},
				{
					text: 'Layouts',
					icon: 'mdi mdi-file-document-edit-outline',
					route: '/app/layouts',
				},
				{
					text: 'Tareas',
					icon: 'mdi mdi-refresh',
					route: '/app/tareas',
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
