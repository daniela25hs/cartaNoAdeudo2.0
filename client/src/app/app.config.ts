import { registerLocaleData } from '@angular/common';
import { provideHttpClient, withInterceptors, withXhr } from '@angular/common/http';
import localeEsMx from '@angular/common/locales/es-MX';
import {
	ApplicationConfig,
	LOCALE_ID,
	inject,
	provideAppInitializer,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { DialogService } from 'primeng/dynamicdialog';
import { routes } from './app.routes';
import { AuthService } from './core/services';
import { errorInterceptor, requestInterceptor } from './core/interceptors';
import { providePrimeNG } from 'primeng/config';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import Aura from '@primeuix/themes/aura';

registerLocaleData(localeEsMx, 'es-Mx');

export const appConfig: ApplicationConfig = {
	providers: [
		// provideAppInitializer(() => {
		//   const initializerFn = ((_auth: AuthService) => () => _auth.initLlaveClient())(inject(AuthService));
		//   return initializerFn();
		// }),
		{ provide: LOCALE_ID, useValue: 'es-Mx' },
		provideRouter(routes),
		provideHttpClient(withXhr(), 
			withInterceptors([requestInterceptor, errorInterceptor]),
		),
		provideAnimationsAsync(),
		providePrimeNG({
			theme: {
				preset: Aura,
				options: {
					darkModeSelector: false,
				},
			},
		}),
	],
};
