declare const Swal: any;
declare const toastr: any;

type SweetAlertIconsType =
	| 'success'
	| 'info'
	| 'error'
	| 'warning'
	| 'question';
interface SweetAlertOptions {
	title?: string;
	text?: string;
	icon?: SweetAlertIconsType;
	confirmButtonText?: string;
	footer?: string;
	html?: string;
}
type ToastrMethodsType = 'success' | 'info' | 'error' | 'warning';
interface ToastrOptions {
	title?: string;
	method?: ToastrMethodsType;
	timeOut?: number;
	static?: boolean;
}
export function cAlert(
	text: string,
	options: SweetAlertOptions = {
		icon: 'success',
		confirmButtonText: 'Aceptar',
	}
): Promise<void> {
	return Swal.fire({
		...options,
		text: text,
		buttonsStyling: false,
		customClass: {
			confirmButton:
				'btn btn-' + (options.icon == 'error' ? 'danger' : options.icon),
		},
	});
}
export function cConfirm(
	title: string,
	options: SweetAlertOptions = { confirmButtonText: 'Aceptar' },
	showInput = false
): Promise<boolean | string> {
	const config: any = {
		...options,
		text: title,
		icon: 'warning',
		showCancelButton: true,
		confirmButtonColor: '#9B2F3E',
		cancelButtonColor: '#999',
		cancelButtonText: 'Cancelar',
		// customClass: {
		//   confirmButton: "btn btn-warning",
		//   cancelButton: "btn btn-primary",
		// },
	};
	if (showInput) {
		config.input = 'text';
	}
	return Swal.fire(config).then((r: any) => {
		if (showInput) {
			return r.isConfirmed && r.value;
		}
		return r.isConfirmed;
	});
}
export function toast(text: string, options: ToastrOptions = {}) {
	options = {
		method: 'success',
		...options,
	};
	toastr.options = {
		closeButton: false,
		debug: false,
		newestOnTop: false,
		progressBar: true,
		positionClass: 'toastr-top-right',
		preventDuplicates: true,
		onclick: null,
		showDuration: '300',
		hideDuration: '1000',
		timeOut: options.static ? null : options.timeOut || 5000,
		extendedTimeOut: options.static ? null : options.timeOut || 5000,
		showEasing: 'swing',
		hideEasing: 'linear',
		showMethod: 'fadeIn',
		hideMethod: 'fadeOut',
	};

	toastr[options.method!](text, options.title);
}
