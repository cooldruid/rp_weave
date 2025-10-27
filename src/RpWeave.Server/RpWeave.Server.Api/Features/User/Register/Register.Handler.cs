using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Api.Features.User.Register;

[ScopedService]
public class RegisterHandler(UserManager<AppUser> userManager)
{
    public async Task<Result> HandleAsync(RegisterRequest request)
    {
        var user = new AppUser
        {
            UserName = request.Username
        };
        
        var creationResult = await userManager.CreateAsync(user, request.Password);
        
        if(!creationResult.Succeeded)
            return Result.Failure("test", string.Join("\n", creationResult.Errors.Select(x => x.Description)));
        
        await userManager.AddToRoleAsync(user, "Admin");
        
        return Result.Success();
    }
}