import { JsonPipe } from '@angular/common';
import { Component, ElementRef, EventEmitter, input, Input, model, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';

@Component({
    selector: 'app-select',
    imports: [
        DropdownModule,
        FormsModule,
    ],
    templateUrl: './select.component.html',
    styleUrls: ['./select.component.scss']
})
export class SelectComponent implements OnInit, OnChanges {
	items = input.required<Record<string, any>[]>()
	keyField = input('id')
	descriptionField = input('description')
	required = input(false)
	disabled = input(false)
	placeholder = input('Seleccione item')
	invalid = input(false)
	value = model.required<Record<string, any> | undefined>()

	filtered: Record<string, any>[] = [];
	selectedItem?: Record<string, any>

	constructor() { }

	ngOnInit(): void {
	}

	ngOnChanges(changes: SimpleChanges): void {
		if (changes.value) {
			this.selectedItem = changes.value.currentValue
		}
	}

	onChange(event: any) {
		this.value.set(this.selectedItem);
	}
}
