using MongoDB.Driver;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Data.Repositories;

[ScopedService(typeof(IAppUserRefreshTokenRepository))]
public class AppUserRefreshTokenRepository : IAppUserRefreshTokenRepository
{
    private readonly IMongoCollection<AppUserRefreshToken> refreshTokenCollection;

    public AppUserRefreshTokenRepository()
    {
        var mongoClient = new MongoClient("mongodb://mongo:27017");
        
        var mongoDatabase = mongoClient.GetDatabase("rpweave");
        
        refreshTokenCollection = mongoDatabase.GetCollection<AppUserRefreshToken>("RefreshTokens");
    }

    public async Task<AppUserRefreshToken?> FindByRefreshTokenAsync(string refreshToken)
    {
        return await refreshTokenCollection
            .Find(x => x.RefreshToken == refreshToken)
            .FirstOrDefaultAsync();
    }

    public async Task UpsertAsync(AppUserRefreshToken refreshToken)
    {
        var exists = await refreshTokenCollection.Find(x => x.UserId == refreshToken.UserId)
            .AnyAsync();
        
        if(exists)
            await refreshTokenCollection.ReplaceOneAsync(x => x.UserId == refreshToken.UserId, refreshToken);
        else
            await refreshTokenCollection.InsertOneAsync(refreshToken);
    }

    public async Task DeleteAsync(AppUserRefreshToken refreshToken)
    {
        await refreshTokenCollection.DeleteOneAsync(x => x.UserId == refreshToken.UserId);
    }
}

public interface IAppUserRefreshTokenRepository
{
    Task<AppUserRefreshToken?> FindByRefreshTokenAsync(string refreshToken);
    Task UpsertAsync(AppUserRefreshToken refreshToken);
    Task DeleteAsync(AppUserRefreshToken refreshToken);
}