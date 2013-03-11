using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class BearerAuthnClientHandler : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, Task<BearerToken>> _prov;

        public BearerAuthnClientHandler(Func<HttpRequestMessage, Task<BearerToken>> prov)
        {
            _prov = prov;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var token = await _prov(request);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token.Value);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}