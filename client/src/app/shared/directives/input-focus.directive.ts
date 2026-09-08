import { Directive, ElementRef, OnInit } from '@angular/core';

@Directive({
	selector: '[appInputFocus]',
	standalone: true
})
export class InputFocusDirective implements OnInit {

	constructor(private _el: ElementRef<HTMLInputElement>) { }
	ngOnInit(): void {
		setTimeout(() => {
			this._el.nativeElement.focus()
		}, 100);
	}

}
