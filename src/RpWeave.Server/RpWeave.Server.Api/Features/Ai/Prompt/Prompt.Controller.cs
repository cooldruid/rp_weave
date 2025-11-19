using Microsoft.AspNetCore.Mvc;

namespace RpWeave.Server.Api.Features.Ai.Prompt;

[ApiController]
public class PromptController(PromptHandler handler) : ControllerBase
{
    [HttpPost("api/ai/prompt")]
    public async Task<IActionResult> Prompt(PromptRequest request)
    {
        var response = await handler.HandleAsync(request);

        return Ok(response);
    }
}