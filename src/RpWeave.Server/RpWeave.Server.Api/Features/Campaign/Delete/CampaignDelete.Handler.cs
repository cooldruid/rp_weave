using RpWeave.Server.Core.Results;
using RpWeave.Server.Data.Repositories;

namespace RpWeave.Server.Api.Features.Campaign.Delete;

public class CampaignDeleteHandler(ICampaignEntityRepository campaignRepository)
{
    public async Task<Result> HandleAsync(string id)
    {
        var isSuccess = await campaignRepository.DeleteAsync(id);

        if (!isSuccess)
            return Result.Failure(ErrorCodes.SystemError, "Failed to delete campaign");
        
        return Result.Success();
    }
}