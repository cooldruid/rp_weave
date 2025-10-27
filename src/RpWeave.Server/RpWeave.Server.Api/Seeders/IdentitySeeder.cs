using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Data.Entities;

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
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppUserRole>>();

        string[] roles = ["Admin", "GameMaster", "Player"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new AppUserRole() { Name = role });
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}