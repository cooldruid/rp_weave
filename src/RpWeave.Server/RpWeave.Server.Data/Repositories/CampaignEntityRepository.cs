using MongoDB.Driver;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Data.Repositories;

[ScopedService(typeof(ICampaignEntityRepository))]
public class CampaignEntityRepository : ICampaignEntityRepository
{
    private readonly IMongoCollection<CampaignEntity> collection;

    public CampaignEntityRepository()
    {
        var mongoClient = new MongoClient("mongodb://mongo:27017");

        var mongoDatabase = mongoClient.GetDatabase("rpweave");

        collection = mongoDatabase.GetCollection<CampaignEntity>("Campaigns");
    }

    public async Task AddAsync(CampaignEntity entity)
    {
        await collection.InsertOneAsync(entity);
    }

    public async Task<List<CampaignEntity>> ListAsync()
    {
        return (await collection.FindAsync(x => true)).ToList();
    }
}

public interface ICampaignEntityRepository
{
    Task AddAsync(CampaignEntity entity);
    Task<List<CampaignEntity>> ListAsync();
}