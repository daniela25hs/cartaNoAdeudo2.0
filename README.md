# Carta de No Adeudo

Sistema para la generación, firma electrónica y validación de Cartas de No
Adeudo de la Contraloría del Estado de Sonora. Automatiza la emisión del
documento a partir de solicitudes de SAP, lo firma electrónicamente con los
certificados digitales oficiales y permite su validación pública mediante
folio o código QR.

```
starter/
├── api/       # API .NET 10 + EF Core + PostgreSQL   → api/README.md
├── client/    # Cliente Angular 19 (standalone)       → client/README.md
└── mkdocs/      # Manual de usuarios

```

## Requisitos

### API

- .NET 10 SDK
- PostgreSQL 16+

### Client

- Node.js 20+
- npm 10+ 
- Angular CLI 19
