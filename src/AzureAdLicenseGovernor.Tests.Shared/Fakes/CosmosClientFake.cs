using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureAdLicenseGovernor.Core.Models.Data;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes
{
    public class CosmosClientFake : CosmosClient
    {
        private readonly TestContext _context;

        public CosmosClientFake(TestContext context)
        {
            _context = context;
        }
        public override Container GetContainer(string databaseId, string containerId)
        {
            return new ContainerFake(_context);
        }
    }

    public class ContainerFake : Container
    {
        private TestContext _context;

        public ContainerFake(TestContext context)
        {
            _context = context;
        }

        public override string Id => throw new NotImplementedException();

        public override Database Database => throw new NotImplementedException();

        public override Conflicts Conflicts => throw new NotImplementedException();

        public override Scripts Scripts => throw new NotImplementedException();

        public override Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ResponseMessage> CreateItemStreamAsync(Stream streamPayload, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey)
        {
            throw new NotImplementedException();
        }

        public override Task<ContainerResponse> DeleteContainerAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ResponseMessage> DeleteContainerStreamAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ItemResponse<T>> DeleteItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var data = ResolveFakeDatasource<T>();

            data.TryRemove(id, out T removed);
            return Task.FromResult((ItemResponse<T>)new ItemResponseFake<T>(removed));
        }

        public override Task<ResponseMessage> DeleteItemStreamAsync(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(string processorName, ChangesEstimationHandler estimationDelegate, TimeSpan? estimationPeriod = null)
        {
            throw new NotImplementedException();
        }

        public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(string processorName, ChangesHandler<T> onChangesDelegate)
        {
            throw new NotImplementedException();
        }

        public override IOrderedQueryable<T> GetItemLinqQueryable<T>(bool allowSynchronousQueryExecution = false, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var data = ResolveFakeDatasource<T>();

            var result = data.Values
                   .AsQueryable()
                   .Cast<T>()
                   .OrderBy(d => "value");

            return result;
        }

        public override FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            throw new NotImplementedException();
        }

        public override FeedIterator<T> GetItemQueryIterator<T>(string queryText = null, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            throw new NotImplementedException();
        }

        public override FeedIterator GetItemQueryStreamIterator(QueryDefinition queryDefinition, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            throw new NotImplementedException();
        }

        public override FeedIterator GetItemQueryStreamIterator(string queryText = null, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            throw new NotImplementedException();
        }

        public override Task<ContainerResponse> ReadContainerAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ResponseMessage> ReadContainerStreamAsync(ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ItemResponse<T>> ReadItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var data = ResolveFakeDatasource<T>();

            var result = data.GetValueOrDefault(id);
            return Task.FromResult((ItemResponse<T>)new ItemResponseFake<T>(result));
        }

        public override Task<ResponseMessage> ReadItemStreamAsync(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ContainerResponse> ReplaceContainerAsync(ContainerProperties containerProperties, ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ResponseMessage> ReplaceContainerStreamAsync(ContainerProperties containerProperties, ContainerRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ItemResponse<T>> ReplaceItemAsync<T>(T item, string id, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ResponseMessage> ReplaceItemStreamAsync(Stream streamPayload, string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<ItemResponse<T>> UpsertItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            var dataset = ResolveFakeDatasource<T>();
            var idValue = GetIdValue(item);
            dataset.AddOrUpdate(idValue, item, (id, existing) => { return item; });

            return Task.FromResult((ItemResponse<T>)new ItemResponseFake<T>(item));
        }

        public override Task<ResponseMessage> UpsertItemStreamAsync(Stream streamPayload, PartitionKey partitionKey, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private ConcurrentDictionary<string, T> ResolveFakeDatasource<T>()
        {
            var typeToFind = typeof(T);

            if (typeToFind == typeof(DirectoryData)) return _context.Data.Directories as ConcurrentDictionary<string, T>;
            if (typeToFind == typeof(LicensedGroupData)) return _context.Data.Groups as ConcurrentDictionary<string, T>;
            if (typeToFind == typeof(ProductDataSnapshot)) return _context.Data.ProductSnapshots as ConcurrentDictionary<string, T>;

            throw new ArgumentOutOfRangeException($"Unsupported Type: {typeToFind.Name}");
        }

        private string GetIdValue<T>(T toRead)
        {
            var idValue = typeof(T).GetProperty("Id").GetValue(toRead);

            return idValue.ToString();
        }
    }

    public class ItemResponseFake<T> : ItemResponse<T>
    {
        public ItemResponseFake(object resource)
        {
            Resource = (T)resource;
        }

        public override T Resource { get; }
    }
}
