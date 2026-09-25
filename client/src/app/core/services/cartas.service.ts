import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpService } from './http.service';
import {
	CartaAceptadaResponse,
	EstatusSolicitud,
	GenerarCartaRequest,
	ReprocesoItemResultado,
	SolicitudListItemResponse,
	TipoCartaResponse,
} from '../dtos';

@Injectable({ providedIn: 'root' })
export class CartasService {
	private http = inject(HttpService);
	private base = `${environment.api_url}/api/Cartas`;

	listarTipos() {
		return this.http.GET(`${this.base}/tipos`) as Promise<TipoCartaResponse[]>;
	}

	solicitar(request: GenerarCartaRequest) {
		return this.http.POST(this.base, request) as Promise<CartaAceptadaResponse>;
	}

	listar(estatus?: EstatusSolicitud) {
		const query = estatus != null ? `?estatus=${estatus}` : '';
		return this.http.GET(`${this.base}${query}`) as Promise<SolicitudListItemResponse[]>;
	}

	reprocesar(id: string) {
		return this.http.POST(`${this.base}/${id}/reprocesar`) as Promise<ReprocesoItemResultado>;
	}
}
