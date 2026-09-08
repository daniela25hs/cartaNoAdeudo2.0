import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { PageContentComponent } from '../../layouts/main-layout/components';
import { UserState } from '../../shared/states/user.state';

// Placeholder mínimo. Reemplaza el contenido por el de tu aplicación.
@Component({
	standalone: true,
	templateUrl: './dashboard.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [PageContentComponent],
})
export class DashboardComponent {
	$user = inject(UserState);
}
