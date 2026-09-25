import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { PageContentComponent } from '../../layouts/main-layout/components';
import { FirmantesService } from '../../core/services';
import { FirmanteListItemResponse } from '../../core/dtos';
import { cConfirm } from '../../shared/functions';
import { FirmanteFormComponent } from './firmante-form/firmante-form.component';

@Component({
	selector: 'app-firmantes',
	standalone: true,
	templateUrl: './firmantes.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DialogService],
	imports: [PageContentComponent],
})
export class FirmantesComponent implements OnInit {
	private service = inject(FirmantesService);
	private dialogService = inject(DialogService);

	firmantes = signal<FirmanteListItemResponse[]>([]);

	async ngOnInit(): Promise<void> {
		await this.cargar();
	}

	async cargar(): Promise<void> {
		this.firmantes.set(await this.service.getAll());
	}

	nuevo(): void {
		const ref = this.dialogService.open(FirmanteFormComponent, {
			header: 'Nuevo firmante',
			width: '600px',
		});
		ref?.onClose.subscribe((exito: boolean) => {
			if (exito) this.cargar();
		});
	}

	async editar(id: string): Promise<void> {
		const firmante = await this.service.getById(id);
		const ref = this.dialogService.open(FirmanteFormComponent, {
			header: 'Editar firmante',
			width: '600px',
			data: { firmante },
		});
		ref?.onClose.subscribe((exito: boolean) => {
			if (exito) this.cargar();
		});
	}

	async toggle(event: Event, f: FirmanteListItemResponse): Promise<void> {
		// El checkbox no cambia solo: si estaba activo (lo vamos a desactivar) se
		// confirma antes. Recargamos siempre al final para que el switch quede
		// sincronizado con el estado real del back, sin importar la respuesta.
		event.preventDefault();

		if (f.activo) {
			const confirmado = await cConfirm(
				`¿Seguro que deseas desactivar a ${f.nombre}?`,
			);
			if (!confirmado) return;
		}

		try {
			await this.service.toggle(f.id);
		} finally {
			await this.cargar();
		}
	}
}
