using System.Security.Cryptography;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Core.Options;
using CartaNoAdeudoApi.Core.Utils.Validaciones.Carta;
using CartaNoAdeudoApi.Infrastructure.Auth;
using CartaNoAdeudoApi.Infrastructure.Data;
using CartaNoAdeudoApi.Infrastructure.Data.Interceptors;
using CartaNoAdeudoApi.Infrastructure.Repositories;
using CartaNoAdeudoApi.Infrastructure.Services;

namespace CartaNoAdeudoApi.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            // Necesario para la auditoría aunque SIGA esté desactivado (sin token,
            // CurrentUserService simplemente regresa null en CreadoPor/EditadoPor).
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<AuditoriaSaveChangesInterceptor>();

            services.AddDbContext<AppDbContext>((sp, opt) =>
                opt.UseNpgsql(
                        config.GetConnectionString("Postgres"),
                        npgsql => npgsql.MigrationsAssembly("CartaNoAdeudoApi.Infrastructure"))
                   .AddInterceptors(sp.GetRequiredService<AuditoriaSaveChangesInterceptor>()));

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        /// <summary>
        /// Registra aquí cada servicio de aplicación nuevo:
        ///   services.AddScoped&lt;ICategoriaService, CategoriaService&gt;();
        /// </summary>
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ICartaService, CartaService>();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddSingleton<ICertificadoFirmaService, CertificadoFirmaService>();
            services.AddScoped<INotificacionSapService, NotificacionSapService>();
            return services;
        }

        /// <summary>
        /// Reintentos automáticos de firma en background (ver <c>TareaFirmaOptions</c>,
        /// <c>TareaFirmaQueue</c>, <c>TareaFirmaWorker</c> y <c>TareaFirmaRecovery</c>).
        /// Sección "TareaFirma" de appsettings.
        /// </summary>
        public static IServiceCollection AddTareaFirma(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<TareaFirmaOptions>(config.GetSection("TareaFirma"));

            services.AddSingleton<ITareaFirmaQueue, TareaFirmaQueue>();
            services.AddHostedService<TareaFirmaWorker>();

            // Singleton (no solo AddHostedService) para poder inyectarla también en el
            // controller, vía ITareaFirmaRecovery, y disparar la recuperación a mano sin
            // reiniciar la app.
            services.AddSingleton<TareaFirmaRecovery>();
            services.AddSingleton<ITareaFirmaRecovery>(sp => sp.GetRequiredService<TareaFirmaRecovery>());
            services.AddHostedService(sp => sp.GetRequiredService<TareaFirmaRecovery>());
            return services;
        }

        /// <summary>
        /// Extracción de marcadores y guardado en disco de los .docx de <c>Layout</c>
        /// (RF-002). Sección "LayoutStorage" de appsettings; ruta relativa se resuelve
        /// contra <c>AppContext.BaseDirectory</c>, igual que el <c>Pdf:AssetsPath</c> legado.
        /// </summary>
        public static IServiceCollection AddLayoutStorage(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<LayoutStorageOptions>(config.GetSection("LayoutStorage"));
            services.PostConfigure<LayoutStorageOptions>(opt =>
            {
                if (!Path.IsPathRooted(opt.BasePath))
                    opt.BasePath = Path.Combine(AppContext.BaseDirectory, opt.BasePath);
            });

            services.AddScoped<IMarcadorExtractor, DocxMarcadorExtractor>();
            services.AddScoped<IDocxLayoutStorage, DocxLayoutStorage>();
            return services;
        }

        /// <summary>
        /// Firmantes: .pfx en disco y contraseña cifrada con Data Protection. Sección
        /// "FirmanteStorage"; rutas relativas se resuelven contra <c>AppContext.BaseDirectory</c>.
        /// Las llaves de Data Protection se persisten en disco: sin ellas (o sin ese volumen en
        /// Docker) las contraseñas guardadas dejan de poder descifrarse.
        /// </summary>
        public static IServiceCollection AddFirmanteStorage(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<FirmanteStorageOptions>(config.GetSection("FirmanteStorage"));
            services.PostConfigure<FirmanteStorageOptions>(opt =>
            {
                opt.PfxPath = ResolverRuta(opt.PfxPath);
                opt.LlavesProteccionPath = ResolverRuta(opt.LlavesProteccionPath);
            });

            var storage = config.GetSection("FirmanteStorage").Get<FirmanteStorageOptions>() ?? new FirmanteStorageOptions();
            services.AddDataProtection()
                .SetApplicationName("CartaNoAdeudoApi")
                .PersistKeysToFileSystem(new DirectoryInfo(ResolverRuta(storage.LlavesProteccionPath)));

            services.AddScoped<IPfxStorage, PfxStorage>();
            services.AddScoped<IFirmanteService, FirmanteService>();
            services.AddScoped<IFirmaCartaService, FirmaCartaService>();
            return services;
        }

        private static string ResolverRuta(string ruta) =>
            Path.IsPathRooted(ruta) ? ruta : Path.Combine(AppContext.BaseDirectory, ruta);

        /// <summary>
        /// Registra los IValidator&lt;T&gt; de FluentValidation (Core/Utils/Validaciones) para
        /// poder inyectarlos donde se necesiten. No valida automáticamente los
        /// requests entrantes: hay que llamar validator.ValidateAsync(...) donde
        /// corresponda (controller o service).
        /// </summary>
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CartaValidator>();
            return services;
        }

        /// <summary>
        /// PDF de la carta (RF-002/RF-003): combina el layout .docx activo con los datos de la
        /// solicitud (<see cref="DocxMarcadorReplacer"/>) y convierte el resultado con
        /// LibreOffice headless (<see cref="LibreOfficePdfConverter"/>, ver "Pdf:LibreOfficePath"
        /// y el paquete libreoffice-writer del Dockerfile). Sección "Pdf" de appsettings.
        /// </summary>
        public static IServiceCollection AddCartaDocumento(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<PdfOptions>(config.GetSection("Pdf"));

            services.AddScoped<IDocxMarcadorReplacer, DocxMarcadorReplacer>();
            services.AddScoped<IPdfConverter, LibreOfficePdfConverter>();
            services.AddScoped<ICartaDocumentoService, CartaDocumentoService>();
            return services;
        }

        /// <summary>
        /// Correo de notificación al solicitante cuando su carta queda Firmada (ver
        /// <c>CartaService.GenerarDocumentoYCorreoAsync</c>). Sección "Email" de appsettings;
        /// las credenciales reales van por variables de entorno, nunca en el appsettings
        /// versionado (mismo criterio que "Siga").
        /// </summary>
        public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<EmailOptions>(config.GetSection("Email"));
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
        {
            var origins = config.GetSection("Cors:Origins").Get<string[]>() ?? [];

            services.AddCors(options => options.AddPolicy("CartaNoAdeudoPolicy", policy =>
                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()));

            return services;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Integración con SIGA
        //
        //  Estos métodos están listos pero NO se llaman desde Program.cs (la
        //  llamada está comentada). Actívalos cuando tengas un AppKey real
        //  emitido por un administrador de SIGA. Ver docs/INTEGRACION_SIGA.md.
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Valida localmente los JWT RS256 que emite SIGA (firma + issuer + audience
        /// + lifetime) usando las llaves públicas de "Siga:SigningKeys". No hace
        /// ninguna llamada de red para validar tokens.
        /// </summary>
        public static IServiceCollection AddSigaAuth(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SigaOptions>(config.GetSection("Siga"));
            var sigaOptions = config.GetSection("Siga").Get<SigaOptions>()
                ?? throw new InvalidOperationException("Falta la sección 'Siga' en la configuración.");

            var signingKeys = BuildSigningKeysFromConfig(sigaOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.MapInboundClaims = false; // conserva los nombres de claim tal como los emite SIGA (sub, appId, grupo, role...)

                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = sigaOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = sigaOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = signingKeys,
                        ClockSkew = TimeSpan.FromSeconds(60) // margen ajustado (default 5 min); requiere reloj sincronizado (NTP) entre SIGA y esta API
                    };

                    opt.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>()
                                .LogWarning("JWT rechazado: {Motivo}", ctx.Exception.GetType().Name);
                            return Task.CompletedTask;
                        },
                        OnChallenge = ctx =>
                        {
                            ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>()
                                .LogWarning("JWT challenge (401): {Error}", ctx.Error ?? "sin token o pipeline no ejecutó AuthN");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(opts =>
                opts.AddPolicy("SigaApp", p => p.AddRequirements(new SigaAppRequirement())));
            services.AddSingleton<IAuthorizationHandler, SigaAppHandler>();

            return services;
        }

        /// <summary>Cliente HTTP hacia SIGA para el proxy de login / refresh (ver AuthController).</summary>
        public static IServiceCollection AddSigaAuthProxy(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SigaOptions>(config.GetSection("Siga"));

            services.AddHttpClient<SigaAuthClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<SigaOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            return services;
        }

        private static IReadOnlyList<SecurityKey> BuildSigningKeysFromConfig(SigaOptions sigaOptions)
        {
            if (sigaOptions.SigningKeys is not { Count: > 0 })
                throw new InvalidOperationException(
                    "No hay llaves en 'Siga:SigningKeys'. Se necesita al menos una llave pública de SIGA " +
                    "cargada localmente para validar JWT.");

            return sigaOptions.SigningKeys
                .Select(key =>
                {
                    var rsa = RSA.Create();
                    rsa.ImportParameters(new RSAParameters
                    {
                        Modulus = Base64UrlEncoder.DecodeBytes(key.Modulus),
                        Exponent = Base64UrlEncoder.DecodeBytes(key.Exponent)
                    });

                    return (SecurityKey)new RsaSecurityKey(rsa) { KeyId = key.Kid };
                })
                .ToList();
        }
    }
}
