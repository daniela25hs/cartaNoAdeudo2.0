import { AnimationTriggerMetadata, trigger, transition, style, animate } from '@angular/animations';


export function FadeInOut(timingIn: number, timingOut: number): AnimationTriggerMetadata {
	return trigger('fadeInOut', [
		transition(':enter', [
			style({ opacity: 0, }),
			animate(timingIn, style({ opacity: 1, })),
		]),
		transition(':leave', [
			animate(timingOut, style({ opacity: 0 })),
		]),
	]);
}
export function FadeOut(timing: number): AnimationTriggerMetadata {
	return trigger('fadeOut', [
		transition(':leave', [
			animate(timing, style({ opacity: 0 })),
		])
	]);
}
