using StackExchange.Redis;
using UserManagment.Service.Service;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IDatabase _redisDb;

    public TokenBlacklistService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task BlacklistTokenAsync(string token, TimeSpan expiry)
    {
        await _redisDb.StringSetAsync(GetKey(token), "blacklisted", expiry);
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _redisDb.KeyExistsAsync(GetKey(token));
    }

    private string GetKey(string token) => $"blacklist:{token}";
}
