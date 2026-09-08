import { Signal, computed, signal } from '@angular/core';

export class BaseStateService<T> {
	readonly state = signal(this.initialState);

	constructor(public initialState: T) { }

	public select<K extends keyof T>(key: K): Signal<T[K]> {
		return computed(() => this.state()[key]);
	}

	public set<K extends keyof T>(key: K, data: T[K]) {
		this.state.update((currentValue) => ({ ...currentValue, [key]: data }));
	}

	public setState(partialState: T): void {
		this.state.update((currentValue) => ({ ...currentValue, ...partialState }));
	}

	resetState() {
		this.setState(this.initialState)
	}
}
