namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.RelationshipExtraction;

public record RelationshipExtractionResponse(List<RelationshipExtractionElement> Elements);

public record RelationshipExtractionElement(string SourceEntity, string TargetEntity, string Relationship);