using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Api.Features.User.ChangePassword;

[ScopedService]
public class ChangePasswordHandler(UserManager<AppUser> userManager)
{
    public async Task<Result> HandleAsync(string userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
            return Result.Failure(ErrorCodes.NotFound, "User with given Id not found.");

        var changePasswordResult = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

        if (!changePasswordResult.Succeeded)
            return Result.Failure(ErrorCodes.UserInput,
                string.Join("\n", changePasswordResult.Errors.Select(x => x.Description)));
        
        return Result.Success();
    }
}