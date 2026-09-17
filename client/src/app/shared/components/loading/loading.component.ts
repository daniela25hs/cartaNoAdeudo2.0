
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, NgZone, OnDestroy, OnInit, computed, effect, input, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { FadeInOut } from '../../functions';
import { AppState } from '../../states/app.state';

@Component({
    selector: 'app-loading',
    templateUrl: './loading.component.html',
    styleUrls: ['./loading.component.scss'],
    imports: [],
    changeDetection: ChangeDetectionStrategy.OnPush,
    animations: [FadeInOut(100, 200)]
})
export class LoadingComponent {
	listener = input.required<boolean>()
	isLoading = computed(() => {
		if (this.listener()) {
			return this.$app.isLoading()
		}
		return true
	});

	constructor(private $app: AppState) { }
}
