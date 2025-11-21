using Microsoft.AspNetCore.Identity;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Api.Features.Settings.ListUsers;

[ScopedService]
public class ListUsersHandler(UserManager<AppUser> userManager)
{
    public async Task<ValueResult<ListUsersResponse>> HandleAsync()
    {
        var userItems = new List<ListUsersItem>();

        foreach (var user in userManager.Users)
        {
            var username = user.UserName 
                ?? string.Empty;
            var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() 
                ?? string.Empty;
            var item = new ListUsersItem(username, role);
            userItems.Add(item);
        }
        
        return ValueResult<ListUsersResponse>.Success(new ListUsersResponse(userItems.ToList()));
    }
}