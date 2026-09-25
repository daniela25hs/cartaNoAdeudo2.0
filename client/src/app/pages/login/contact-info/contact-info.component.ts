import { Component, ChangeDetectionStrategy } from '@angular/core';
import { PageTextsPipe } from '../../../shared/pipes';

@Component({
    selector: 'app-contact-info',
    templateUrl: './contact-info.component.html',
    styleUrls: ['./contact-info.component.scss'],
    changeDetection: ChangeDetectionStrategy.Default,
    imports: [PageTextsPipe]
})
export class ContactInfoComponent {
	constructor() { }
}
