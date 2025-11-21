namespace RpWeave.Server.Api.Features.Settings.ListUsers;

public record ListUsersResponse(List<ListUsersItem> Users);

public record ListUsersItem(string Username, string Role);