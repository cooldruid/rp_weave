using System.Text;
using System.Text.Json;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Extensions;
using RpWeave.Server.Api.Middleware;
using RpWeave.Server.Api.Seeders;
using RpWeave.Server.Api.Settings;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;
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
        typeof(RpWeave.Server.Data.AssemblyMarker).Assembly
    ]);
builder.Services.AddHostedService<IdentitySeeder>();

builder.Services.AddRpwIdentityProvider()
    .AddRpwAuthentication(builder.Configuration)
    .AddRpwAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// builder.Services.AddMcpServer()
//     .WithStdioServerTransport()
//     .WithToolsFromAssembly();

var app = builder.Build();

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