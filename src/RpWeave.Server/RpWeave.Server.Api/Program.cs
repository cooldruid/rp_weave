using System.Text;
using System.Text.Json;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Extensions;
using RpWeave.Server.Api.Seeders;
using RpWeave.Server.Api.Settings;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseExceptionHandler();

app.Run();