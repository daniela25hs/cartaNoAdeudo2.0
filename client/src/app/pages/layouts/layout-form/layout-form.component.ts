import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputComponent } from '../../../shared/components';
import { LayoutsService } from '../../../core/services';
import { LayoutMarcadoresResponse, LayoutResponse } from '../../../core/dtos';
import { cAlert } from '../../../shared/functions';

@Component({
	selector: 'app-layout-form',
	standalone: true,
	templateUrl: './layout-form.component.html',
	changeDetection: ChangeDetectionStrategy.Default,
	imports: [FormsModule, InputComponent],
})
export class LayoutFormComponent implements OnInit {
	private service = inject(LayoutsService);
	private ref = inject(DynamicDialogRef);
	private config = inject(DynamicDialogConfig);

	layout = this.config.data?.layout as LayoutResponse | undefined;
	esEdicion = !!this.layout;

	descripcion = '';
	vigenciaInicio = '';
	vigenciaFin = '';
	activo = true;
	archivo: File | null = null;

	previsualizacion = signal<LayoutMarcadoresResponse | null>(null);
	previsualizando = signal(false);
	guardando = signal(false);

	ngOnInit(): void {
		if (this.layout) {
			this.descripcion = this.layout.descripcion ?? '';
			this.vigenciaInicio = this.layout.vigenciaInicio;
			this.vigenciaFin = this.layout.vigenciaFin;
			this.activo = this.layout.activo;
		}
	}

	async onArchivoSeleccionado(event: Event): Promise<void> {
		const input = event.target as HTMLInputElement;
		this.archivo = input.files?.[0] ?? null;
		this.previsualizacion.set(null);
		if (!this.archivo) return;

		this.previsualizando.set(true);
		try {
			this.previsualizacion.set(
				await this.service.previsualizarMarcadores(this.archivo),
			);
		} finally {
			this.previsualizando.set(false);
		}
	}

	async guardar(): Promise<void> {
		if (!this.esEdicion && !this.archivo) {
			cAlert('Selecciona la plantilla en formato .docx.', { icon: 'error' });
			return;
		}

		this.guardando.set(true);
		try {
			if (this.esEdicion) {
				await this.service.actualizar(
					this.layout!.id,
					{
						descripcion: this.descripcion,
						vigenciaInicio: this.vigenciaInicio,
						vigenciaFin: this.vigenciaFin,
					},
					this.archivo,
				);
			} else {
				await this.service.registrar(
					{
						descripcion: this.descripcion,
						vigenciaInicio: this.vigenciaInicio,
						vigenciaFin: this.vigenciaFin,
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
