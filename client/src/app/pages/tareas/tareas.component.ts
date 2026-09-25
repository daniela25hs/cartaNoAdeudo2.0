import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { PageContentComponent } from '../../layouts/main-layout/components';
import { CartasService } from '../../core/services';
import { EstatusSolicitud, SolicitudListItemResponse } from '../../core/dtos';
import { cConfirm, toast } from '../../shared/functions';

@Component({
	selector: 'app-tareas',
	standalone: true,
	templateUrl: './tareas.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [PageContentComponent],
})
export class TareasComponent implements OnInit {
	private service = inject(CartasService);

	EstatusSolicitud = EstatusSolicitud;

	solicitudes = signal<SolicitudListItemResponse[]>([]);
	soloErrores = signal(true);
	reintentando = signal<string | null>(null);

	async ngOnInit(): Promise<void> {
		await this.cargar();
	}

	async cargar(): Promise<void> {
		this.solicitudes.set(
			await this.service.listar(
				this.soloErrores() ? EstatusSolicitud.ErrorDefinitivo : undefined,
			),
		);
	}

	async cambiarFiltro(soloErrores: boolean): Promise<void> {
		this.soloErrores.set(soloErrores);
		await this.cargar();
	}

	async reintentar(s: SolicitudListItemResponse): Promise<void> {
		const confirmado = await cConfirm(
			`¿Reintentar la firma de la solicitud de ${s.nombre} (folio ${s.folio || s.id})?`,
		);
		if (!confirmado) return;

		this.reintentando.set(s.id);
		try {
			await this.service.reprocesar(s.id);
			toast('La solicitud se reencoló para volver a firmarse.');
		} finally {
			this.reintentando.set(null);
			await this.cargar();
		}
	}
}
