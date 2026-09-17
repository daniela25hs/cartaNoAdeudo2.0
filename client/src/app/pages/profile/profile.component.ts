import { Component, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PageContentComponent } from '../../layouts/main-layout/components/page-content/page-content.component';
import { InputComponent } from '../../shared/components';
import { UserState } from '../../shared/states/user.state';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
        PageContentComponent,
        FormsModule,
        ReactiveFormsModule,
        InputComponent,
    ]
})
export class ProfileComponent {
	constructor(
		public $user: UserState
	) { }

	ngOnInit(): void {
	}

}
