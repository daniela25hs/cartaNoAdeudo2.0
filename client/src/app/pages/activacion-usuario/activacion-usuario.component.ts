import { Component, OnInit, inject, ChangeDetectionStrategy } from '@angular/core';
import {
	ReactiveFormsModule,
	FormGroup,
	FormControl,
	Validators,
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ValidatorService } from '../../core/services';
import { cAlert } from '../../shared/functions';
import { AppState } from '../../shared/states/app.state';

export type ActivacionMode = 'activar' | 'cambiar-password';

@Component({
	selector: 'app-activacion-usuario',
	imports: [ReactiveFormsModule],
	templateUrl: './activacion-usuario.component.html',
	changeDetection: ChangeDetectionStrategy.Eager,
	styles: ``,
})
export class ActivacionUsuarioComponent implements OnInit {
	private $app = inject(AppState);
	private route = inject(ActivatedRoute);
	private validatorService = inject(ValidatorService);

	mode: ActivacionMode = 'activar';

	get titulo(): string {
		return this.mode === 'cambiar-password'
			? 'Cambiar contraseña'
			: 'Activación de cuenta';
	}

	get subtitulo(): string {
		return this.mode === 'cambiar-password'
			? 'Estimado(a) compañero(a), ingrese los datos para restablecer su contraseña.'
			: 'Estimado(a) compañero(a), por favor capture los siguientes datos para poder realizar la activación de su cuenta.';
	}

	get labelBoton(): string {
		return this.mode === 'cambiar-password'
			? 'Cambiar contraseña'
			: 'Activar cuenta';
	}

	form = new FormGroup(
		{
			email: new FormControl('', [
				Validators.required,
				Validators.pattern(ValidatorService.emailPattern),
			]),
			token: new FormControl('', [Validators.required]),
			password: new FormControl('', [
				Validators.required,
				this.validatorService.strongPassword.bind(this.validatorService),
			]),
			confirmacionPassword: new FormControl('', [Validators.required]),
		},
		{
			validators: this.validatorService.equalInputs(
				'password',
				'confirmacionPassword',
			),
		},
	);

	ngOnInit(): void {
		this.$app.hideLoading();
		this.mode =
			(this.route.snapshot.data['mode'] as ActivacionMode) ?? 'activar';
		const qToken = this.route.snapshot.queryParamMap.get('token');
		const qEmail = this.route.snapshot.queryParamMap.get('email');
		if (qToken) this.form.get('token')!.setValue(qToken);
		if (qEmail) this.form.get('email')!.setValue(qEmail);
	}

	async submit() {
		this.form.markAllAsTouched();
		if (this.form.invalid) return;

		// El esqueleto de API no expone endpoint de activación / cambio de contraseña.
		// Se conserva el formulario (patrón de UI reutilizable); conecta aquí la
		// llamada correspondiente cuando tu API la ofrezca.
		await cAlert(
			'Esta función todavía no está disponible: la API no expone un endpoint de activación / cambio de contraseña.',
			{ icon: 'warning' },
		);
	}
}
