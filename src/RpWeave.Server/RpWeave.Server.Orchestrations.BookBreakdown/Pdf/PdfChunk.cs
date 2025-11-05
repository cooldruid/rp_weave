namespace RpWeave.Server.Orchestrations.BookBreakdown.Pdf;

public class PdfChunk
{
    public required string Chapter { get; set; }
    public required string Subchapter { get; set; }
    public required int Order { get; set; }
    public required string Content { get; set; }
}