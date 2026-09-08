import { CommonModule } from '@angular/common';
import { Component, input } from '@angular/core';

export interface Step {
	id: string;
	title: string;
	active?: boolean;
}

@Component({
	selector: 'app-stepper',
	imports: [CommonModule],
	templateUrl: './stepper.component.html',
	styles: `
		@use '../../../../variables.scss' as *;

		.stepper {
			display: flex;
			align-items: center;
			justify-content: space-between;
			margin-bottom: 2rem;
			position: relative;
			padding: 0 1rem;
			max-width: 600px;
			margin-left: auto;
			margin-right: auto;
		}

		.step-container {
			display: flex;
			flex-direction: column;
			align-items: center;
			position: relative;
			flex: 1;
		}

		// Línea conectora entre pasos (relativa al step-container, no al indicador)
		.step-container:not(:first-child)::before {
			content: '';
			position: absolute;
			top: 25px;
			left: -50%;
			right: 50%;
			height: 2px;
			background-color: #a9a9a9;
			z-index: 0;
		}

		.step-indicator {
			width: 50px;
			height: 50px;
			border-radius: 50%;
			display: flex;
			align-items: center;
			justify-content: center;
			border: 2px solid #a9a9a9;
			background-color: white;
			margin-bottom: 0.75rem;
			position: relative;
			z-index: 1;
			transition: all 0.3s ease;
			box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);

			&.active {
				background-color: $dark-pink;
				border-color: $dark-pink;
				box-shadow: 0 4px 8px rgba(63, 0, 63, 0.3);
			}

			&.completed {
				background-color: $pink;
				border-color: $pink;
				box-shadow: 0 4px 8px rgba(63, 0, 63, 0.3);
			}
		}

		.step-number {
			font-weight: bold;
			font-size: 1.2rem;
			color: $dark-pink;
			transition: color 0.3s ease;

			.step-indicator.active & {
				color: white;
			}
		}

		.step-checkmark {
			font-weight: bold;
			font-size: 1.5rem;
			color: white;
			transition: color 0.3s ease;
		}

		.step-label {
			font-size: 1rem;
			color: #666;
			text-align: center;
			font-weight: normal;
			transition: all 0.3s ease;
			min-height: 1.2rem;

			&.active {
				color: #000;
				font-weight: bold;
			}

			&.completed {
				color: #000;
				font-weight: bold;
			}
		}
	`,
})
export class StepperComponent {
	steps = input.required<Step[]>();
	currentStep = input.required<number>();
}
