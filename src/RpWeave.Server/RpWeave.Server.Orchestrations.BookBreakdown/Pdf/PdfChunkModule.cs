using System.Text;
using System.Text.RegularExpressions;
using Serilog;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Pdf;

public record PdfChunkRequest(
    string FilePath,
    int ChapterFontSize,
    int SubChapterFontSize,
    int HeaderFontSize,
    bool IgnoreFooter);

public class PdfChunkModule
{
    public List<PdfChunk> ChunkPdf(PdfChunkRequest request)
    {
        using var pdf = PdfDocument.Open(request.FilePath);
        var chunks = new List<PdfChunk>();

        var chapterName = "";
        var subChapterName = "";
        var headerName = "";
        var currentContent = "";
        var order = 0;

        foreach (var page in pdf.GetPages())
        {
            var footerThreshold = page.Height * 0.05;
            var words = page.GetWords();
            var blocks = RecursiveXYCut.Instance.GetBlocks(words);
            
            var orderedBlocks = OrderTextBlocks(blocks, true);

            foreach (var block in orderedBlocks)
            {
                foreach (var line in block.TextLines)
                {
                    var lineWords = request.IgnoreFooter
                        ? line.Words.Where(x => x.BoundingBox.Bottom >= footerThreshold)
                        : line.Words;

                    foreach (var word in lineWords)
                    {
                        var text = FormatText(word.Text);

                        // assume one word only has one font size
                        var fontSize = word.Letters[0].PointSize;

                        var isChapter = fontSize >= request.ChapterFontSize;
                        var isSubChapter = fontSize >= request.SubChapterFontSize && fontSize < request.ChapterFontSize;
                        var isHeader = fontSize >= request.HeaderFontSize && fontSize < request.SubChapterFontSize;

                        if (isChapter)
                        {
                            if (currentContent.Length > 0)
                            {
                                var chunk = new PdfChunk
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

                            chapterName += text + " ";

                            continue;
                        }

                        if (isSubChapter)
                        {
                            if (currentContent.Length > 0)
                            {
                                var chunk = new PdfChunk
                                {
                                    Chapter = chapterName.Trim(),
                                    Subchapter = subChapterName.Trim(),
                                    Header = headerName.Trim(),
                                    Order = order,
                                    Content = currentContent.Trim()
                                };
                                chunks.Add(chunk);

                                // keep same chapter name for grouping
                                subChapterName = "";
                                headerName = "";
                                currentContent = "";
                                order++;
                            }

                            subChapterName += text + " ";
                            continue;
                        }
                        
                        if (isHeader)
                        {
                            if (currentContent.Length > 0)
                            {
                                var chunk = new PdfChunk
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

                            headerName += text + " ";
                            continue;
                        }

                        currentContent += text + " ";
                    }
                }
            }
        }

        foreach (var chunk in chunks)
        {
            Log.Information("New chunk: {chunkPath}\n{content}", $"{chunk.Chapter} > {chunk.Subchapter} > {chunk.Header}",
                chunk.Content);
        }

        return chunks;
    }

    private string FormatText(string text)
    {
        text = text.Normalize(NormalizationForm.FormKC);

        text = Regex.Replace(text, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "");

        text = text.Replace("ﬁ", "fi")
            .Replace("ﬂ", "fl")
            .Replace("ﬃ", "ffi")
            .Replace("ﬄ", "ffl");

        text = Regex.Replace(text, @"[^\w\s\.,;:'‘’”“\""\-\(\)\?!]", "");
        text = Regex.Replace(text, @"\s+", " ");
        text = Regex.Replace(text, @"(\w+)-\s+(\w+)", "$1$2");
        text = Regex.Replace(text, @"(?<!\n)\n(?!\n)", " ");

        text = text.Trim();

        return text;
    }

    private IEnumerable<TextBlock> OrderTextBlocks(IEnumerable<TextBlock> textBlocks, bool isDoubleColumn)
    {
        var enumeratedTextBlocks = textBlocks.ToList();
        
        if (isDoubleColumn)
        {
            var averageLeft = enumeratedTextBlocks.Average(x => x.BoundingBox.Left);
            
            return enumeratedTextBlocks.OrderBy(x => x.BoundingBox.Left > averageLeft)
                .ThenByDescending(x => x.BoundingBox.Top);
        }
        
        return enumeratedTextBlocks.OrderByDescending(x => x.BoundingBox.Top);
    }
}