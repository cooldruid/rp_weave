using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;
using RpWeave.Server.Data.Repositories;
using RpWeave.Server.Orchestrations.BookBreakdown;

namespace RpWeave.Server.Api.Features.Campaign.Create;

[ScopedService]
public class CampaignCreateHandler(
    BookBreakdownOrchestrator bookBreakdownOrchestrator,
    ICampaignEntityRepository campaignRepository)
{
    private const string BaseFilePath = "/srv/rpweave/uploads";
    
    public async Task<Result> HandleAsync(CampaignCreateRequest request)
    {
        // Validation
        var validator = new CampaignCreateValidator();
        var validationResult = await validator.ValidateAsync(request);
        
        if(!validationResult.IsValid)
            return Result.Failure(ErrorCodes.UserInput, string.Join("\n", validationResult.Errors));
        
        // Create entity
        var entity = new CampaignEntity
        {
            Name = request.Data.Name,
            Description = request.Data.Description
        };
        
        // Store file
        if (request.Pdf != null)
        {
            var filePath = Path.Combine(BaseFilePath, $"{entity.Id}.pdf");
            await using Stream fileStream = new FileStream(filePath, FileMode.Create);
            await request.Pdf.CopyToAsync(fileStream);
            entity.PdfPath = filePath;

            if (request.Data.CreateEmbeddings)
            {
                var collectionName = await bookBreakdownOrchestrator.ProcessBookBreakdown(filePath);
                entity.VectorCollectionName = collectionName;
            }
        }
        
        await campaignRepository.AddAsync(entity);
        
        return Result.Success();
    }
}