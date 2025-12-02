using MongoDB.Bson;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;
using RpWeave.Server.Data.Repositories;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction;
using Serilog;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.Storage;

[ScopedService]
public class StorageModule(
    IChapterEntityRepository chapterEntityRepository,
    VectorDbClient vectorDbClient,
    OllamaEmbedClient embedClient)
{
    // just return collection name for now
    public async Task<string> ProcessAsync(StorageRequest request)
    {
        // create vector collection
        var collectionNamePrefix = request.Name.Replace(" ", "").ToLower();
        var collectionName = $"{Path.GetFileNameWithoutExtension(collectionNamePrefix)}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        await vectorDbClient.CreateCollectionAsync(collectionName);
        Log.Information("Vector collection created with name: {CollectionName}", collectionName);
        
        // store chapters in db
        var chapterGroups = request.Chunks.GroupBy(c => c.Level1Heading);
        foreach (var chapterGroup in chapterGroups)
        {
            var chapterEntity = new ChapterEntity()
            {
                CampaignId = ObjectId.Parse(request.CampaignId),
                Text = $"{chapterGroup.Key}\n\n"
            };
            
            foreach (var chunk in chapterGroup)
            {
                // create entity in db
                PopulateChapterEntityText(chunk, chapterEntity);

                // create vector
                await CreateVectorEntryAsync(chunk, collectionName, chapterEntity);
                Log.Information("Vector entry created for chunk with path: {Path}", $"{chunk.Level1Heading} > {chunk.Level2Heading} > {chunk.Level3Heading} > {chunk.Level4Heading} > {chunk.Level5Heading}");
            }
            
            await chapterEntityRepository.AddAsync(chapterEntity);
            Log.Information("Chapter entity created for chapter {Chapter} of campaign {CampaignId}", chapterGroup.Key, request.CampaignId);
        }

        return collectionName;
    }

    private async Task CreateVectorEntryAsync(TextChunk chunk, string collectionName, ChapterEntity chapterEntity)
    {
        var headings = new List<string?> {chunk.Level1Heading, chunk.Level2Heading, chunk.Level3Heading, chunk.Level4Heading, chunk.Level5Heading}
            .Where(x => !string.IsNullOrWhiteSpace(x));
        var vectorContent = $"Chapter map: {string.Join(" > ", headings)}\n" +
                            $"Content: {chunk.Content}";
        var vector = await embedClient.GenerateEmbeddingsAsync(vectorContent);
        
        await vectorDbClient.UpsertAsync(
            new VectorDbUpsertRequest(collectionName, 
                vector, 
                chapterEntity.Id.ToString(),
                vectorContent));
    }

    private static void PopulateChapterEntityText(TextChunk chunk, ChapterEntity chapterEntity)
    {
        var chunkText = "";
                
        if(!string.IsNullOrWhiteSpace(chunk.Level2Heading))
            chunkText += chunk.Level2Heading;

        if (!string.IsNullOrWhiteSpace(chunk.Level3Heading))
            chunkText += $" > {chunk.Level3Heading}";

        chunkText += "\n";
        chunkText += chunk.Content;
        chunkText += "\n\n";
                
        chapterEntity.Text += chunkText;
    }
}