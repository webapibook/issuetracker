using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiBook.IssueTrackerApi.Tests.Handlers
{
    public class TestHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handle;

        public TestHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handle)
        {
            _handle = handle;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handle(request);
        }
    }
}