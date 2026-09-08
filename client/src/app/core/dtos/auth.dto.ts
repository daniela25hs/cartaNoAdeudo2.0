// El login solo manda email/password. Si la API proxya a un IdP, el identificador
// de la aplicación lo inyecta el servidor, no el navegador.
export interface LoginRequest {
	email: string;
	password: string;
}

export interface TokenResponse {
	accessToken: string;
	refreshToken: string;
	expiresIn: number;
	usuarioId: string;
	email: string;
	nombre: string;
	roles: string[];
}
