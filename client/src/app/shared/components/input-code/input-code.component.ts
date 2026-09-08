import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, output, QueryList, ViewChildren } from '@angular/core';
import { InputCodeItemChange, InputCodeItemComponent } from './input-code-item/input-code-item.component';

@Component({
    selector: 'app-input-code',
    imports: [CommonModule, InputCodeItemComponent],
    templateUrl: './input-code.component.html',
    styleUrls: ['./input-code.component.scss']
})
export class InputCodeComponent implements AfterViewInit {
	onChange = output<string>()
	@ViewChildren(InputCodeItemComponent) inputs!: QueryList<InputCodeItemComponent>;

	ngAfterViewInit(): void {
	}
	getInputsValue(): string {
		const values = this.inputs.map((i) => i.input.nativeElement.value)
		if (values.includes('')) {
			return ''
		}
		return values.join('')
	}
	inputKeyDown(e: InputCodeItemChange, ix: number) {
		let c: InputCodeItemComponent | undefined
		if (e.prev) {
			c = this.inputs.get(ix - 1)
		} else if (e.next) {
			c = this.inputs.get(ix + 1)
		}
		c && c?.focus()
	}
	inputKeyUp() {
		this.onChange.emit(this.getInputsValue())
	}
}
