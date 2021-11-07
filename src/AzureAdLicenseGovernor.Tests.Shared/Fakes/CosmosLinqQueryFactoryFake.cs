using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureAdLicenseGovernor.Core.Services;
using Microsoft.Azure.Cosmos;


namespace AzureAdLicenseGovernor.Tests.Shared.Fakes
{
    public class CosmosLinqQueryFactoryFake : ICosmosLinqQueryFactory
    {
        public FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query)
        {
            return new FeedIteratorFake<T>(query);
        }
    }

    public class FeedIteratorFake<T> : FeedIterator<T>
    {
        public FeedIteratorFake(IQueryable<T> query)
        {
            _query = query;
        }

        private bool _hasResults = true;
        private readonly IQueryable<T> _query;

        public override bool HasMoreResults => _hasResults;

        public override Task<FeedResponse<T>> ReadNextAsync(CancellationToken cancellationToken = default)
        {
            _hasResults = false;
            FeedResponse<T> response = new FeedResponseFake<T>(_query);
            return Task.FromResult(response);

        }
    }

    public class FeedResponseFake<T> : FeedResponse<T>
    {
        private ICollection<T> _dataSource;

        public FeedResponseFake(IQueryable<T> query)
        {
            _dataSource = query.ToList();

        }

        public override string ContinuationToken => null;

        public override int Count => _dataSource.Count;

        public override Headers Headers => new Headers();

        public override IEnumerable<T> Resource => _dataSource;

        public override HttpStatusCode StatusCode => HttpStatusCode.OK;

        public override CosmosDiagnostics Diagnostics => null;

        public override IEnumerator<T> GetEnumerator()
        {
            return _dataSource.GetEnumerator();
        }
    }
}
