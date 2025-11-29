namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction;

public class TextChunk
{
    public required string Chapter { get; set; }
    public required string Subchapter { get; set; }
    public required string Header { get; set; }
    public required int Order { get; set; }
    public required string Content { get; set; }
}