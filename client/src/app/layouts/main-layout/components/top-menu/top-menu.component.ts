import { TitleCasePipe } from '@angular/common';
import { Component, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services';
import { AppState } from '../../../../shared/states/app.state';
import { UserState } from '../../../../shared/states/user.state';

@Component({
    selector: 'app-top-menu',
    templateUrl: './top-menu.component.html',
    styleUrls: ['./top-menu.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
        RouterLink,
        FormsModule,
        TitleCasePipe,
    ]
})
export class TopMenuComponent{
	constructor(private _auth: AuthService, public $app: AppState, public $user: UserState) { }

	logout() {
		this._auth.logOut();
	}
}
