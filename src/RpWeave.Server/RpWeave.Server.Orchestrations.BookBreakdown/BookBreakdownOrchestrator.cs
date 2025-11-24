using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;
using RpWeave.Server.Orchestrations.BookBreakdown.Pdf;
using RpWeave.Server.Orchestrations.BookBreakdown.Prompts;
using RpWeave.Server.Orchestrations.BookBreakdown.Tools;

namespace RpWeave.Server.Orchestrations.BookBreakdown;

public record BookBreakdownOrchestrationRequest(
    string FilePath,
    int ChapterFontSize,
    int SubChapterFontSize,
    int HeaderFontSize,
    bool IgnoreFooter);

[ScopedService]
public class BookBreakdownOrchestrator(
    OllamaEmbedClient embedClient,
    VectorDbClient vectorDbClient)
{
    public async Task<string> ProcessBookBreakdown(BookBreakdownOrchestrationRequest request)
    {
        var pdfChunker = new PdfChunkModule();
        var pdfChunkRequest = new PdfChunkRequest(request.FilePath, request.ChapterFontSize, request.SubChapterFontSize, request.HeaderFontSize, request.IgnoreFooter);
        var chunks = pdfChunker.ChunkPdf(pdfChunkRequest);

        foreach (var chunk in chunks)
        {
            chunk.Content = chunk.Content.Insert(0, $"{chunk.Chapter} > {chunk.Subchapter} > {chunk.Header}\n");
        }
        
        var collectionName = $"{Path.GetFileNameWithoutExtension(request.FilePath)}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        await vectorDbClient.CreateCollectionAsync(collectionName);
        
        foreach (var chunk in chunks)
        {
            var vector = await embedClient.GenerateEmbeddingsAsync(chunk.Content);
        
            await vectorDbClient.UpsertAsync(
                new VectorDbUpsertRequest(collectionName, vector, $"{chunk.Chapter} > {chunk.Subchapter}",
                    chunk.Content));
        }
        
        return collectionName;
    }
}