using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RpWeave.Server.Core.Results;

namespace RpWeave.Server.Core.Startup;

public abstract class BaseEndpoint<TRequest> : IEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    protected async Task<IResult> ExecuteAsync(
        Func<TRequest, Task<IProcessResult>> handler,
        TRequest request)
    {
        try
        {
            var result = await handler(request);

            if (result.IsSuccess)
            {
                if (result is ValueResult<object> valueResult)
                    return TypedResults.Ok(valueResult.Value);

                return TypedResults.Accepted("");
            }
            else
            {
                return TypedResults.BadRequest($"{result.Error?.Code}: {result.Error?.Message}");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.InternalServerError("Unexpected error occurred.");
        }
    }
}

public abstract class BaseEndpoint : IEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    protected async Task<IResult> ExecuteAsync(
        Func<Task<IProcessResult>> handler)
    {
        try
        {
            var result = await handler();

            if (result.IsSuccess)
            {
                if (result is ValueResult<object> valueResult)
                    return TypedResults.Ok(valueResult.Value);

                return TypedResults.Accepted("");
            }
            else
            {
                return TypedResults.BadRequest($"{result.Error?.Code}: {result.Error?.Message}");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.InternalServerError("Unexpected error occurred.");
        }
    }
}

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}