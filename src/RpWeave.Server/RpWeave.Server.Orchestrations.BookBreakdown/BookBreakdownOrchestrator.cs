using RpWeave.Server.Core.Startup;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.EntityExtraction;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.RelationshipExtraction;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Storage;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.TextExtraction;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.TextExtraction.Markdown;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.TextExtraction.Pdf;
using Serilog;

namespace RpWeave.Server.Orchestrations.BookBreakdown;

public record BookBreakdownOrchestrationRequest(
    string CampaignName,
    string CampaignId,
    string FilePath,
    int ChapterFontSize,
    int SubChapterFontSize,
    int HeaderFontSize,
    bool IgnoreFooter);

[ScopedService]
public class BookBreakdownOrchestrator(
    MarkdownChunkModule markdownChunkModule,
    StorageModule storageModule,
    EntityExtractionModule entityExtractionModule,
    RelationshipExtractionModule relationshipExtractionModule)
{
    public async Task<string> ProcessBookBreakdown(BookBreakdownOrchestrationRequest request)
    {
        var chunks = ExtractTextChunks(request);

        var collectionName = await StoreTextDataAsync(request, chunks);

        // var extractionResponse = await ExtractEntitiesAsync(chunks);
        //
        // Log.Information(
        //     $"###########\nENTITIES\n\n{string.Join("\n", extractionResponse.Elements.Select(x => $"{x.Name} - {x.Type}"))}\n\n\n" +
        //     $"{string.Join(",", extractionResponse.Elements.Select(x => $"{x.Name} - {x.Type}"))}\n\n\n###########");
        //
        // var relationshipResponse = await  ExtractRelationshipsAsync(collectionName, extractionResponse);
        //
        // Log.Information(
        //     $"###########\nRELATIONSHIPS\n\n{string.Join("\n", relationshipResponse.Elements.Select(x => $"{x.SourceEntity} -> {x.TargetEntity} : {x.Relationship}"))}\n\n\n" +
        //     $"{string.Join(",", relationshipResponse.Elements.Select(x => $"{x.SourceEntity} -> {x.TargetEntity} : {x.Relationship}"))}\n\n\n###########");

        return collectionName;
    }

    private async Task<RelationshipExtractionResponse> ExtractRelationshipsAsync(string collectionName, EntityExtractionResponse entityExtractionResponse)
    {
        var entities = entityExtractionResponse.Elements.Select(x =>
            new RelationshipExtractionEntity(x.Name, x.Type)).ToList();
        var request = new RelationshipExtractionRequest(collectionName, entities);
        return await relationshipExtractionModule.ProcessAsync(request);
    }

    private async Task<EntityExtractionResponse> ExtractEntitiesAsync(List<TextChunk> chunks)
    {
        var chunkContents = chunks.Select(x => x.Content).ToList();
        var extractionResponse = await entityExtractionModule.ProcessAsync(chunkContents);
        return extractionResponse;
    }

    private async Task<string> StoreTextDataAsync(BookBreakdownOrchestrationRequest request, List<TextChunk> chunks)
    {
        var storageRequest = new StorageRequest
        {
            Name = request.CampaignName,
            CampaignId = request.CampaignId,
            Chunks = chunks
        };
        var collectionName = await storageModule.ProcessAsync(storageRequest);

        return collectionName;
    }

    private List<TextChunk> ExtractTextChunks(BookBreakdownOrchestrationRequest request)
    {
        var chunks = new List<TextChunk>();

        if (Path.GetExtension(request.FilePath) == ".pdf")
        {
            var pdfChunker = new PdfChunkModule();
            var pdfChunkRequest = new PdfChunkRequest(request.FilePath, request.ChapterFontSize,
                request.SubChapterFontSize, request.HeaderFontSize, request.IgnoreFooter);
            chunks = pdfChunker.ChunkPdf(pdfChunkRequest);
        }
        else if (Path.GetExtension(request.FilePath) == ".md")
        {
            var mdChunkRequest = new MarkdownChunkRequest(request.FilePath);
            chunks = markdownChunkModule.Process(mdChunkRequest);
        }

        return chunks;
    }
}