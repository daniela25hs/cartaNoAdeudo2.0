import { Pipe, PipeTransform } from '@angular/core';
import { AppTexts } from '../../app.texts';

@Pipe({
	name: 'pageTexts',
	standalone: true
})
export class PageTextsPipe implements PipeTransform {

	transform(value: string): string {
		return value.split('.').reduce((o: any, k) => o && o[k] || '', AppTexts.page)
	}
}
