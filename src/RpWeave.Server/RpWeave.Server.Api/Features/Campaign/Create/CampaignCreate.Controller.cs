using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.Campaign.Create;

[ApiController]
public class CampaignCreateController(CampaignCreateHandler handler) : ControllerBase
{
    [HttpPost("api/campaign/create")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateAsync([FromForm] CampaignCreateRequest request)
    {
        var result = await handler.HandleAsync(request);

        return result.ToActionResult();
    }
}