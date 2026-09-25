import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from './http.service';
import {
	ActualizarFirmanteRequest,
	FirmanteListItemResponse,
	FirmanteResponse,
	RegistrarFirmanteRequest,
} from '../dtos';

@Injectable({ providedIn: 'root' })
export class FirmantesService {
	private http = inject(HttpService);
	private base = `${environment.api_url}/api/Firmantes`;

	getAll() {
		return this.http.GET(this.base) as Promise<FirmanteListItemResponse[]>;
	}

	getById(id: string) {
		return this.http.GET(`${this.base}/${id}`) as Promise<FirmanteResponse>;
	}

	registrar(request: RegistrarFirmanteRequest, archivo: File) {
		return this.http.POST_FORM(
			this.base,
			this.toFormData(request, archivo),
		) as Promise<FirmanteResponse>;
	}

	actualizar(id: string, request: ActualizarFirmanteRequest, archivo: File | null) {
		return this.http.UPDATE_FORM(
			`${this.base}/${id}`,
			this.toFormData(request, archivo),
		) as Promise<FirmanteResponse>;
	}

	toggle(id: string) {
		return this.http.PATCH(`${this.base}/${id}/toggle`);
	}

	private toFormData(
		request: RegistrarFirmanteRequest | ActualizarFirmanteRequest,
		archivo: File | null,
	): FormData {
		const form = new FormData();
		for (const [key, value] of Object.entries(request)) {
			if (value === undefined || value === null) continue;
			form.append(key[0].toUpperCase() + key.slice(1), String(value));
		}
		if (archivo) form.append('archivo', archivo);
		return form;
	}
}
