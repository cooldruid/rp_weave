namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.RelationshipExtraction;

public static class RelationshipExtractionPrompts
{
    public const string SystemPrompt = """
                                           You are a deterministic data extraction system.
                                           You do not explain, do not ask questions, and do not add commentary.
                                           If the source text contains no valid entities, output nothing.
                                           
                                           Task: Extract relationships of the given TTRPG entity.
                                       """;

    public static string UserPrompt(string sourceText, string entityName, List<string> allEntityNames)
    {
        return $$"""
                     SOURCE TEXT:
                     {{sourceText}}
                     END SOURCE TEXT
                     
                     Entity of focus: {{entityName}}
                     
                     List of valid related entities:
                     {{string.Join(", ", allEntityNames)}}
                     
                     Allowed relationship types:
                     - located
                     - part_of
                     - owner
                     - ally
                     - enemy
                     - family
                     - worship
                     - control
                     - responsible
                     - desire
                     
                     Instructions:
                     1. Identify zero or more relationships between the entity of focus and the VALID RELATED ENTITIES listed above.
                     2. Use ONLY the entities provided in the list. Do not invent new ones.
                     3. Use EXACT entity names even if they contain punctuation such as colons.
                     4. If no valid relationships exist, output nothing.
                     5. Do NOT treat area labels, section headers, room numbers, or text in the format "Area X: Something" as entities.
                     6. Do not overthink. Quickly make a best-effort guess for the relationship type. A human will oversee and correct any mistakes.
                     7. Output JSON Lines where each line is:
                     {"entity": "<entityName>", "relationship": "<relationshipType>"}
                         - entity -> the related entity
                         - relationship -> the relationship type
                         The entity of focus does not need to be added to the JSON. The user is aware which entity is of focus.
                 
                     Format rules:
                     - Do not explain.
                     - Do not include the entity of focus as a related entity.
                     - Do not output arrays. Only JSON Lines.
                 """;
    }

    public static string RepairPrompt(string wrongOutput) =>
        $$"""
          The previous response was not valid JSON. Here is the exact text returned:
          {{wrongOutput}}

          Please transform it into EXACTLY this JSON shape, adding each JSON on a new line:
          {"entity": "<entityName>", "relationship": "<relationshipType>"}
          
          The invalid output might contain references of source entity and target entity. If more than one entity exist, always choose the one that makes the most sense as the target entity, subject, etc.
          A pattern you may find is that entities that you should ignore might be repeating in every JSON object you find.
          
          Transform as much as you can, skip the ones you cannot transform. If you cannot transform anything, output nothing.
          
          Respond with valid JSON and nothing else. Do not explain or ask questions.
          """;
}