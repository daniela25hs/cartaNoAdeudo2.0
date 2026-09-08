import { Component } from '@angular/core';
import { PageTextsPipe } from '../../../shared/pipes';

@Component({
    selector: 'app-contact-info',
    templateUrl: './contact-info.component.html',
    styleUrls: ['./contact-info.component.scss'],
    imports: [PageTextsPipe]
})
export class ContactInfoComponent {
	constructor() { }
}
