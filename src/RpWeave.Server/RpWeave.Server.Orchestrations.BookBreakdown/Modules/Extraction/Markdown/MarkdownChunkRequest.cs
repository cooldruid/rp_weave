namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction.Markdown;

public record MarkdownChunkRequest(
    string FilePath,
    int ChapterLevel,
    int SubChapterLevel,
    int HeaderLevel);