using RpWeave.Server.Core.Startup;
using RpWeave.Server.Orchestrations.BookBreakdown.Pdf;

namespace RpWeave.Server.Orchestrations.BookBreakdown;

[ScopedService]
public class BookBreakdownOrchestrator
{
    public async Task<List<PdfChunk>> ProcessBookBreakdown(string filePath)
    {
        var pdfChunker = new PdfChunkingProcessor();
        var chunks = pdfChunker.ChunkPdf(filePath);
        
        // HARDCODED STUFF TO MOVE ON
        
        var chunk0 = chunks.First(x => x.Order == 0);
        var chunk1 = chunks.First(x => x.Order == 1);
        var chunk2 = chunks.First(x => x.Order == 2);

        chunk0.Chapter = "Introduction";
        chunk1.Chapter = "Introduction";

        chunk1.Content += " " + chunk2.Content;

        chunks.Remove(chunk2);
        
        //
        
        return chunks;
    }
}