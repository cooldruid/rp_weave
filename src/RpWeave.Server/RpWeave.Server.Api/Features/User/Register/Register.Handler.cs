using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Api.Constants;
using RpWeave.Server.Api.Settings;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Api.Features.User.Register;

[ScopedService]
public class RegisterHandler(
    UserManager<AppUser> userManager,
    SystemSettings systemSettings)
{
    public async Task<Result> HandleAsync(RegisterRequest request)
    {
        if (!systemSettings.UsersCanRegister)
            return Result.Failure(ErrorCodes.Unauthorized, "Server does not currently allow user registrations.");
        
        var user = new AppUser
        {
            UserName = request.Username
        };
        
        var creationResult = await userManager.CreateAsync(user, request.Password);
        
        if(!creationResult.Succeeded)
            return Result.Failure(ErrorCodes.UserInput, string.Join("\n", creationResult.Errors.Select(x => x.Description)));
        
        await userManager.AddToRoleAsync(user, UserRoleConstants.User);
        
        return Result.Success();
    }
}