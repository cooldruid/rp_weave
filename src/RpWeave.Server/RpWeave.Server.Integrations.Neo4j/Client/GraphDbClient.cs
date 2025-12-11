using Neo4jClient;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Neo4j.Entities;

namespace RpWeave.Server.Integrations.Neo4j.Client;

[SingletonService]
public class GraphDbClient : IGraphDbClient
{
    private readonly IGraphClient graphClient;
    
    public GraphDbClient()
    {
        graphClient = new BoltGraphClient("neo4j://localhost:7687", "neo4j", "<Password123>");
        graphClient.ConnectAsync().GetAwaiter().GetResult();
    }

    public async Task AddNode(GraphNode node)
    {
        await graphClient.Cypher
            .Merge($"(n:{node.EntityType} {{ Name: $name, CampaignId: $campaignId }})")
            .OnCreate()
            .Set("n = $entity")
            .WithParams(new {
                name = node.Name,
                campaignId = node.CampaignId,
                entity = node
            })
            .ExecuteWithoutResultsAsync();
    }

    public async Task AddRelationship(GraphRelationship relationship)
    {
        await graphClient.Cypher
            .Match($"(a:{relationship.NodeAType} {{ Name: $nodeA, CampaignId: $campaignId }})",
                $"(b:{relationship.NodeBType} {{ Name: $nodeB, CampaignId: $campaignId }})")
            .Merge($"(a)-[r:{relationship.RelationshipType}]->(b)")
            .WithParams(new
            {
                nodeA = relationship.NodeAName,
                nodeB = relationship.NodeBName,
                campaignId = relationship.CampaignId
            })
            .ExecuteWithoutResultsAsync();
    }
}

public interface IGraphDbClient
{
    public Task AddNode(GraphNode node);
}