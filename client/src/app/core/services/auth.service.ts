import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { AppState } from '../../shared/states/app.state';
import { ApiResponse, LoginRequest, TokenResponse } from '../dtos';
import { AppStorageService, HttpService } from '../services';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

export const url_refresh_token: string = `${environment.api_url}/api/auth/refresh`;

@Injectable({
	providedIn: 'root',
})
export class AuthService {
	url = environment.api_url;
	constructor(
		private _http: HttpService,
		private router: Router,
		private $app: AppState,
		private http: HttpClient,
	) {}

	async login(
		email: string,
		password: string,
	): Promise<ApiResponse<TokenResponse>> {
		const body: LoginRequest = { email, password };
		return await firstValueFrom(
			this.http.post<ApiResponse<TokenResponse>>(
				`${this.url}/api/auth/login`,
				body,
			),
		);
	}

	async refreshToken(): Promise<boolean> {
		const refreshToken = AppStorageService.getRefreshToken();
		if (!refreshToken) return false;
		try {
			const res = await this._http.request<ApiResponse<TokenResponse>>(
				'POST',
				url_refresh_token,
				{ body: { refreshToken }, skipInterceptor: true },
			);
			if (res.success && res.data) {
				AppStorageService.setTokens(res.data);
				return true;
			}
			return false;
		} catch {
			return false;
		}
	}

	async logOut(): Promise<void> {
		this.$app.showLoading();
		// El backend de este esqueleto solo expone login/refresh — no hay endpoint
		// de logout. El cierre de sesión es local: se limpia el almacenamiento y se
		// redirige. Si tu API agrega POST /api/auth/logout, llámalo aquí antes de
		// clearAll().
		AppStorageService.clearAll();
		setTimeout(() => {
			this.$app.hideLoading();
			this.router.navigate(['/login']);
		}, 800);
	}

	isAuthenticated(): boolean {
		return !!AppStorageService.getAccessToken();
	}

	async verificarUsuario(): Promise<boolean> {
		// No hay un endpoint "usuario actual" dedicado en el esqueleto: se usa el
		// endpoint protegido de ejemplo, que responde 200 solo con un JWT válido.
		// Cámbialo por el endpoint definitivo cuando exista.
		if (!AppStorageService.getAccessToken()) {
			return false;
		}
		try {
			await this._http.GET(`${this.url}/api/EjemploProtegido/yo`);
			return true;
		} catch {
			return false;
		}
	}
}
