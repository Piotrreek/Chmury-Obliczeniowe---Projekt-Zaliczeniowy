using System.Text.Json;
using Chmury.Infrastructure;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using OneOf;
using OneOf.Types;

namespace Chmury.Services;

public interface INeo4jService : IAsyncDisposable
{
    Task<OneOf<Success, string>> WriteAsync<T>(string query, IDictionary<string, object>? parameters = null);

    Task<OneOf<Success<List<T>>, string>> ReadListAsync<T>(string query, string returnObjectKey,
        IDictionary<string, object>? parameters = null);
}

public class Neo4jService : INeo4jService
{
    private readonly IAsyncSession _session;

    public Neo4jService(IDriver driver, IOptions<Neo4jSettings> options)
    {
        var database = options.Value.Neo4jDatabase;
        _session = driver.AsyncSession(o => o.WithDatabase(database));
    }

    public async Task<OneOf<Success, string>> WriteAsync<T>(string query,
        IDictionary<string, object>? parameters = null)
    {
        try
        {
            parameters = parameters ?? new Dictionary<string, object>();
            await _session.ExecuteWriteAsync(async tx => await tx.RunAsync(query, parameters));
            return new Success();
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<OneOf<Success<List<T>>, string>> ReadListAsync<T>(string query, string returnObjectKey,
        IDictionary<string, object>? parameters = null)
    {
        return await ExecuteReadTransactionAsync<T>(query, returnObjectKey, parameters);
    }

    public async ValueTask DisposeAsync()
    {
        await _session.CloseAsync();
    }

    private async Task<OneOf<Success<List<T>>, string>> ExecuteReadTransactionAsync<T>(string query,
        string returnObjectKey, IDictionary<string, object>? parameters)
    {
        try
        {
            parameters = parameters ?? new Dictionary<string, object>();

            var result = await _session.ExecuteReadAsync(async tx =>
            {
                var res = await tx.RunAsync(query, parameters);
                var list = new List<T>();

                while (await res.FetchAsync())
                {
                    var json = JsonSerializer.Serialize(res.Current[0].As<INode>().Properties,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                    list.Add(JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })!);
                }

                return list;
            });

            return new Success<List<T>>(result);
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}