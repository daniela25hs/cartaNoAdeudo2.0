import { Permissions } from '../data';

export interface SideBarModel {
	groups: SideBarGroup[];
}

export interface SideBarGroup {
	text: string;
	items: SideBarItem[];
}

export interface SideBarItem {
	text: string;
	icon?: string;
	route?: string;
	items?: SideBarItem[];
	onClick?: string;
	permissions?: Permissions[];
}
