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
        var chapterGroups = request.Chunks.GroupBy(c => c.Chapter);
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
                Log.Information("Vector entry created for chunk with path: {Path}", $"{chunk.Chapter} > {chunk.Subchapter} > {chunk.Header}");
            }
            
            await chapterEntityRepository.AddAsync(chapterEntity);
            Log.Information("Chapter entity created for chapter {Chapter} of campaign {CampaignId}", chapterGroup.Key, request.CampaignId);
        }

        return collectionName;
    }

    private async Task CreateVectorEntryAsync(TextChunk chunk, string collectionName, ChapterEntity chapterEntity)
    {
        var vectorContent = chunk.Content.Insert(0, $"{chunk.Chapter} > {chunk.Subchapter} > {chunk.Header}\n");
        var vector = await embedClient.GenerateEmbeddingsAsync(vectorContent);
        
        await vectorDbClient.UpsertAsync(
            new VectorDbUpsertRequest(collectionName, 
                vector, 
                chapterEntity.Id.ToString(),
                chunk.Content));
    }

    private static void PopulateChapterEntityText(TextChunk chunk, ChapterEntity chapterEntity)
    {
        var chunkText = "";
                
        if(!string.IsNullOrWhiteSpace(chunk.Subchapter))
            chunkText += chunk.Subchapter;

        if (!string.IsNullOrWhiteSpace(chunk.Header))
            chunkText += $" > {chunk.Header}";

        chunkText += "\n";
        chunkText += chunk.Content;
        chunkText += "\n\n";
                
        chapterEntity.Text += chunkText;
    }
}