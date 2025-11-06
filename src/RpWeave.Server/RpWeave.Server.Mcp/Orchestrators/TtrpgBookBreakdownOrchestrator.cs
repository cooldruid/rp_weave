using RpWeave.Server.Core.Startup;
using Serilog;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace RpWeave.Server.Mcp.Orchestrators;

[ScopedService]
public class TtrpgBookBreakdownOrchestrator
{
    private readonly OllamaClientWithFunctions ollamaClient = new();

    public async Task<string> OrchestrateAsync(string fileLocation)
    {
        var runId = Guid.NewGuid().ToString();
        
        var systemPrompt = """
                           You are a deterministic data extraction agent integrated into a structured data pipeline.
                           You have access to several tools for reading and writing files.
                           Your ONLY valid form of output is calling the correct tool functions.
                           You never ask questions, never explain, and never generate text unless in a tool call.
                           If no valid tool call can be made, output nothing.
                           """;
        
        var userPrompt = $$"""
                           Task: Extract named NPCs from the TTRPG module extract.
                           
                           You are given the text of an extract of a TTRPG (Dungeons & Dragons, Pathfinder, etc.) module. Perform the following flow in order:
                           
                           1. Check if any mention or description of named NPCs exist. Ignore unnamed characters and monsters.
                             1.2. If none are present, do nothing.
                             1.3. If at least one is present, continue with the following steps and perform them for each NPC you have found.
                           2. Check if the JSON for this NPC exists using the following tool ListAllJsonFilesAsync(runId: "{{runId}}"). Match the NPC you found with the existing files by the NPC name:
                             2.1. If it already exists, read the content using ReadJsonFromFileAsync(runId: "{{runId}}", fileName: <filename>) and evaluate if the information you gathered is there and sufficient
                               2.1.1. If the information you found isn't represented, enrich the entry by appending to the current values based on the data you find and store it using WriteJsonToFileAsync(runId: "{{runId}}", fileName: <filename>, content: <json>).
                                        Do not change the JSON schema.
                               2.1.2. If your information is there and you don't believe it needs a change, do nothing and continue with the next NPC
                             2.2. If it does not exist, create a JSON object:
                             {
                               "name": string,
                               "appearance": string,
                               "personality": string
                             }
                               - name = the character's name from the module
                               - appearance = a short summary of appearance (no stats)
                               - personality = a short summary of personality (no stats)
                               - leave fields blank if unsure
                           
                           For each JSON object:
                           1. The filename must be PascalCase(name) + ".json"
                           2. Call the tool WriteJsonToFileAsync(runId: "{{runId}}", fileName: <filename>, content: <json>)
                           3. Do NOT print or describe the JSON — only invoke the tool.
                           
                           ---
                           BELOW BEGINS MODULE EXTRACT:
                           """;

        var response = "";
        var pageNumber = 1;
        var pdfText = "";
        do
        {
            Log.Information("Extracting page: {pageNumber}", pageNumber);
            pdfText = ExtractTextFromPdf(fileLocation, pageNumber);
            response += await ollamaClient.SendAsync(systemPrompt, userPrompt + $"\n{pdfText}");
            pageNumber++;
        }
        while(pdfText != "");

        return response;
    }
    
    public static string ExtractTextFromPdf(
        string filePath,
        int pageNumber)
    {
        using var pdf = PdfDocument.Open(filePath);

        if (pageNumber > pdf.GetPages().Count())
            return "";

        var page = pdf.GetPage(pageNumber);
        return ContentOrderTextExtractor.GetText(page);
    }
    
    // var prompt = $"You are an assistant to a Dungeons & Dragons Dungeon Master. The game will be ran with the help of a module. " +
    //              $"It contains adult themes and monsters and that is intended. Do not suggest any changes or ideas. Only perform the following task. " +
    //              $"The module is in PDF format and accessible to you at the following location: {fileLocation}. " +
    //              $"Using any functions you choose that are provided to you, extract the text of the PDF and store a separate JSON file " +
    //              $"for each named NPC that you find. Skip monsters without names, such as 'Giant Rat'. The JSON that you store must conform to the following standards:\n" +
    //              $"1. The file name must be the name of the NPC written in pascal case with no spaces or other punctuation.\n" +
    //              $"2. The JSON content must conform to the following template:\n" +
    //              $"{{name: string, description: string}}\n" +
    //              $"The name property should be filled with the name of the NPC and the description with a brief summary about what they look like and their personality. Do not concern yourself with statblocks.\n" +
    //              $"3. Be as close as possible to the original material and do not come up with new NPCs or information that is not backed up by the source. " +
    //              $"If you consider that the PDF does not contain enough information to fill a property, leave it blank.\n" +
    //              $"Functions that you call may require a runId for correlation, use this: {runId}\n" +
    //              $"Do not respond with a suggestion. Only create the necessary JSON files as mentioned previously.";
}