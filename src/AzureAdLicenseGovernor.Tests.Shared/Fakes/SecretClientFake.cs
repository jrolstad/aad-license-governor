using System;
using System.Threading;
using Azure;
using Azure.Security.KeyVault.Secrets;

namespace AzureAdLicenseGovernor.Tests.Shared.Fakes
{
    public class SecretClientFake : SecretClient
    {
        public override Response<KeyVaultSecret> GetSecret(string name,
            string version = null,
            CancellationToken cancellationToken = default)
        {
            var secret = new KeyVaultSecret(name: name, value: Guid.NewGuid().ToString());
            return new ResponseFake<KeyVaultSecret>(secret);
        }
    }

    public class ResponseFake<T> : Response<T>
    {
        public ResponseFake(T value)
        {
            Value = value;
        }
        public override T Value { get; }

        public override Response GetRawResponse()
        {
            throw new NotImplementedException();
        }
    }
}
