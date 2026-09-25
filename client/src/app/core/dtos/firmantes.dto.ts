// Contratos alineados con CartaNoAdeudoApi.Core.DTOs.{Requests,Responses}.Firmantes
// y el enum CartaNoAdeudoApi.Core.Entities.Catalogos.Genero.

export enum Genero {
	NoEspecificado = 0,
	Masculino = 1,
	Femenino = 2,
}

export const GENERO_OPTIONS: { id: Genero; name: string }[] = [
	{ id: Genero.NoEspecificado, name: 'No especificado' },
	{ id: Genero.Masculino, name: 'Masculino' },
	{ id: Genero.Femenino, name: 'Femenino' },
];

export interface FirmanteListItemResponse {
	id: string;
	nombre: string;
	puesto: string | null;
	estatusCertificado: string | null;
	vigenciaOperativaInicio: string;
	vigenciaOperativaFin: string;
	activo: boolean;
	proximoAVencer: boolean;
}

export interface FirmanteResponse {
	id: string;
	nombre: string;
	puesto: string | null;
	genero: Genero;
	certificado: string | null;
	estatusCertificado: string | null;
	fechaInicioCertificado: string;
	fechaFinCertificado: string;
	vigenciaOperativaInicio: string;
	vigenciaOperativaFin: string;
	activo: boolean;
	proximoAVencer: boolean;
}

// El nombre no se captura: sale del CN del certificado del PFX.
export interface RegistrarFirmanteRequest {
	contrasena: string;
	puesto?: string | null;
	genero: Genero;
	vigenciaOperativaInicio: string;
	vigenciaOperativaFin: string;
	activo: boolean;
}

// pfx opcional: si se omite se conserva el certificado actual (requiere
// contrasena para validarlo). Si se adjunta, contrasena es obligatoria.
export interface ActualizarFirmanteRequest {
	puesto?: string | null;
	genero: Genero;
	vigenciaOperativaInicio: string;
	vigenciaOperativaFin: string;
	contrasena?: string | null;
}
