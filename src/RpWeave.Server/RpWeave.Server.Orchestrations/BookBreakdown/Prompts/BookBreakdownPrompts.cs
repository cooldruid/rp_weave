namespace RpWeave.Server.Orchestrations.BookBreakdown.Prompts;

public static class BookBreakdownPrompts
{
    public const string SystemPrompt = """
                                           You are a deterministic data extraction companion integrated into a structured data pipeline.
                                           Your ONLY valid form of output is JSON objects or arrays of JSON objects, as preferred by the user.
                                           You never ask questions, never explain.
                                           If no valid output can be made, respond with nothing.
                                       """;
    
    public static string UserPrompt(string chunkContent) =>
        $$"""
            Task: Extract important TTRPG entities
            
            Extract all TTRPG-relevant entities in the text marked below.  
            Use JSON strictly.
            
            Entities:
            - NPCs (type name: 'npc')
            - Locations (type name: 'location')
            - Quests (type name: 'quest')
            - Items (type name: 'item')
            - Factions (type name: 'faction') 
            - Events (type name: 'event')
            - Monsters (type name: 'monster')
            
            Named characters are always NPCs. 
            
            For each entity:
            {
              "type": "string, use correct type name from above",
              "name": "string",
              "description": "string, be brief but complete",
              "relationships": [
                 { "target": "string, name of the other entity", "relation": "string, short description of relationship" }
              ]
            }
            
            Finally, respond with an JSON-valid array of all the items you created.

            ---
            BELOW BEGINS EXTRACT:
            {{chunkContent}}
        """;
}