// Infraestructura de control de acceso por rol/permiso: la consumen
// rolePermissionsChildGuard (core/guards) y RoleControlDirective vía
// route.data.roles / route.data.permissions y [appRoleControl].
//
// Se usa `type = string` (no un enum): los roles son strings arbitrarios que
// entrega el IdP en el JWT, no un conjunto cerrado conocido en compilación. Un
// enum de string vacío además rompe la inferencia de matchArrays<T>() en
// shared/functions/objects.ts.
//
// TODO: si tu app maneja un catálogo cerrado de roles/permisos, cámbialo por una
// unión de literales. El mecanismo (guard + directiva + UserState.roles()) ya
// está listo.
export type Roles = string;
export type Permissions = string;
