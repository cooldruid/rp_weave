using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Classification;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction.Markdown;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction.Pdf;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Storage;

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
    StorageModule storageModule)
{
    public async Task<string> ProcessBookBreakdown(BookBreakdownOrchestrationRequest request)
    {
        var chunks = new List<TextChunk>();

        if (Path.GetExtension(request.FilePath) == ".pdf")
        {
            var pdfChunker = new PdfChunkModule();
            var pdfChunkRequest = new PdfChunkRequest(request.FilePath, request.ChapterFontSize, request.SubChapterFontSize, request.HeaderFontSize, request.IgnoreFooter);
            chunks = pdfChunker.ChunkPdf(pdfChunkRequest);
        }
        else if (Path.GetExtension(request.FilePath) == ".md")
        {
            var mdChunker = new MarkdownChunkModule();
            // HARDCODED FOR TEST
            var mdChunkRequest = new MarkdownChunkRequest(request.FilePath, 1, 2, 3);
            chunks = mdChunker.Process(mdChunkRequest);
        }

        var storageRequest = new StorageRequest()
        {
            Name = request.CampaignName,
            CampaignId = request.CampaignId,
            Chunks = chunks
        };
        var collectionName = await storageModule.ProcessAsync(storageRequest);

        // var index = 0;
        // do
        // {
        //     var classificationInput = "";
        //
        //     for (int i = 0; i < 5; i++)
        //     {
        //         if(index < chunks.Count)
        //             classificationInput += chunks[index].Content + "\n\n";
        //
        //         index++;
        //     }
        //     
        //     classifications += await classificationModule.ClassifyAsync(classificationInput);
        // }
        // while(index < chunks.Count);
        
        return collectionName;
    }
}