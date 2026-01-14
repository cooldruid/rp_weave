namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction;

public class TextChunk
{
    public string? Level1Heading { get; set; }
    public string? Level2Heading { get; set; }
    public string? Level3Heading { get; set; }
    public string? Level4Heading { get; set; }
    public string? Level5Heading { get; set; }
    public required int Order { get; set; }
    public required string Content { get; set; }
}