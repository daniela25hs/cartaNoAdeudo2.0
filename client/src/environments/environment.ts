// Desarrollo local. `api_url` es el origen de la API (sin /api al final; los
// servicios agregan la ruta). Ajusta el puerto al de tu API.
export const environment = {
	production: false,
	sessio_time_in_min: 0, // 0 = la sesión no se cierra por inactividad
	api_url: 'http://localhost:5080',
};
