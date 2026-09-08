import { HttpClient, HttpContext, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export const X_SKIP_INTERCEPTOR = 'X-Skip-Interceptor'

type HttpClientOptionsType = {
	body?: any;
	headers?: HttpHeaders | {
		[header: string]: string | string[];
	};
	context?: HttpContext;
	observe?: 'body';
	params?: HttpParams | {
		[param: string]: string | number | boolean | ReadonlyArray<string | number | boolean>;
	};
	responseType?: 'json';
	reportProgress?: boolean;
	withCredentials?: boolean;
	skipInterceptor?: boolean;
}
type HttpMethodsType = 'GET' | 'POST' | 'PATCH' | 'DELETE' | 'PUT'

@Injectable({
	providedIn: 'root',
})
export class HttpService {
	constructor(private _http: HttpClient) { }

	GET(url: string): Promise<any> {
		return this.request<any>('GET', url);
	}

	GET_BLOB(url: string): Promise<Blob> {
		return firstValueFrom(this._http.get(url, { responseType: 'blob' }));
	}

	DELETE(url: string) {
		return this.request<any>('DELETE', url);
	}

	POST(url: string, data?: any) {
		return this.request<any>('POST', url, { body: data });
	}

	PATCH(url: string, data?: any) {
		return this.request<any>('PATCH', url, { body: data });
	}

	UPDATE(url: string, data: any) {
		return this.request<any>('PUT', url, { body: data });
	}

	request<T>(method: HttpMethodsType, url: string, options?: HttpClientOptionsType): Promise<T> {
		if (options && !options.headers) {
			options.headers = this.defaultHeaders()
		}
		if (options && options.skipInterceptor) {
			options.headers = {
				...options.headers,
				[X_SKIP_INTERCEPTOR]: 'true'
			}
		}
		const observable = this._http.request<T>(method, url, options)
		return firstValueFrom(observable)
	}
	defaultHeaders() {
		return new HttpHeaders({ 'Content-Type': 'application/json' });
	}
}
