namespace RpWeave.Server.Integrations.Neo4j.Entities;

public class GraphRelationship
{
    public required string CampaignId { get; set; }
    public required string NodeAName { get; set; }
    public required string NodeAType { get; set; }
    public required string NodeBName { get; set; }
    public required string NodeBType { get; set; }
    public required string RelationshipType { get; set; }
}