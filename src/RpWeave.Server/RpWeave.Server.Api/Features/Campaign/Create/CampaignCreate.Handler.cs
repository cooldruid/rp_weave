using System.Text.Json;
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
        var data = JsonSerializer.Deserialize<CampaignCreateData>(request.Data,
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        var validator = new CampaignCreateValidator(data);
        var validationResult = await validator.ValidateAsync(request);
        
        if(!validationResult.IsValid)
            return Result.Failure(ErrorCodes.UserInput, string.Join("\n", validationResult.Errors));
        
        // Create entity
        var entity = new CampaignEntity
        {
            Name = data.Name,
            Description = data.Description
        };
        
        // Store file
        if (request.File != null)
        {
            var extension = Path.GetExtension(request.File.FileName);
            var filePath = Path.Combine(BaseFilePath, $"{entity.Id}{extension}");
            await using Stream fileStream = new FileStream(filePath, FileMode.Create);
            await request.File.CopyToAsync(fileStream);
            entity.PdfPath = filePath;

            if (data.CreateEmbeddings)
            {
                var orchestrationRequest = new BookBreakdownOrchestrationRequest(
                    entity.Name,
                    entity.Id.ToString(),
                    filePath, 
                    data.ChapterFontSize!.Value, 
                    data.SubChapterFontSize!.Value, 
                    data.HeaderFontSize!.Value, 
                    data.IgnoreFooter!.Value);
                var collectionName = await bookBreakdownOrchestrator.ProcessBookBreakdown(orchestrationRequest);
                entity.VectorCollectionName = collectionName;
            }
        }
        
        await campaignRepository.AddAsync(entity);
        
        return Result.Success();
    }
}