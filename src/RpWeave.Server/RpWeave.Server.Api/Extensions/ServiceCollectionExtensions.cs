using System.Text;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Settings;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRpwIdentityProvider(this IServiceCollection services)
    {
        services.AddIdentityMongoDbProvider<AppUser, AppUserRole, ObjectId>(
            setupDatabaseAction: mongo =>
            {
                mongo.ConnectionString = "mongodb://mongo:27017/rpweave";
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
            options.AddPolicy(UserRoleConstants.GameMaster, policy =>
                policy.RequireRole(UserRoleConstants.Admin, UserRoleConstants.GameMaster));
            options.AddPolicy(UserRoleConstants.Admin, policy =>
                policy.RequireRole(UserRoleConstants.Admin));
    
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(UserRoleConstants.Admin, UserRoleConstants.GameMaster, UserRoleConstants.Player)
                .Build();
        });

        return services;
    }
}