using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.Campaign.Delete;

[ApiController]
public class CampaignDeleteController(CampaignDeleteHandler handler) : ControllerBase
{
    [HttpDelete("api/campaign/{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var result = await handler.HandleAsync(id);

        return result.ToActionResult();
    }
}