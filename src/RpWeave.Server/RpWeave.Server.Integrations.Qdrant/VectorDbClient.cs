using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Qdrant.Requests;
using RpWeave.Server.Integrations.Qdrant.Responses;
using Serilog;

namespace RpWeave.Server.Integrations.Qdrant;

[ScopedService]
public class VectorDbClient
{
    private readonly QdrantClient qdrantClient;
    
    public VectorDbClient()
    {
        qdrantClient = new QdrantClient("qdrant", https: false);
    }

    public async Task CreateCollectionAsync(string collectionName)
    {
        await qdrantClient.CreateCollectionAsync(collectionName, new VectorParams()
        {
            Size = 1024,
            Distance = Distance.Cosine
        });
    }

    public async Task UpsertAsync(VectorDbUpsertRequest request)
    {
        var pointStruct = new PointStruct
        {
            Id = new PointId(Guid.NewGuid()),
            Vectors = request.Vector,
            Payload =
            {
                ["titlePath"] = request.TitlePath,
                ["text"] = request.Text
            }
        };
        
        var result = await qdrantClient.UpsertAsync(request.CollectionName, [pointStruct]);
        
        Log.Information("Updated vectors for title path {titlePath} with result {result}",
            request.TitlePath, result.Status.ToString());
    }

    public async Task<VectorDbSearchResponse> SearchAsync(VectorDbSearchRequest request)
    {
        var points = await qdrantClient.SearchAsync(
            request.CollectionName,
            request.Vector,
            limit: 8);
        
        var resultElements = new List<VectorDbSearchResponseElement>();
        foreach (var point in points)
        {
            var element = new VectorDbSearchResponseElement(
                point.Payload.FirstOrDefault(x => x.Key == "titlePath").Value.StringValue,
                point.Payload.FirstOrDefault(x => x.Key == "text").Value.StringValue,
                point.Score);
            
            resultElements.Add(element);
        }
        
        return new VectorDbSearchResponse(resultElements);
    }
}