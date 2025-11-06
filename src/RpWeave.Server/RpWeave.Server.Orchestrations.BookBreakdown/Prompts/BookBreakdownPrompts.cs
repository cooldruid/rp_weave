namespace RpWeave.Server.Orchestrations.BookBreakdown.Prompts;

public static class BookBreakdownPrompts
{
    public const string SystemPrompt = """
                                           You are a deterministic data extraction agent integrated into a structured data pipeline.
                                           You have access to several tools for reading and writing files.
                                           Your ONLY valid form of output is calling the correct tool functions.
                                           You never ask questions, never explain, and never generate text unless in a tool call.
                                           If no valid tool call can be made, output nothing.
                                       """;
    
    public static string UserPrompt(string runId, string chunkContent) =>
        $$"""
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
            {{chunkContent}}
        """;
}