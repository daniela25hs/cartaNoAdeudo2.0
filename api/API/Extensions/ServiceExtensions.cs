using System.Security.Cryptography;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>Cliente HTTP hacia "wsLicAlcoholes" (firma FEA vía Contraloría). Ver <see cref="IFirmaContraloriaService"/>.</summary>
        public static IServiceCollection AddFirmaContraloria(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<FirmaContraloriaOptions>(config.GetSection("FirmaContraloria"));
            services.AddHttpClient<IFirmaContraloriaService, FirmaContraloriaClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<FirmaContraloriaOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSegundos);
            });
            return services;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  PDF de la carta (RF-002/RF-003)
        //
        //  TODO: reimplementar tras la reescritura de entidades. Los tipos que
        //  registraban este método (PdfOptions / IPdfGeneratorService /
        //  CartaPdfGenerator) ya no existen en la solución. Al volver a crearlos,
        //  restaurar este método y su llamada en Program.cs.
        //
        //  public static IServiceCollection AddPdfGeneration(this IServiceCollection services, IConfiguration config)
        //  {
        //      services.Configure<PdfOptions>(config.GetSection("Pdf"));
        //      services.PostConfigure<PdfOptions>(opt =>
        //      {
        //          if (!Path.IsPathRooted(opt.AssetsPath))
        //              opt.AssetsPath = Path.Combine(AppContext.BaseDirectory, opt.AssetsPath);
        //      });
        //      services.AddScoped<IPdfGeneratorService, CartaPdfGenerator>();
        //      return services;
        //  }
        // ─────────────────────────────────────────────────────────────────────

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
