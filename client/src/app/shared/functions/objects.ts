export function hasJsonStructure(str: string) {
	if (typeof str !== 'string') return false;
	try {
		const result = JSON.parse(str);
		const type = Object.prototype.toString.call(result);
		return (
			(type === '[object Object]' || type === '[object Array]') && result
		);
	} catch (err) {
		return false;
	}
}
export function IsNoE(value: any) {
	return (
		value == null ||
		typeof value === 'undefined' ||
		value.length == 0 ||
		(typeof value === 'string' && value.trim() == '')
	);
}
export function matchArrays<T>(array1: T[], array2: T[]) {
	array1 = array1 || [];
	array2 = array2 || [];
	return array1.filter((element) => array2.includes(element));
}

export function removeNullProperties(obj: any): { [k: string]: any; } {
	const filteredEntries = Object.entries(obj).filter(([_, value]) => !IsNoE(value));
	return Object.fromEntries(filteredEntries);
}

export function filterObjectFields<T>(original: any, keys: (keyof T)[]): Partial<T> {
	const filtered: Partial<T> = {};

	keys.forEach(key => {
		if (original[key] !== undefined) {
			filtered[key] = original[key];
		}
	});

	return filtered;
}
