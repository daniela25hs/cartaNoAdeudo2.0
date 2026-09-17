import { NgClass } from '@angular/common';
import { Component, input, Input, output, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SideBarItem } from '../../../../core/models';
import { PermissionControlDirective, RemoveHostDirective } from '../../../../shared/directives';

@Component({
    selector: 'app-sidebar-item',
    templateUrl: './sidebar-item.component.html',
    styleUrls: ['./sidebar-item.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
        PermissionControlDirective,
        RouterLinkActive,
        RouterLink,
        NgClass,
        RemoveHostDirective,
    ]
})
export class SidebarItemComponent {
	sidebarItem = input.required<SideBarItem>();
	click = output<SideBarItem>()
}
