using System.Text.Json;
using RpWeave.Server.Api.Extensions;
using RpWeave.Server.Api.Middleware;
using RpWeave.Server.Api.Seeders;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Extensions;
using RpWeave.Server.Mcp;
using Serilog;
using AssemblyMarker = RpWeave.Server.Data.AssemblyMarker;

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
        typeof(AssemblyMarker).Assembly,
        typeof(RpWeave.Server.Mcp.AssemblyMarker).Assembly,
        typeof(RpWeave.Server.Orchestrations.BookBreakdown.AssemblyMarker).Assembly,
        typeof(RpWeave.Server.Integrations.Ollama.AssemblyMarker).Assembly,
    ]);
builder.Services.AddHostedService<IdentitySeeder>();

builder.Services.AddRpwIdentityProvider()
    .AddRpwAuthentication(builder.Configuration)
    .AddRpwAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOllamaIntegration(builder.Configuration);

var app = builder.Build();

// this is bad bad bad, but will do for now
ServiceProviderInstance.Initialize(app.Services);

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.UseExceptionHandler();

app.Run();