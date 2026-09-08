export interface IdName {
	id: number;
	name: string;
}
export interface KeyName {
	key: string;
	name: string;
}

export interface ApiResponse<T> {
	success: boolean;
	data?: T;
	error?: string;
}

export class ApiResponseFactory {
	static ok<T>(data: T): ApiResponse<T> {
		return {
			success: true,
			data,
		};
	}

	static fail<T>(error: string): ApiResponse<T> {
		return {
			success: false,
			error,
		};
	}
}

export interface PagedResponse<T> {
	items: T[];
	totalCount: number;
	page: number;
	pageSize: number;
	totalPages: number;
}
