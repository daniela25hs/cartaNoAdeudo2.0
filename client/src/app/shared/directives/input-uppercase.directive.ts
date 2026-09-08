import { Directive, ElementRef, HostListener, Renderer2 } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
	selector: '[appInputUppercase]',
	standalone: true,
})
export class InputUppercaseDirective {
	constructor(private el: ElementRef, private ngControl: NgControl) { }

	@HostListener('input', ['$event'])
	onInput(event: Event): void {
		const inputElement = event.target as HTMLInputElement;
		const newValue = inputElement.value.toUpperCase();
		const control = this.ngControl.control
		if (control) {
			const cursorStart = inputElement.selectionStart;
			const cursorEnd = inputElement.selectionEnd;
			control.setValue(newValue, { emitEvent: false });
			inputElement.setSelectionRange(cursorStart, cursorEnd);
		}
	}
}
