import { TokenResponse } from '../dtos/index';

export class AppStorageService {
	static setTokens(data: TokenResponse): void {
		AppStorageService.set('accessToken', data.accessToken);
		AppStorageService.set('refreshToken', data.refreshToken);
		AppStorageService.set('usuarioId', data.usuarioId);
		AppStorageService.set('email', data.email);
		AppStorageService.set('nombre', data.nombre);
		AppStorageService.set('roles', JSON.stringify(data.roles));
		AppStorageService.lastActivity(Date.now());
	}

	static getAccessToken(): string | null {
		return localStorage.getItem('accessToken');
	}
	static getRefreshToken(): string | null {
		return localStorage.getItem('refreshToken');
	}
	// Id del usuario (claim "sub" del JWT) — identificador técnico autoritativo,
	// distinto de email/nombre que son datos de perfil mutables.
	static getUsuarioId(): string | null {
		return localStorage.getItem('usuarioId');
	}
	static getEmail(): string | null {
		return localStorage.getItem('email');
	}
	static getNombre(): string | null {
		return localStorage.getItem('nombre');
	}

	static getRoles(): string[] {
		try {
			return JSON.parse(localStorage.getItem('roles') || '[]');
		} catch {
			return [];
		}
	}

	static lastActivity(v?: number): number | undefined {
		return AppStorageService.getSetItem('lastActivity', v) || undefined;
	}

	static getSetItem(k: string, v: any): any {
		if (v !== undefined && v !== null) AppStorageService.set(k, v);
		return AppStorageService.get(k);
	}

	static set(key: string, value: any) {
		if (value !== null && typeof value === 'object')
			value = JSON.stringify(value);
		localStorage.setItem(key, value);
	}

	static get(key: string) {
		let valor = localStorage.getItem(key);
		try {
			valor = JSON.parse(valor!);
		} catch (e) {}
		return valor;
	}

	static clearAll() {
		localStorage.clear();
	}
	static removeItem(key: string) {
		localStorage.removeItem(key);
	}
}
