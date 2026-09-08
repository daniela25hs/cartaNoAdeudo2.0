import { Injectable } from '@angular/core';
import { AppStorageService } from '../../core/services';
import { BaseStateService } from './base.state';

type UserStateType = {
	usuarioId: string;
	nombre: string;
	email: string;
	roles: string[];
};

@Injectable({
	providedIn: 'root',
})
export class UserState extends BaseStateService<UserStateType> {
	constructor() {
		super({ usuarioId: '', nombre: '', email: '', roles: [] });
		const usuarioId = AppStorageService.getUsuarioId();
		const email = AppStorageService.getEmail();
		const nombre = AppStorageService.getNombre();
		const roles = AppStorageService.getRoles();

		if (email) {
			this.setState({ usuarioId: usuarioId ?? '', email, nombre: nombre ?? '', roles });
		}
	}

	setFromToken(data: { usuarioId: string; nombre: string; email: string; roles: string[] }) {
		this.setState(data);
	}

	// Id del usuario (claim "sub" del JWT) — identificador técnico autoritativo,
	// no confundir con email/nombre, que son datos de perfil mutables.
	get usuarioId() {
		return this.select('usuarioId');
	}
	get nombre() {
		return this.select('nombre');
	}
	get email() {
		return this.select('email');
	}
	get roles() {
		return this.select('roles');
	}
}
