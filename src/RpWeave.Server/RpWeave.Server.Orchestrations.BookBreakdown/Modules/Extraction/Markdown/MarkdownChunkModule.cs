using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using RpWeave.Server.Core.Startup;
using Serilog;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction.Markdown;

[ScopedService]
public class MarkdownChunkModule
{
    public List<TextChunk> Process(MarkdownChunkRequest request)
    {
        var markdownText = File.ReadAllText(request.FilePath);
        var markdown = Markdig.Markdown.Parse(markdownText);
        
        var chapterName = "";
        var subChapterName = "";
        var headerName = "";
        var currentContent = "";
        var order = 0;
        
        var chunks = new List<TextChunk>();

        foreach (var block in markdown)
        {
            if (block is HeadingBlock heading)
            {
                var isChapter = heading.Level == request.ChapterLevel;

                if (isChapter)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Chapter = chapterName.Trim(),
                            Subchapter = subChapterName.Trim(),
                            Header = headerName.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        chapterName = "";
                        subChapterName = "";
                        headerName = "";
                        currentContent = "";
                        order++;
                    }
                    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    chapterName += text;

                    continue;
                }
                
                var isSubchapter = heading.Level == request.SubChapterLevel;
                
                if (isSubchapter)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Chapter = chapterName.Trim(),
                            Subchapter = subChapterName.Trim(),
                            Header = headerName.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        subChapterName = "";
                        headerName = "";
                        currentContent = "";
                        order++;
                    }
    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    subChapterName += text;

                    continue;
                }
                
                var isHeader = heading.Level == request.HeaderLevel;
                
                if (isHeader)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Chapter = chapterName.Trim(),
                            Subchapter = subChapterName.Trim(),
                            Header = headerName.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        headerName = "";
                        currentContent = "";
                        order++;
                    }
                    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    headerName += text;

                    continue;
                }
            }
            
            //var blockText = string.Concat(block.Inline?.Select(i => i.ToString()) ?? []);
            var writer = new StringWriter();
            var renderer = new NormalizeRenderer(writer);
            renderer.Render(block);
            currentContent += writer.ToString();
        }
        
        foreach (var chunk in chunks)
        {
            Log.Information("New chunk: {chunkPath}\n{content}", $"{chunk.Chapter} > {chunk.Subchapter} > {chunk.Header}",
                chunk.Content);
        }

        return chunks;
    }
}