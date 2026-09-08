import { HttpParams } from '@angular/common/http';

export function convertToHttpParams(data: any): HttpParams {
	let params = new HttpParams();

	Object.keys(data).forEach((key) => {
		const value = data[key];
		if (Array.isArray(value)) {
			value.forEach((item: any) => {
				params = params.append(`${key}[]`, item);
			});
		} else {
			params = params.set(key, value);
		}
	});

	return params;
}
export function getSiteUrl() {
	let pathArray: string[] = window.location.pathname.split('/');
	pathArray.pop();
	return `${window.location.origin}${pathArray.join('/')}`;
}

export function loadScripts(dynamicAssets: string[]) {
	const isScriptAdded = (src: string) => {
		return Boolean(document.querySelector('script[src="' + src + '"]'));
	};
	return new Promise<void>((resolve) => {
		const da = dynamicAssets.map((i) => ({ url: i, loaded: false }));
		const check = () => {
			const c = da.filter((i) => !i.loaded).length;
			c === 0 && resolve();
		};
		da.forEach((i) => {
			const ext = i.url.split('.').pop();
			let node: HTMLScriptElement | HTMLLinkElement;
			if (ext == 'js') {
				if (isScriptAdded(i.url)) {
					return;
				}
				node = document.createElement('script');
				node.src = i.url;
				node.type = 'text/javascript';
				node.async = false;
				node.charset = 'utf-8';
			} else {
				node = document.createElement('link');
				node.href = i.url;
				node.rel = 'stylesheet';
			}
			node.onload = () => {
				i.loaded = true;
				check();
			};
			document.getElementsByTagName('head')[0].appendChild(node);
		});
	});
}

export function downloadHandled(
	url: string,
	name: string,
	// cb = (error?: any) => { },
	progress = (p: number) => {},
) {
	return new Promise<void>((resolve, reject) => {
		// todo: implementar los resolve y reject
		window.URL = window.URL || window['webkitURL'];

		const xhr = new XMLHttpRequest(),
			a = document.createElement('a');

		let file;

		xhr.open('GET', url, true);
		xhr.responseType = 'blob';
		xhr.onprogress = (evt) => {
			if (evt.lengthComputable) {
				const percentComplete = Math.floor((evt.loaded / evt.total) * 100);
				progress(percentComplete);
			}
		};
		xhr.onload = function () {
			if (this.status === 0 || this.status > 299) {
				const blobError = new Blob([xhr.response], {
					type: 'application/json',
				});
				blobError.text().then(function (z) {
					reject(JSON.parse(z));
				});
				return;
			}
			file = new Blob([xhr.response], { type: 'application/octet-stream' });
			const url_object = window.URL.createObjectURL(file);
			a.href = url_object;
			a.download = name;
			a.click();
			setTimeout(() => {
				window.URL.revokeObjectURL(url_object);
				// cb();
				resolve();
			}, 100);
		};
		xhr.onerror = function (error) {
			console.error('downloadHandled', error);
		};
		// xhr.setRequestHeader('Authorization', 'Bearer ' + AppStorageService.token());
		xhr.send();
	});
}

/**
 * Extrae el mensaje de error de diferentes estructuras de respuesta
 * @param error El error capturado
 * @param defaultMessage Mensaje por defecto si no se encuentra uno específico
 * @returns El mensaje de error extraído
 */
/**
 * Extrae el mensaje de error de diferentes estructuras de respuesta
 * @param error El error capturado
 * @param defaultMessage Mensaje por defecto si no se encuentra uno específico
 * @returns El mensaje de error extraído
 */
export function extractErrorMessage(
	error: any,
	defaultMessage: string = 'Ocurrió un error inesperado',
): string {
	// 1. Estructura directa: error.error.description (TU CASO ACTUAL) ⭐
	if (error?.error?.description) {
		return error.error.description;
	}

	// 2. Estructura directa: error.error.title
	if (error?.error?.title) {
		return error.error.title;
	}

	// 3. Estructura de HTTPExceptionSchema de FastAPI con detail
	if (error?.error?.detail?.description) {
		return error.error.detail.description;
	}

	// 4. Título del error de FastAPI con detail
	if (error?.error?.detail?.title) {
		return error.error.detail.title;
	}
	if (error?.detail) {
		return error.detail;
	}

	// 5. Detail como string directo
	if (typeof error?.error?.detail === 'string') {
		return error.error.detail;
	}

	// 6. Mensaje genérico del error
	if (error?.error?.message) {
		return error.error.message;
	}

	// 7. Mensaje del objeto error
	if (error?.message) {
		return error.message;
	}
	// 6. Mensaje genérico del error
	if (error?.description) {
		return error.description;
	}

	// 7. Mensaje del objeto error
	if (error?.message) {
		return error.message;
	}

	// 8. Si error.error es un string
	if (typeof error?.error === 'string') {
		return error.error;
	}

	return defaultMessage;
}
