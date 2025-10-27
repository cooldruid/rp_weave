using Microsoft.AspNetCore.Mvc;

namespace RpWeave.Server.Api.Features.User.Register;

public class RegisterController(RegisterHandler handler) : ControllerBase
{
    [HttpPost("api/user/register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        try
        {
            var result = await handler.HandleAsync(registerRequest);

            if (!result.IsSuccess)
                return BadRequest($"{result.Error?.Code}: {result.Error?.Message}");

            return Accepted();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500);
        }
    }
}