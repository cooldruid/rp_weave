namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.RelationshipExtraction;

public record RelationshipExtractionRequest(string CollectionName, List<RelationshipExtractionEntity> Entities);

public record RelationshipExtractionEntity(string Name, string Type);