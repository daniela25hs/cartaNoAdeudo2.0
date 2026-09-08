import {
	HttpClient,
	HttpErrorResponse,
	HttpEvent,
	HttpHandlerFn,
	HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, from, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { cAlert, hasJsonStructure } from '../../shared/functions';
import {
	X_SKIP_INTERCEPTOR,
	AuthService,
	url_refresh_token,
	AppStorageService,
} from '../services';

export const errorInterceptor = (
	request: HttpRequest<unknown>,
	next: HttpHandlerFn,
): Observable<HttpEvent<any>> => {
	if (request.headers.get(X_SKIP_INTERCEPTOR) === 'true') {
		return next(request);
	}

	const _auth = inject(AuthService);
	const _http = inject(HttpClient);

	return next(request).pipe(
		catchError((httpErrorRes: HttpErrorResponse) => {
			if (httpErrorRes.status === 401 && request.url !== url_refresh_token) {
				return tryRefreshAndRetry(next, request, _auth, _http);
			}
			return throwError(() => extractError(httpErrorRes));
		}),
	);
};

const tryRefreshAndRetry = (
	next: HttpHandlerFn,
	request: HttpRequest<any>,
	_auth: AuthService,
	_http: HttpClient,
): Observable<HttpEvent<any>> => {
	return from(_auth.refreshToken()).pipe(
		switchMap((refreshed) => {
			if (!refreshed) {
				_auth.logOut();
				return throwError(() => new Error('Sesión expirada.'));
			}
			const newToken = AppStorageService.getAccessToken()!;
			const retryReq = request.clone({
				headers: request.headers.set('Authorization', 'Bearer ' + newToken),
			});
			return _http.request(retryReq);
		}),
	);
};

/**
 * Extrae el mensaje de error de diferentes formatos de respuesta:
 * - ProblemDetails (ASP.NET Core): usa 'detail'
 * - Errores de validación: objeto con campo 'errors'
 * - Mensaje simple: campo 'message'
 * - String JSON: intenta parsear
 */
const extractError = (res: HttpErrorResponse): string => {
	let message = '';

	// 1. ProblemDetails format (ASP.NET Core 7+)
	// { "type": "...", "title": "...", "status": 500, "detail": "..." }
	if (res.error?.detail) {
		message = res.error.detail;
	}
	// 2. Traditional message field
	else if (res.error?.message) {
		message = res.error.message;
	}
	// 3. Validation errors object (FluentValidation, ModelState)
	// { "errors": { "Campo": ["Error1", "Error2"] } }
	else if (
		res.error?.errors &&
		typeof res.error.errors === 'object' &&
		!Array.isArray(res.error.errors)
	) {
		const msgs = Object.values(res.error.errors)
			.flat()
			.filter(Boolean)
			.join(', ');
		message = msgs || 'Error de validación';
	}
	// 4. Validation errors array
	// { "errors": ["Error1", "Error2"] }
	else if (Array.isArray(res.error?.errors) && res.error.errors.length > 0) {
		message = res.error.errors.join(', ');
	}
	// 5. String error response (intenta parsear JSON)
	else if (typeof res.error === 'string') {
		const parsed = hasJsonStructure(res.error);
		message = parsed?.detail || parsed?.message || res.error;
	}
	// 6. Fallback a statusText de HTTP
	else {
		message = res.message || `Error ${res.status}: ${res.statusText}`;
	}

	cAlert(message, { icon: 'error' });
	return message;
};
