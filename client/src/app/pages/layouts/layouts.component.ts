import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { PageContentComponent } from '../../layouts/main-layout/components';
import { LayoutsService } from '../../core/services';
import { LayoutListItemResponse } from '../../core/dtos';
import { cConfirm } from '../../shared/functions';
import { LayoutFormComponent } from './layout-form/layout-form.component';

@Component({
	selector: 'app-layouts',
	standalone: true,
	templateUrl: './layouts.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DialogService],
	imports: [PageContentComponent],
})
export class LayoutsComponent implements OnInit {
	private service = inject(LayoutsService);
	private dialogService = inject(DialogService);

	layouts = signal<LayoutListItemResponse[]>([]);

	async ngOnInit(): Promise<void> {
		await this.cargar();
	}

	async cargar(): Promise<void> {
		this.layouts.set(await this.service.getAll());
	}

	nuevo(): void {
		const ref = this.dialogService.open(LayoutFormComponent, {
			header: 'Nuevo layout',
			width: '650px',
		});
		ref?.onClose.subscribe((exito: boolean) => {
			if (exito) this.cargar();
		});
	}

	async ver(l: LayoutListItemResponse): Promise<void> {
		const blob = await this.service.obtenerArchivo(l.id);
		const url = URL.createObjectURL(blob);
		window.open(url, '_blank');
		setTimeout(() => URL.revokeObjectURL(url), 60_000);
	}

	async editar(id: number): Promise<void> {
		const layout = await this.service.getById(id);
		const ref = this.dialogService.open(LayoutFormComponent, {
			header: 'Editar layout',
			width: '650px',
			data: { layout },
		});
		ref?.onClose.subscribe((exito: boolean) => {
			if (exito) this.cargar();
		});
	}

	async toggle(event: Event, l: LayoutListItemResponse): Promise<void> {
		// El checkbox no cambia solo: si estaba activo (lo vamos a desactivar) se
		// confirma antes. Recargamos siempre al final para que el switch quede
		// sincronizado con el estado real del back, sin importar la respuesta.
		event.preventDefault();

		if (l.activo) {
			const confirmado = await cConfirm(
				`¿Seguro que deseas desactivar "${l.descripcion || "este layout"}"?`,
			);
			if (!confirmado) return;
		}

		try {
			await this.service.toggle(l.id);
		} finally {
			await this.cargar();
		}
	}
}
