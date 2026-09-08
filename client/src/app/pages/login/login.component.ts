import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DialogService } from 'primeng/dynamicdialog';
import { AppStorageService, AuthService } from '../../core/services';
import { PageTextsPipe } from '../../shared/pipes';
import { AppState } from '../../shared/states/app.state';
import { ContactInfoComponent } from './contact-info/contact-info.component';
import { cAlert } from '../../shared/functions';
import { UserState } from '../../shared/states/user.state';

@Component({
	selector: 'app-login',
	imports: [FormsModule, PageTextsPipe],
	providers: [DialogService],
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
	email = '';
	password = '';
	submitted = false;
	dialogService = inject(DialogService);

	constructor(
		private _auth: AuthService,
		public router: Router,
		private $app: AppState,
		private $user: UserState,
	) {}

	ngOnInit(): void {
		this.$app.hideLoading();
	}

	async login() {
		if (!this.email.trim() || !this.password.trim()) {
			return cAlert('Ingrese un usuario y contraseña válido', {
				icon: 'error',
			});
		}
		const res = await this._auth.login(this.email, this.password);

		if (!res.success || !res.data) {
			cAlert(res.error || 'Usuario o contraseña incorrectos', {
				icon: 'error',
			});
			return;
		}
		AppStorageService.setTokens(res.data);
		this.$user.setFromToken({
			usuarioId: res.data.usuarioId,
			nombre: res.data.nombre,
			email: res.data.email,
			roles: res.data.roles,
		});
		this.router.navigate(['/app']);
	}

	openContactInfo() {
		this.dialogService.open(ContactInfoComponent, {
			header: 'Información de contacto',
			width: '500px',
		});
	}
}
