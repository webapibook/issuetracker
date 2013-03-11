using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class BasicAuthnClientHandler : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, Task<BasicCredentials>> _prov;

        public BasicAuthnClientHandler(Func<HttpRequestMessage, Task<BasicCredentials>> prov)
        {
            _prov = prov;
        }

        protected override async Task<HttpResponseMessage> 
            SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var creds = await _prov(request);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", creds.ToCredentialString());
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
