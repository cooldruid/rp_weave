namespace RpWeave.Server.Api.Constants;

public static class UserRoleConstants
{
    public const string Admin = "ADMIN";
    public const string User = "USER";
    public static string[] AllRoles => [Admin, User];
}