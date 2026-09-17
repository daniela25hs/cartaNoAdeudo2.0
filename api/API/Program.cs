using CartaNoAdeudoApi.API.Extensions;
using CartaNoAdeudoApi.API.Middleware;
using Serilog;

// Logger de arranque: captura fallos que ocurren antes de construir el host.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddEnvironmentVariables(prefix: "CARTA_NO_ADEUDO__");

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers();
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddAppServices();
    builder.Services.AddValidators();
    builder.Services.AddFirmaContraloria(builder.Configuration);
    builder.Services.AddLayoutStorage(builder.Configuration);
    // TODO: restaurar cuando exista el servicio de PDF (ver ServiceExtensions.cs).
    // builder.Services.AddPdfGeneration(builder.Configuration);
    builder.Services.AddCorsPolicy(builder.Configuration);
    builder.Services.AddAuthorization();

    // ── Integración con SIGA ────────────────────────────────────────────────
    // Descomenta cuando tengas un AppKey real de SIGA y las llaves públicas
    // cargadas en "Siga:SigningKeys". Ver docs/INTEGRACION_SIGA.md.
    // Sin esto la API arranca SIN autenticación (todos los endpoints anónimos).
    //
    // builder.Services.AddSigaAuth(builder.Configuration);       // valida el JWT de SIGA
    // builder.Services.AddSigaAuthProxy(builder.Configuration);  // proxy de login/refresh (AuthController)
    // ────────────────────────────────────────────────────────────────────────

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Carta de No Adeudo API", Version = "v1" });
        // Al activar SIGA, agrega aquí la definición de seguridad Bearer para
        // poder mandar el token desde Swagger UI.
    });
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Carta de No Adeudo API v1"));
    }

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.UseCors("CartaNoAdeudoPolicy");

    // app.UseAuthentication(); // ← descomenta junto con AddSigaAuth
    app.UseAuthorization();

    app.MapControllers();

    await app.MigrateAsync();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación no pudo arrancar.");
}
finally
{
    Log.CloseAndFlush();
}
