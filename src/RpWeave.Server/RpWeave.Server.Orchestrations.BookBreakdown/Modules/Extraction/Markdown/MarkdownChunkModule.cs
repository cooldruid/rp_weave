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
        
        var level1Heading = "";
        var level2Heading = "";
        var level3Heading = "";
        var level4Heading = "";
        var level5Heading = "";
        var currentContent = "";
        var order = 0;
        
        var chunks = new List<TextChunk>();

        foreach (var block in markdown)
        {
            if (block is HeadingBlock heading)
            {
                if (heading.Level == 1)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Level1Heading = level1Heading.Trim(),
                            Level2Heading = level2Heading.Trim(),
                            Level3Heading = level3Heading.Trim(),
                            Level4Heading = level4Heading.Trim(),
                            Level5Heading = level5Heading.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        level1Heading = "";
                        level2Heading = "";
                        level3Heading = "";
                        level4Heading = "";
                        level5Heading = "";
                        currentContent = "";
                        order++;
                    }
                    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    level1Heading += text;

                    continue;
                }
                
                if (heading.Level == 2)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Level1Heading = level1Heading.Trim(),
                            Level2Heading = level2Heading.Trim(),
                            Level3Heading = level3Heading.Trim(),
                            Level4Heading = level4Heading.Trim(),
                            Level5Heading = level5Heading.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        level2Heading = "";
                        level3Heading = "";
                        level4Heading = "";
                        level5Heading = "";
                        currentContent = "";
                        order++;
                    }
    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    level2Heading += text;

                    continue;
                }
                
                if (heading.Level == 3)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Level1Heading = level1Heading.Trim(),
                            Level2Heading = level2Heading.Trim(),
                            Level3Heading = level3Heading.Trim(),
                            Level4Heading = level4Heading.Trim(),
                            Level5Heading = level5Heading.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        level3Heading = "";
                        level4Heading = "";
                        level5Heading = "";
                        currentContent = "";
                        order++;
                    }
                    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    level3Heading += text;

                    continue;
                }
                
                if (heading.Level == 4)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Level1Heading = level1Heading.Trim(),
                            Level2Heading = level2Heading.Trim(),
                            Level3Heading = level3Heading.Trim(),
                            Level4Heading = level4Heading.Trim(),
                            Level5Heading = level5Heading.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        level4Heading = "";
                        level5Heading = "";
                        currentContent = "";
                        order++;
                    }
                    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    level4Heading += text;

                    continue;
                }
                
                if (heading.Level == 5)
                {
                    if (currentContent.Length > 0)
                    {
                        var chunk = new TextChunk
                        {
                            Level1Heading = level1Heading.Trim(),
                            Level2Heading = level2Heading.Trim(),
                            Level3Heading = level3Heading.Trim(),
                            Level4Heading = level4Heading.Trim(),
                            Level5Heading = level5Heading.Trim(),
                            Order = order,
                            Content = currentContent.Trim()
                        };
                        chunks.Add(chunk);

                        level5Heading = "";
                        currentContent = "";
                        order++;
                    }
                    
                    var text = string.Concat(heading.Inline?.Select(i => i.ToString()) ?? []);
                    level5Heading += text;

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
            Log.Information("New chunk: {chunkPath}\n{content}", $"{chunk.Level1Heading} > {chunk.Level2Heading} > {chunk.Level3Heading}",
                chunk.Content);
        }

        return chunks;
    }
}