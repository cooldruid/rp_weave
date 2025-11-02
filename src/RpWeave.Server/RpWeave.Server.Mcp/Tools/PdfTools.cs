using System.ComponentModel;
using ModelContextProtocol.Server;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace RpWeave.Server.Mcp.Tools;

[McpServerToolType]
public static class PdfTools
{
    [McpServerTool, Description("Extract complete text from a PDF file")]
    public static string ExtractTextFromPdf(
        [Description("The file path of the PDF file to read")] string filePath)
    {
        using var pdf = PdfDocument.Open(filePath);
        string content = "";

        foreach (var page in pdf.GetPages())
        {
            content += ContentOrderTextExtractor.GetText(page);
        }

        return content;
    }
}