import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PageContentComponent } from '../../layouts/main-layout/components';
import { InputComponent, SelectComponent } from '../../shared/components';
import { CartasService, ValidatorService } from '../../core/services';
import {
	CLAVE_TIPO_CARTA_ALCOHOLES,
	CartaAceptadaResponse,
	ESTATUS_SOLICITUD_LABELS,
	TipoCartaResponse,
} from '../../core/dtos';
import { cAlert } from '../../shared/functions';

type TipoCartaOption = TipoCartaResponse & { label: string };

@Component({
	selector: 'app-cartas',
	standalone: true,
	templateUrl: './cartas.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [FormsModule, PageContentComponent, InputComponent, SelectComponent],
})
export class CartasComponent implements OnInit {
	private service = inject(CartasService);

	estatusLabels = ESTATUS_SOLICITUD_LABELS;

	tiposCarta = signal<TipoCartaOption[]>([]);
	tipoCartaSeleccionado = signal<TipoCartaOption | undefined>(undefined);
	cargandoTipos = signal(false);
	enviando = signal(false);
	resultado = signal<CartaAceptadaResponse | null>(null);

	esAlcoholes = computed(
		() => this.tipoCartaSeleccionado()?.clave === CLAVE_TIPO_CARTA_ALCOHOLES,
	);

	rfc = '';
	nombre = '';
	ro = '';
	inicioVigencia = '';
	licAlcoholes = '';
	email = '';

	async ngOnInit(): Promise<void> {
		this.cargandoTipos.set(true);
		try {
			this.tiposCarta.set(
				(await this.service.listarTipos()).map((t) => ({
					...t,
					label: `${t.clave} - ${t.descripcion}`,
				})),
			);
		} finally {
			this.cargandoTipos.set(false);
		}
	}

	async solicitar(): Promise<void> {
		if (
			!this.rfc.trim() ||
			!this.nombre.trim() ||
			!this.ro.trim() ||
			!this.tipoCartaSeleccionado() ||
			!this.inicioVigencia ||
			!this.email.trim()
		) {
			cAlert('Completa todos los campos obligatorios.', { icon: 'error' });
			return;
		}
		if (!ValidatorService.emailPattern.test(this.email.trim().toLowerCase())) {
			cAlert('El correo electrónico no es válido.', { icon: 'error' });
			return;
		}
		if (this.esAlcoholes() && !this.licAlcoholes.trim()) {
			cAlert('La licencia de alcoholes es obligatoria para este tipo de carta.', {
				icon: 'error',
			});
			return;
		}

		this.enviando.set(true);
		try {
			const resultado = await this.service.solicitar({
				rfc: this.rfc.trim().toUpperCase(),
				nombre: this.nombre.trim(),
				ro: this.ro.trim(),
				tipoCarta: this.tipoCartaSeleccionado()!.clave,
				inicioVigencia: this.inicioVigencia,
				licAlcoholes: this.esAlcoholes() ? this.licAlcoholes.trim() : null,
				email: this.email.trim(),
			});
			this.resultado.set(resultado);
		} finally {
			this.enviando.set(false);
		}
	}

	nuevaSolicitud(): void {
		this.resultado.set(null);
		this.rfc = '';
		this.nombre = '';
		this.ro = '';
		this.tipoCartaSeleccionado.set(undefined);
		this.inicioVigencia = '';
		this.licAlcoholes = '';
		this.email = '';
	}
}
