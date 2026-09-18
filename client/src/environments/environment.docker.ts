// docker-compose. El navegador (no el contenedor) llama a la API, así que la URL
// es la del puerto publicado en el host: api -> 8080 (ver docker-compose.yml).
export const environment = {
	production: true,
	sessio_time_in_min: 0, // 0 = la sesión no se cierra por inactividad
	api_url: 'http://localhost:8080',
};
