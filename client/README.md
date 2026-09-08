# Client — Esqueleto base (Angular 19)

Cliente Angular con standalone components, autenticación JWT contra la API,
guards de sesión y de rol/permiso, interceptors de request/error, layout con
sidebar + top-menu y un set de componentes compartidos.

## Estructura

```
src/app/
├── core/
│   ├── data/          roles/permisos, sidebar.data.ts
│   ├── dtos/           contratos de request/response
│   ├── guards/         authGuard, isAuthenticatedGuard, rolePermissionsChildGuard
│   ├── interceptors/   requestInterceptor (Bearer + loading), errorInterceptor (401→refresh, ProblemDetails)
│   ├── models/
│   └── services/       AuthService, HttpService, AppStorageService, SessionCountdownService, ValidatorService
├── layouts/            public-layout, main-layout (sidebar + top-menu)
├── pages/              login, dashboard, profile, error, activacion-usuario
└── shared/             components, directives, functions, pipes, states (Signals)
```

Lo que **no** trae: módulos de negocio. Se agregan como hijos de la ruta `app`
(ver [`../docs/client/GUIA_DESARROLLO_CLIENT.md`](../docs/client/GUIA_DESARROLLO_CLIENT.md)).

## Requisitos

- Node 20+
- La API corriendo (por defecto se espera en `http://localhost:5080`)

## Primer arranque

```bash
npm install

# Ajusta la URL de la API si no es http://localhost:5080:
#   src/environments/environment.ts  →  api_url

npm start
# App: http://localhost:4200
```

> El esqueleto de API arranca con la integración SIGA **comentada** (sin
> autenticación), así que `POST /api/auth/login` no responde tokens todavía.
> El login empieza a funcionar cuando activas SIGA en la API (ver
> `../docs/INTEGRACION_SIGA.md`) o apuntas `api_url` a un backend que emita JWT.

## Scripts

| Comando | Qué hace |
|---|---|
| `npm start` | dev server con `environment.ts` |
| `npm run build` | build de desarrollo + PurgeCSS |
| `npm run build:qa` / `build:prod` | build con `environment.qa.ts` / `environment.prod.ts` |
| `npm test` | Karma + Jasmine |

## Personalizar la identidad

- `src/app/app.texts.ts` — título, subtítulo, logos del login/footer.
- `src/index.html` — `<title>` y favicon.
- `src/app/core/data/sidebar.data.ts` — entradas del menú.

## Contenerización

No incluida en el esqueleto. Paso a paso (Dockerfile multi-stage + nginx) en
[`../docs/client/DOCKERIZACION.md`](../docs/client/DOCKERIZACION.md).
