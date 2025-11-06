using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;
using RpWeave.Server.Orchestrations.BookBreakdown.Pdf;
using RpWeave.Server.Orchestrations.BookBreakdown.Prompts;
using RpWeave.Server.Orchestrations.BookBreakdown.Tools;

namespace RpWeave.Server.Orchestrations.BookBreakdown;

[ScopedService]
public class BookBreakdownOrchestrator(
    OllamaEmbedClient embedClient,
    OllamaChatClient chatClient,
    VectorDbClient vectorDbClient)
{
    public async Task<List<PdfChunk>> ProcessBookBreakdown(string filePath)
    {
        var runId = Guid.NewGuid().ToString();
        var pdfChunker = new PdfChunkingProcessor();
        var chunks = pdfChunker.ChunkPdf(filePath);
        
        #region hardcoded stuff
        // HARDCODED STUFF TO MOVE ON
        
        var chunk0 = chunks.First(x => x.Order == 0);
        var chunk1 = chunks.First(x => x.Order == 1);
        var chunk2 = chunks.First(x => x.Order == 2);

        chunk0.Chapter = "Introduction";
        chunk1.Chapter = "Introduction";

        chunk1.Content += " " + chunk2.Content;

        chunks.Remove(chunk2);
        
        //
        #endregion

        var collectionName = Guid.NewGuid().ToString();
        await vectorDbClient.CreateCollectionAsync(collectionName);

        var vectorsList = new List<(PdfChunk, float[])>();
        foreach (var chunk in chunks)
        {
            var vector = await embedClient.GenerateEmbeddingsAsync(chunk.Content);

            await vectorDbClient.UpsertAsync(
                new VectorDbUpsertRequest(collectionName, vector, $"{chunk.Chapter} > {chunk.Subchapter}",
                    chunk.Content));
        }

        var searchVector = await embedClient.GenerateEmbeddingsAsync("Who is Cailyassa?");

        var response = await vectorDbClient.SearchAsync(new VectorDbSearchRequest(
            collectionName,
            searchVector));

        // foreach (var chunk in chunks)
        // {
        //     var request = new OllamaChatRequest(
        //         BookBreakdownPrompts.SystemPrompt,
        //         BookBreakdownPrompts.UserPrompt(runId, chunk.Content),
        //         [JsonTools.ListAllJsonFilesAsync, JsonTools.ReadJsonFromFileAsync, JsonTools.WriteJsonToFileAsync]);
        //     
        //     await chatClient.SendAsync(request);
        // }
        
        
        
        return chunks;
    }
}