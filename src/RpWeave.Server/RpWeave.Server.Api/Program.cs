using System.Text.Json;
using AspNetCore.Identity.Mongo;
using MongoDB.Bson;
using RpWeave.Server.Api.Seeders;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

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
builder.Services.AddAttributedServices();
builder.Services.AddHostedService<IdentitySeeder>();

builder.Services.AddIdentityMongoDbProvider<AppUser, AppUserRole, ObjectId>(identity =>
    {
        
    },
    mongo =>
    {
        mongo.ConnectionString = "mongodb://mongo:27017/rpweave";
    });

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();