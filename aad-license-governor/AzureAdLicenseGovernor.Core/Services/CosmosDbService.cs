using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace AzureAdLicenseGovernor.Core.Services
{
    public class CosmosDbService
    {
        private readonly CosmosClient _client;
        private readonly string _defaultDatabaseId;
        private readonly ICosmosLinqQueryFactory _cosmosLinqQueryFactory;

        public CosmosDbService(CosmosClient cosmosClient, string defaultDatabaseId, ICosmosLinqQueryFactory cosmosLinqQueryFactory)
        {
            _client = cosmosClient;
            _defaultDatabaseId = defaultDatabaseId;
            _cosmosLinqQueryFactory = cosmosLinqQueryFactory;
        }

        public IQueryable<T> Query<T>(string collectionId, string databaseId = null)
        {
            var result = _client.GetContainer(databaseId ?? _defaultDatabaseId, collectionId)
                .GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true);

            return result;
        }

        public async Task<T> Get<T>(string id, string containerId, string partitionKey, string databaseId = null)
        {
            try
            {
                var result = await _client.GetContainer(databaseId ?? _defaultDatabaseId, containerId)
                    .ReadItemAsync<T>(id, new PartitionKey(partitionKey));

                return result.Resource;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return default(T);

                throw;
            }
        }

        public async Task<T> Save<T>(T toSave, string collectionId, string databaseId = null)
        {
            var result = await _client.GetContainer(databaseId ?? _defaultDatabaseId, collectionId)
                .UpsertItemAsync(toSave);

            return result.Resource;
        }

        public async Task Save<T>(IEnumerable<T> toSave, string collectionId, Func<T, string> getPartitionKey, string databaseId = null)
        {
            var container = _client.GetContainer(databaseId ?? _defaultDatabaseId, collectionId);

            var upsertTasks = toSave
                .Select(item => container.UpsertItemAsync(item, new PartitionKey(getPartitionKey(item))));

            await Task.WhenAll(upsertTasks);
        }

        public async Task<T> Delete<T>(string id, string containerId, string partitionKey, string databaseId = null)
        {
            var result = await _client.GetContainer(databaseId ?? _defaultDatabaseId, containerId)
                .DeleteItemAsync<T>(id, new PartitionKey(partitionKey));

            return result.Resource;
        }

        public async Task<ICollection<TOut>> ExecuteRead<TIn, TOut>(IQueryable<TIn> query, Func<TIn, TOut> mapper)
        {
            var iterator = _cosmosLinqQueryFactory.GetFeedIterator(query);

            var result = new List<TOut>();
            while (iterator.HasMoreResults)
            {
                var inputData = await iterator.ReadNextAsync();
                var mappedData = inputData.Select(mapper);
                result.AddRange(mappedData);
            }

            return result;
        }

        public async Task<ICollection<T>> ExecuteRead<T>(IQueryable<T> query)
        {
            var iterator = _cosmosLinqQueryFactory.GetFeedIterator(query);

            var result = new List<T>();
            while (iterator.HasMoreResults)
            {
                result.AddRange(await iterator.ReadNextAsync());
            }

            return result;
        }

    }

    public interface ICosmosLinqQueryFactory
    {
        FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query);
    }

    public class CosmosLinqQueryFactory : ICosmosLinqQueryFactory
    {
        /// <summary>
        /// Wraps to FeedIterator so we have a seam that is usable for testing
        /// </summary>
        /// <see cref="https://github.com/Azure/azure-cosmos-dotnet-v3/issues/893"/>
        public FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query)
        {
            return query.ToFeedIterator();
        }


    }
}
