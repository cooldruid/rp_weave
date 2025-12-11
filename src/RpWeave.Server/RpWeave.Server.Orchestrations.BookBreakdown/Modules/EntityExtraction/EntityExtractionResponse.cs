namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.EntityExtraction;

public record EntityExtractionResponse(List<EntityExtractionResponseElement> Elements);

public record EntityExtractionResponseElement(string Name, string Type);