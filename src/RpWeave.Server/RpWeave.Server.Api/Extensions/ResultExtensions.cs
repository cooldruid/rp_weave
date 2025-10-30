using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Core.Results;

namespace RpWeave.Server.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this IProcessResult result)
    {
        if (result.IsFailure)
            return result.ToStatusCodeFromErrorCode();

        if (result is ValueResult<object> valueResult)
            return new OkObjectResult(valueResult.Value);
        
        return new AcceptedResult();
    }

    private static IActionResult ToStatusCodeFromErrorCode(this IProcessResult result) =>
        result.Error?.Code switch
        {
            ErrorCodes.UserInput => new BadRequestObjectResult(result.Error.Message),
            ErrorCodes.Unauthorized => new UnauthorizedObjectResult(result.Error.Message),
            ErrorCodes.NotFound => new NotFoundObjectResult(result.Error.Message),
            _ => new StatusCodeResult(500)
        };
}