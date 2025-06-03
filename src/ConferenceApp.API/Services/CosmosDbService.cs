using ConferenceApp.Shared.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ConferenceApp.API.Services;

/// <summary>
/// Implementation of the Cosmos DB service for entity operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public class CosmosDbService<T> : ICosmosDbService<T> where T : BaseEntity
{
    private readonly Container _container;

    /// <summary>
    /// Constructor to initialize Cosmos DB container
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client</param>
    /// <param name="databaseName">Database name</param>
    /// <param name="containerName">Container name</param>
    public CosmosDbService(
        CosmosClient cosmosClient,
        string databaseName,
        string containerName)
    {
        _container = cosmosClient.GetContainer(databaseName, containerName);
    }

    /// <inheritdoc/>
    public async Task<T?> GetItemAsync(string id, string partitionKey)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(
                id, new PartitionKey(partitionKey));
            
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> GetItemsAsync(string partitionKey)
    {
        var query = _container.GetItemLinqQueryable<T>(
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            })
            .ToFeedIterator();

        var results = new List<T>();
        
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        
        return results;
    }

    /// <inheritdoc/>
    public async Task<T> AddItemAsync(T item)
    {
        var response = await _container.CreateItemAsync(
            item, new PartitionKey(item.PartitionKey));
            
        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<T> UpdateItemAsync(string id, T item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        
        var response = await _container.ReplaceItemAsync(
            item, id, new PartitionKey(item.PartitionKey));
            
        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task DeleteItemAsync(string id, string partitionKey)
    {
        await _container.DeleteItemAsync<T>(
            id, new PartitionKey(partitionKey));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> QueryItemsAsync(Func<T, bool> predicate, string? partitionKey = null)
    {
        QueryRequestOptions? options = null;
        
        if (!string.IsNullOrEmpty(partitionKey))
        {
            options = new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            };
        }
        
        var query = _container.GetItemLinqQueryable<T>(requestOptions: options)
            .ToFeedIterator();

        var results = new List<T>();
        
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.Where(predicate));
        }
        
        return results;
    }
}
