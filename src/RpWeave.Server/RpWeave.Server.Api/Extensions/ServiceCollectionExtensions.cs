using System.Text;
using System.Text.Json;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.AI;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using OllamaSharp;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Settings;
using RpWeave.Server.Data;
using RpWeave.Server.Data.Entities;
using RpWeave.Server.Integrations.Ollama;
using RpWeave.Server.Integrations.Qdrant;
using Serilog;

namespace RpWeave.Server.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRpwIdentityProvider(this IServiceCollection services)
    {
        services.AddIdentityMongoDbProvider<AppUser, AppUserRole, ObjectId>(
            setupDatabaseAction: mongo =>
            {
                mongo.ConnectionString = $"mongodb://{Environment.GetEnvironmentVariable("MongoServerName")}:27017/rpweave";
            });

        return services;
    }

    public static IServiceCollection AddRpwAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var authSettings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        ArgumentNullException.ThrowIfNull(authSettings);
        services.AddSingleton(authSettings);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidAudience = authSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.TokenSecret))
                };
            });
        
        return services;
    }

    public static IServiceCollection AddRpwAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(UserRoleConstants.Admin, policy =>
                policy.RequireRole(UserRoleConstants.Admin));
    
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(UserRoleConstants.Admin, UserRoleConstants.User)
                .Build();
        });

        return services;
    }

    public static IServiceCollection AddMongoSettings(this IServiceCollection services)
    {
        services.AddSingleton(new MongoSettings
        {
            ConnectionString = $"mongodb://{Environment.GetEnvironmentVariable("MongoServerName")}:27017"
        });

        return services;
    }

    public static IServiceCollection AddQdrantSettings(this IServiceCollection services)
    {
        services.AddSingleton(new QdrantSettings
        {
            // TODO: should verify instead
            QdrantHost = Environment.GetEnvironmentVariable("QdrantServerName") ?? string.Empty
        });

        return services;
    }
    
    public static IServiceCollection AddOllamaIntegration(this IServiceCollection serviceCollection)
    {
        var ollamaSettings = new OllamaSettings
        {
            Url = $"http://{Environment.GetEnvironmentVariable("OllamaServerName")}:11434",
            ReasoningModel = Environment.GetEnvironmentVariable("OllamaReasoningModelName") ?? string.Empty,
            EmbeddingsModel = Environment.GetEnvironmentVariable("OllamaEmbeddingsModelName") ?? string.Empty
        };
        
        var ollama = new OllamaApiClient(new Uri(ollamaSettings.Url))
        {
            SelectedModel = ollamaSettings.ReasoningModel
        };

        serviceCollection.AddChatClient(ollama).UseFunctionInvocation();
        serviceCollection.AddSingleton(ollamaSettings);

        return serviceCollection;
    }
    
    public static IServiceCollection AddSystemSettings(this IServiceCollection services)
    {
        const string path = "/srv/rpweave/settings/system.json";

        SystemSettings? settings;
        if (File.Exists(path))
        {
            Log.Information("Loading system settings...");
            var content = File.ReadAllText(path);
            settings = JsonSerializer.Deserialize<SystemSettings>(content);
        }
        else
        {
            Log.Information("Creating system settings json...");
            settings = new SystemSettings();
            var json = JsonSerializer.Serialize(settings);
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
            File.WriteAllText(path, json);
        }
        
        ArgumentNullException.ThrowIfNull(settings);
        services.AddSingleton(settings);
        Log.Information("System settings loaded.");
        
        return services;
    }
}