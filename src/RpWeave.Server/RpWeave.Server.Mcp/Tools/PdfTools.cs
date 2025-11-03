using System.ComponentModel;
using ModelContextProtocol.Server;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace RpWeave.Server.Mcp.Tools;

// [McpServerToolType]
// public static class PdfTools
// {
//     [McpServerTool, Description("Extract text from a PDF file from a specific page. If the page number is beyond the total amount of pages, the tool returns an empty string.")]
//     public static string ExtractTextFromPdf(
//         [Description("The file path of the PDF file to read")] string filePath,
//         [Description("The page number to extract. First page is always 1.")] int pageNumber)
//     {
//         using var pdf = PdfDocument.Open(filePath);
//
//         if (pageNumber > pdf.GetPages().Count())
//             return "";
//         
//         var page = pdf.GetPage(pageNumber);
//         return ContentOrderTextExtractor.GetText(page);
//     }
// }