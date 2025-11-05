using Serilog;
using UglyToad.PdfPig;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Pdf;

public class PdfChunkingProcessor
{
    public List<PdfChunk> ChunkPdf(string filePath, bool ignoreFooter = true)
    {
        var chapterFontSize = 20;
        var subChapterFontSize = 15;
        
        using var pdf = PdfDocument.Open(filePath);
        var chunks = new List<PdfChunk>();

        var chapterName = "";
        var subChapterName = "";
        var currentContent = "";
        var order = 0;
        
        foreach (var page in pdf.GetPages())
        {
            var footerThreshold = page.Height * 0.05;
            var words = page.GetWords();
            
            if(ignoreFooter)
                words = words.Where(x => x.BoundingBox.Bottom > footerThreshold);
            
            var columns = words
                .GroupBy(w => w.BoundingBox.Left < page.Width / 2 ? 1 : 2);
            
            Log.Information("Page {pageNumber}", page.Number);
            var side = 1;
            foreach (var column in columns.Where(x => x.Key == 1))
            {
                foreach (var word in column.Select(x => x))
                {
                    // assume one word only has one font size
                    var fontSize = word.Letters[0].PointSize;
                    var isChapter = fontSize >= chapterFontSize;
                    var isSubChapter = fontSize >= subChapterFontSize && fontSize < chapterFontSize;

                    if (isChapter)
                    {
                        if (currentContent.Length > 0)
                        {
                            var chunk = new PdfChunk 
                            {
                                Chapter = chapterName.Trim(), 
                                Subchapter = subChapterName.Trim(), 
                                Order = order, 
                                Content = currentContent.Trim()
                            };
                            chunks.Add(chunk);
                            
                            chapterName = "";
                            subChapterName = "";
                            currentContent = "";
                            order++;
                        }

                        chapterName += word.Text + " ";
                        
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
                                Order = order, 
                                Content = currentContent.Trim()
                            };
                            chunks.Add(chunk);
                            
                            // keep same chapter name for grouping
                            subChapterName = "";
                            currentContent = "";
                            order++;
                        }

                        
                        subChapterName += word.Text + " ";
                        continue;
                    }

                    currentContent += word.Text + " ";
                }
            }
            
            foreach (var column in columns.Where(x => x.Key == 2))
            {
                foreach (var word in column.Select(x => x))
                {
                    // assume one word only has one font size
                    var fontSize = word.Letters[0].PointSize;
                    var isChapter = fontSize >= chapterFontSize;
                    var isSubChapter = fontSize >= subChapterFontSize && fontSize < chapterFontSize;

                    if (isChapter)
                    {
                        if (currentContent.Length > 0)
                        {
                            var chunk = new PdfChunk 
                            {
                                Chapter = chapterName.Trim(), 
                                Subchapter = subChapterName.Trim(), 
                                Order = order, 
                                Content = currentContent.Trim()
                            };
                            chunks.Add(chunk);
                            
                            chapterName = "";
                            subChapterName = "";
                            currentContent = "";
                            order++;
                        }

                        chapterName += word.Text + " ";
                        
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
                                Order = order, 
                                Content = currentContent.Trim()
                            };
                            chunks.Add(chunk);
                            
                            // keep same chapter name for grouping
                            subChapterName = "";
                            currentContent = "";
                            order++;
                        }

                        
                        subChapterName += word.Text + " ";
                        continue;
                    }

                    currentContent += word.Text + " ";
                }
            }
        }

        return chunks;
    }
}