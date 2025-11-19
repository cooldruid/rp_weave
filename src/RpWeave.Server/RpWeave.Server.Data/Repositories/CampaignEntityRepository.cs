using MongoDB.Bson;
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

    public async Task<CampaignEntity> GetAsync(string id)
    {
        return await collection.Find(x => x.Id == ObjectId.Parse(id))
            .FirstOrDefaultAsync();
    }

    public async Task<List<CampaignEntity>> ListAsync()
    {
        return (await collection.FindAsync(x => true)).ToList();
    }
}

public interface ICampaignEntityRepository
{
    Task AddAsync(CampaignEntity entity);
    Task<CampaignEntity> GetAsync(string id);
    Task<List<CampaignEntity>> ListAsync();
}