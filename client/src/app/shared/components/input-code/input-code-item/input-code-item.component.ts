import { CommonModule } from '@angular/common';
import { Component, ElementRef, output, ViewChild } from '@angular/core';
export interface InputCodeItemChange {
	// value: string,
	next: boolean,
	prev: boolean
}
@Component({
    selector: 'app-input-code-item',
    imports: [CommonModule],
    templateUrl: './input-code-item.component.html',
    styleUrls: ['./input-code-item.component.scss']
})
export class InputCodeItemComponent {
	onKeyDown = output<InputCodeItemChange>()
	onKeyUp = output()
	@ViewChild('input') input!: ElementRef<HTMLInputElement>
	keyDown(e: KeyboardEvent) {
		if (e.key.match(/[0-9]|\bBackspace\b/g)) {
			this.onKeyDown.emit({
				next: e.code !== 'Backspace',
				prev: e.code === 'Backspace',
			})
		}
	}
	keyUp(e: KeyboardEvent) {
		this.onKeyUp.emit()
	}
	focus() {
		setTimeout(() => {
			this.input.nativeElement.focus()
		}, 0);
	}
}
