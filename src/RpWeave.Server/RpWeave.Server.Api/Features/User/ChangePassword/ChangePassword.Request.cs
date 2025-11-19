namespace RpWeave.Server.Api.Features.User.ChangePassword;

public record ChangePasswordRequest(string OldPassword, string NewPassword);