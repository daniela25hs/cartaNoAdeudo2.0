import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-public-layout',
    imports: [
        RouterOutlet
    ],
    template: `
	<router-outlet></router-outlet>
	`,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class PublicLayoutComponent { }
