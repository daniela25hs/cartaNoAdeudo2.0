import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { IsNoE } from '../../shared/functions';

@Injectable({
	providedIn: 'root',
})
export class ValidatorService {
	public static emailPattern = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$/;
	public static rfcPattern = /^([A-ZÑ&]{3,4}) ?(?:- ?)?(\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])) ?(?:- ?)?([A-Z\d]{2})([A\d])$/;
	public static curpPattern = /^[A-Z]{4}\d{6}[HM][A-Z]{2}[B-DF-HJ-NP-TV-Z]{3}[A-Z0-9][0-9]$/;
	public static strongPasswordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12,}$/;

	constructor() { }

	strongPassword(control: AbstractControl): ValidationErrors | null {
		const password: string = control.value;

		if (!ValidatorService.strongPasswordPattern.test(password)) {
			return { strongPassword: true };
		}

		return null;
	}
	equalInputs(input1: string, input2: string): ValidatorFn {
		return (formGroup: AbstractControl): ValidationErrors | null => {
			const input1FC = formGroup.get(input1)
			const input2FC = formGroup.get(input2)

			if (input1FC && input2FC) {
				if (input1FC.value === input2FC.value) {
					if (input2FC.hasError('noIguales')) {
						delete input2FC.errors!.noIguales
						input2FC.setErrors(IsNoE(input2FC.errors) ? null : input2FC.errors)
					}
				} else {
					input2FC.setErrors({
						...input2FC.errors,
						noIguales: true
					})
					return { noIguales: true }
				}
			}
			return null
		};
	}
}
