using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using AzureAdLicenseGovernor.Tests.Shared.TestContexts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes
{
    public class FunctionContextFake : FunctionContext
    {
        private readonly TestContext _context;

        public FunctionContextFake(TestContext context)
        {
            _context = context;
            var services = new ServiceCollection();

            services.AddFunctionsWorkerCore();
            services.AddFunctionsWorkerDefaults();

            InstanceServices = services.BuildServiceProvider();

        }
        public override string InvocationId => Guid.NewGuid().ToString();
        public override string FunctionId => Guid.NewGuid().ToString();
        public override TraceContext TraceContext { get; }
        public override BindingContext BindingContext { get; }
        public override IServiceProvider InstanceServices { get; set; }
        public override FunctionDefinition FunctionDefinition { get; }
        public override IDictionary<object, object> Items { get; set; }
        public override IInvocationFeatures Features { get; }

        public override RetryContext RetryContext => new RetryContextFake();
    }

    public class HttpRequestDataFake : HttpRequestData
    {
        private readonly FunctionContext _functionContext;
        private readonly string _method;
        private MemoryStream _body;
        private readonly TestContext _context;

        public HttpRequestDataFake(FunctionContext functionContext, HttpMethod method, TestContext context) : base(functionContext)
        {
            _functionContext = functionContext;
            _method = method.ToString();
            _context = context;
            _body = new MemoryStream();
        }

        public override HttpResponseData CreateResponse()
        {
            return new HttpResponseDataFake(_functionContext);
        }

        public override Stream Body => _body;
        public override HttpHeadersCollection Headers { get; } = new HttpHeadersCollection();
        public override IReadOnlyCollection<IHttpCookie> Cookies { get; } = new List<IHttpCookie>();
        public override Uri Url { get; }
        public override IEnumerable<ClaimsIdentity> Identities
        {
            get
            {
                IReadOnlyList<ClaimsIdentity> identities = new List<ClaimsIdentity> { _context.Identity.AuthenticatedUser };

                return identities;
            }
        }
        public override string Method => _method;

        public void SetBody<T>(T body)
        {
            var bodyAsJson = JsonConvert.SerializeObject(body);
            var data = Encoding.ASCII.GetBytes(bodyAsJson);
            _body = new MemoryStream(data);
        }

    }

    public class HttpResponseDataFake : HttpResponseData
    {
        public HttpResponseDataFake(FunctionContext functionContext) : base(functionContext)
        {
        }

        public override HttpStatusCode StatusCode { get; set; }
        public override HttpHeadersCollection Headers { get; set; } = new HttpHeadersCollection();
        public override Stream Body { get; set; } = new MemoryStream();
        public override HttpCookies Cookies { get; }
    }

    public class RetryContextFake : RetryContext
    {
        public override int RetryCount =>  0;

        public override int MaxRetryCount => 0;
    }
}
