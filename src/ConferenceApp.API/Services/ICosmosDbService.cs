using ConferenceApp.Shared.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ConferenceApp.API.Services;

/// <summary>
/// Generic repository for interacting with Cosmos DB
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface ICosmosDbService<T> where T : BaseEntity
{
    /// <summary>
    /// Get item by ID
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="partitionKey">Partition key</param>
    /// <returns>The requested entity or null</returns>
    Task<T?> GetItemAsync(string id, string partitionKey);

    /// <summary>
    /// Get all items of a type
    /// </summary>
    /// <param name="partitionKey">Partition key to filter by</param>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetItemsAsync(string partitionKey);

    /// <summary>
    /// Add a new item
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns>The created entity</returns>
    Task<T> AddItemAsync(T item);

    /// <summary>
    /// Update an existing item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="item">Updated item</param>
    /// <returns>The updated entity</returns>
    Task<T> UpdateItemAsync(string id, T item);

    /// <summary>
    /// Delete an item by ID
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="partitionKey">Partition key</param>
    /// <returns>Task representing the operation</returns>
    Task DeleteItemAsync(string id, string partitionKey);

    /// <summary>
    /// Query items using a predicate
    /// </summary>
    /// <param name="predicate">Filter expression</param>
    /// <param name="partitionKey">Optional partition key for cross-partition queries</param>
    /// <returns>Collection of entities matching the predicate</returns>
    Task<IEnumerable<T>> QueryItemsAsync(Func<T, bool> predicate, string? partitionKey = null);
}
