import { Directive, ElementRef, input, Input, OnInit, Renderer2 } from '@angular/core';
import { Roles } from '../../core/data';
import { AppStorageService } from '../../core/services';
import { matchArrays } from '../functions';
import { UserState } from '../states/user.state';

@Directive({
	selector: '[appRoleControl]',
	standalone: true,
})
export class RoleControlDirective implements OnInit {
	appRoleControl = input.required<Roles[]>() // roles permitidos
	constructor(private elementRef: ElementRef, private renderer: Renderer2, private $user: UserState) { }
	ngOnInit() {
		if (this.appRoleControl && this.appRoleControl.length == 0) {
			return;
		}
		this.renderer.setStyle(this.elementRef.nativeElement, 'display', 'none');
		this.checkRol();
	}
	checkRol() {
		const user_roles = this.$user.roles() || [];
		const match = matchArrays(this.appRoleControl(), user_roles.map((i) => i));
		if (match.length > 0) {
			this.renderer.setStyle(this.elementRef.nativeElement, 'display', 'block');
		} else {
			this.elementRef.nativeElement.remove();
		}
	}
}
