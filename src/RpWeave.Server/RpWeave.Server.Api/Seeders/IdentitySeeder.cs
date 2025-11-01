using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Data.Entities;
using Serilog;

namespace RpWeave.Server.Api.Seeders;

public class IdentitySeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public IdentitySeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("Seeding user roles...");
        
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppUserRole>>();

        string[] roles = [
            UserRoleConstants.Admin,
            UserRoleConstants.GameMaster,
            UserRoleConstants.Player
        ];

        foreach (var role in roles)
        {
            Log.Information($"Seeding role {role}...");
            
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new AppUserRole() { Name = role });
                Log.Information($"Role {role} successfully seeded.");
            }
            else
            {
                Log.Information($"Role {role} already exists.");
            }
        }
        
        Log.Information("User roles seeded.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}