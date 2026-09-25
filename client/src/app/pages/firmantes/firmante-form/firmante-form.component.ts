import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputComponent, SelectComponent } from '../../../shared/components';
import { FirmantesService } from '../../../core/services';
import { FirmanteResponse, GENERO_OPTIONS, Genero } from '../../../core/dtos';
import { cAlert } from '../../../shared/functions';

@Component({
	selector: 'app-firmante-form',
	standalone: true,
	templateUrl: './firmante-form.component.html',
	changeDetection: ChangeDetectionStrategy.Default,
	imports: [FormsModule, InputComponent, SelectComponent],
})
export class FirmanteFormComponent implements OnInit {
	private service = inject(FirmantesService);
	private ref = inject(DynamicDialogRef);
	private config = inject(DynamicDialogConfig);

	firmante = this.config.data?.firmante as FirmanteResponse | undefined;
	esEdicion = !!this.firmante;

	generoOptions = GENERO_OPTIONS;
	generoSeleccionado = signal<{ id: Genero; name: string } | undefined>(undefined);

	puesto = '';
	vigenciaInicio = '';
	vigenciaFin = '';
	activo = true;
	contrasena = '';
	archivo: File | null = null;

	guardando = signal(false);

	ngOnInit(): void {
		if (this.firmante) {
			this.puesto = this.firmante.puesto ?? '';
			this.vigenciaInicio = this.firmante.vigenciaOperativaInicio;
			this.vigenciaFin = this.firmante.vigenciaOperativaFin;
			this.activo = this.firmante.activo;
			this.generoSeleccionado.set(
				GENERO_OPTIONS.find((g) => g.id === this.firmante!.genero),
			);
		} else {
			this.generoSeleccionado.set(GENERO_OPTIONS[0]);
		}
	}

	onArchivoSeleccionado(event: Event): void {
		const input = event.target as HTMLInputElement;
		this.archivo = input.files?.[0] ?? null;
	}

	async guardar(): Promise<void> {
		if (!this.esEdicion && !this.archivo) {
			cAlert('Selecciona el archivo .pfx del certificado.', { icon: 'error' });
			return;
		}
		if (!this.esEdicion && !this.contrasena.trim()) {
			cAlert('La contraseña del certificado es obligatoria.', { icon: 'error' });
			return;
		}
		if (this.archivo && !this.contrasena.trim()) {
			cAlert('Indica la contraseña del certificado que estás cargando.', {
				icon: 'error',
			});
			return;
		}

		this.guardando.set(true);
		try {
			if (this.esEdicion) {
				await this.service.actualizar(
					this.firmante!.id,
					{
						puesto: this.puesto || null,
						genero: this.generoSeleccionado()!.id,
						vigenciaOperativaInicio: this.vigenciaInicio,
						vigenciaOperativaFin: this.vigenciaFin,
						contrasena: this.contrasena || null,
					},
					this.archivo,
				);
			} else {
				await this.service.registrar(
					{
						contrasena: this.contrasena,
						puesto: this.puesto || null,
						genero: this.generoSeleccionado()!.id,
						vigenciaOperativaInicio: this.vigenciaInicio,
						vigenciaOperativaFin: this.vigenciaFin,
						activo: this.activo,
					},
					this.archivo!,
				);
			}
			this.ref.close(true);
		} finally {
			this.guardando.set(false);
		}
	}

	cancelar(): void {
		this.ref.close(false);
	}
}
