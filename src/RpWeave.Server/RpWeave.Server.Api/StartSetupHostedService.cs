using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Data.Entities;
using Serilog;

namespace RpWeave.Server.Api;

public class StartSetupHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        await SeedUserRolesAsync(scope);
        await CreateAdminIfNoUsersAsync(scope);
    }

    private async Task SeedUserRolesAsync(IServiceScope scope)
    {
        Log.Information("Seeding user roles...");
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppUserRole>>();

        string[] roles = [
            UserRoleConstants.Admin,
            UserRoleConstants.User
        ];

        foreach (var role in roles)
        {
            Log.Information("Seeding role {Role}...", role);
            
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new AppUserRole() { Name = role });
                Log.Information("Role {Role} successfully seeded.", role);
            }
            else
            {
                Log.Information("Role {Role} already exists.", role);
            }
        }
        
        Log.Information("User roles seeded.");
    }

    private async Task CreateAdminIfNoUsersAsync(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        
        if (userManager.Users.Any())
        {
            Log.Information("Users already exist, skipping admin creation...");
            return;
        }

        var username = "admin";
        var password = "ChangeMe!123";
        
        await userManager.CreateAsync(new AppUser { UserName = username }, password);
        
        Log.Information("Admin user created. Use credentials:\n{Username}\n{Password}\nMake sure to change your password as soon as possible.", username, password);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}