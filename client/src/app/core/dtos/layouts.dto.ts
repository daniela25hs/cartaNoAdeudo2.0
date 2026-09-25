// Contratos alineados con CartaNoAdeudoApi.Core.DTOs.{Requests,Responses}.Layouts.

export interface LayoutListItemResponse {
	id: number;
	descripcion: string | null;
	vigenciaInicio: string;
	vigenciaFin: string;
	activo: boolean;
}

export interface LayoutResponse {
	id: number;
	descripcion: string | null;
	archivo: string;
	fechaHora: string;
	vigenciaInicio: string;
	vigenciaFin: string;
	activo: boolean;
	marcadores: string[];
}

export interface LayoutMarcadoresResponse {
	marcadoresReconocidos: string[];
	marcadoresNoReconocidos: string[];
	marcadoresObligatoriosFaltantes: string[];
	aptoParaActivar: boolean;
}

export interface RegistrarLayoutRequest {
	descripcion: string;
	vigenciaInicio: string;
	vigenciaFin: string;
	activo: boolean;
}

export interface ActualizarLayoutRequest {
	descripcion: string;
	vigenciaInicio: string;
	vigenciaFin: string;
}
