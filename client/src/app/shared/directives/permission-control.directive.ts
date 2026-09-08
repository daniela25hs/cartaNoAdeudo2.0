import { Directive, ElementRef, input, Input, Renderer2 } from '@angular/core';
import { AppStorageService } from '../../core/services';
import { matchArrays } from '../functions';
import { Permissions } from '../../core/data';
import { UserState } from '../states/user.state';

@Directive({
	selector: '[appPermissionControl]',
	standalone: true,
})
export class PermissionControlDirective {
	appPermissionControl = input.required<Permissions[]>(); // permisos permitidos
	constructor(
		private elementRef: ElementRef,
		private renderer: Renderer2,
		private $user: UserState,
	) {}
	ngOnInit() {
		if (this.appPermissionControl && this.appPermissionControl.length == 0) {
			return;
		}
		this.renderer.setStyle(this.elementRef.nativeElement, 'display', 'none');
		// this.checkPermissions();
	}
	// checkPermissions() {
	// 	const user_permissions = this.$user.permissions() || [];
	// 	const match = matchArrays(
	// 		this.appPermissionControl().map((i) => i),
	// 		user_permissions.map((i) => i.key)
	// 	);
	// 	if (match.length > 0) {
	// 		this.renderer.setStyle(this.elementRef.nativeElement, 'display', 'unset');
	// 	} else {
	// 		this.elementRef.nativeElement.remove();
	// 	}
	// }
}
