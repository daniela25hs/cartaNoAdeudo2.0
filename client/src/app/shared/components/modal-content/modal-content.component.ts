import { Component, Input, OnInit } from '@angular/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

declare const $: any;
@Component({
	selector: 'app-modal-content',
	standalone: true,
	templateUrl: './modal-content.component.html',
	styleUrls: ['./modal-content.component.scss'],
})
export class ModalContentComponent implements OnInit {
	title!: string;
	constructor(public dialog: DynamicDialogRef) {}

	ngOnInit(): void {
		$('.p-dialog-title').text(this.title);
	}

	close() {
		this.dialog.close();
	}

	closeSuccess() {
		this.dialog.close(true);
	}
}
