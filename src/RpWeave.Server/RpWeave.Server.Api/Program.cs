using System.Text.Json;
using RpWeave.Server.Api;
using RpWeave.Server.Api.Extensions;
using RpWeave.Server.Api.Middleware;
using RpWeave.Server.Core.Startup;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerDocument();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAttributedServices(
    [
        typeof(Program).Assembly,
        typeof(RpWeave.Server.Data.AssemblyMarker).Assembly,
        typeof(RpWeave.Server.Orchestrations.AssemblyMarker).Assembly,
        typeof(RpWeave.Server.Integrations.Ollama.AssemblyMarker).Assembly,
        typeof(RpWeave.Server.Integrations.Qdrant.AssemblyMarker).Assembly
    ]);
builder.Services.AddHostedService<StartSetupHostedService>();

// do not do this at home
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);

builder.Services.AddRpwIdentityProvider()
    .AddRpwAuthentication(builder.Configuration)
    .AddRpwAuthorization()
    .AddMongoSettings()
    .AddQdrantSettings()
    .AddOllamaIntegration()
    .AddSystemSettings();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseCors(options => 
    options.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        // also temporary, should come from env
        .WithOrigins("http://localhost:4200", "http://rpweaveui:80", "http://localhost:22344"));

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.UseExceptionHandler();

app.Run();