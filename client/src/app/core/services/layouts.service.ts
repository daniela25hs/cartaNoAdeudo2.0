import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from './http.service';
import {
	ActualizarLayoutRequest,
	LayoutListItemResponse,
	LayoutMarcadoresResponse,
	LayoutResponse,
	RegistrarLayoutRequest,
} from '../dtos';

@Injectable({ providedIn: 'root' })
export class LayoutsService {
	private http = inject(HttpService);
	private base = `${environment.api_url}/api/Layouts`;

	getAll() {
		return this.http.GET(this.base) as Promise<LayoutListItemResponse[]>;
	}

	getById(id: number) {
		return this.http.GET(`${this.base}/${id}`) as Promise<LayoutResponse>;
	}

	obtenerArchivo(id: number) {
		return this.http.GET_BLOB(`${this.base}/${id}/archivo`);
	}

	previsualizarMarcadores(archivo: File) {
		const form = new FormData();
		form.append('archivo', archivo);
		return this.http.POST_FORM(
			`${this.base}/marcadores`,
			form,
		) as Promise<LayoutMarcadoresResponse>;
	}

	registrar(request: RegistrarLayoutRequest, archivo: File) {
		return this.http.POST_FORM(
			this.base,
			this.toFormData(request, archivo),
		) as Promise<LayoutResponse>;
	}

	actualizar(id: number, request: ActualizarLayoutRequest, archivo: File | null) {
		return this.http.UPDATE_FORM(
			`${this.base}/${id}`,
			this.toFormData(request, archivo),
		) as Promise<LayoutResponse>;
	}

	toggle(id: number) {
		return this.http.PATCH(`${this.base}/${id}/toggle`);
	}

	private toFormData(
		request: RegistrarLayoutRequest | ActualizarLayoutRequest,
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
