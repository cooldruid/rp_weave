using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Api.Features.Settings.CreateUser;

[ScopedService]
public class CreateUserHandler(UserManager<AppUser> userManager)
{
    public async Task<Result> HandleAsync(CreateUserRequest request)
    {
        if (!UserRoleConstants.AllRoles.Contains(request.Role))
            return Result.Failure(ErrorCodes.UserInput, "Role does not exist.");
        
        var user = new AppUser
        {
            UserName = request.Username
        };
        
        var creationResult = await userManager.CreateAsync(user, request.Password);
        
        if(!creationResult.Succeeded)
            return Result.Failure(ErrorCodes.UserInput, string.Join("\n", creationResult.Errors.Select(x => x.Description)));
        
        await userManager.AddToRoleAsync(user, request.Role);
        
        return Result.Success();
    }
}