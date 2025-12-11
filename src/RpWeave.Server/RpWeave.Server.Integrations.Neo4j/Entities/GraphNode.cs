using Newtonsoft.Json;

namespace RpWeave.Server.Integrations.Neo4j.Entities;

public class GraphNode
{
    public required string EntityType { get; set; }
    public required string Name { get; set; }
    public required string CampaignId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
}