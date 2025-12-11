namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.EntityExtraction;

public static class EntityExtractionPrompts
{
    public const string SystemPrompt = """
                                           You are a deterministic data extraction system.
                                           You do not explain, do not ask questions, and do not add commentary.
                                           If the source text contains no valid entities, output nothing.
                                           
                                           Task: Extract only narratively significant TTRPG entities.
                                       """;

    public static string UserPrompt(string sourceText)
    {
        return $"""
                    SOURCE TEXT:
                    {sourceText}
                    END SOURCE TEXT
                    
                    Entity types:
                    - npc
                    - location
                    - quest
                    - item
                    - faction
                    - event
                    - deity
                    
                    Rules:
                    1. Extract ONLY narratively significant entities: named characters, named places, major quest objectives, unique magical or plot-relevant items, major factions, and major story events.
                    2. DO NOT extract:
                       - generic weapons, tools, or equipment (swords, bows, torches, etc.).
                       - unnamed monsters or generic undead/animals.
                       - generic rooms or directions (small room, hallway, area 2, intersection, north end, etc.).
                       - mechanical actions or trivial events (a door opens, a lever is pulled, a trap activates, etc.).
                       - repeated duplicates.
                    
                    3. Events must be story events with consequences (e.g. “undead uprising”, “collapse of the temple”), not moment-to-moment actions.
                    
                    Output format:
                    A comma-separated list of entries in the format:
                    <entity_name>:<entity_type>
                    
                    Example:
                    John Doe:npc,Temple of Deity:location,Faction of Deity:faction
                """;
    }
}