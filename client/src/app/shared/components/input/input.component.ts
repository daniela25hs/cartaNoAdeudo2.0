import { AfterContentInit, AfterViewInit, ChangeDetectorRef, Component, ContentChild, ElementRef, EventEmitter, input, Input, Output, ViewChild } from '@angular/core';

declare const $: any

@Component({
	selector: 'app-input',
	standalone: true,
	templateUrl: './input.component.html',
	styleUrls: ['./input.component.scss']
})
export class InputComponent implements AfterViewInit {
	label = input<string>();
	error = input<string>();
	valid  = input(false);
	@ViewChild('contentWrap') contentWrap!: ElementRef;
	content: HTMLCollection | undefined;
	required = false
	constructor(private cd: ChangeDetectorRef) { }
	ngAfterViewInit(): void {
		this.content = (this.contentWrap.nativeElement as HTMLDivElement).children;
		this.required = !!$(this.content).attr('required');
		this.cd.detectChanges()
	}
}
