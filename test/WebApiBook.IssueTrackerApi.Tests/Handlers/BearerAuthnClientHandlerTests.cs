using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Infrastructure;
using Xunit;

namespace WebApiBook.IssueTrackerApi.Tests.Handlers
{
    class BearerAuthnClientHandlerTests
    {
        public class TheBasicAuthnClientHandler
        {
            [Fact]
            public async Task ShouldAddBasicCredentialsToTheOutboundRequest()
            {
                const string token = "TheToken";
                
                Func<HttpRequestMessage, Task<HttpResponseMessage>> assert = req =>
                {
                    Assert.True(
                        req.Headers.Authorization.
                            Scheme.Equals("bearer", StringComparison.InvariantCultureIgnoreCase));
                    Assert.NotNull(req.Headers.Authorization.Parameter);
                    Assert.Equal(token, req.Headers.Authorization.Parameter);
                    return Task.FromResult(new HttpResponseMessage());
                };

                using (var client = new HttpClient(
                    new BearerAuthnClientHandler(req => Task.FromResult(new BearerToken(token)))
                    {
                        InnerHandler = new TestHandler(assert)
                    }))
                {
                    var resp = await client.GetAsync("https://it.does.not.matter");
                    Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                }
            }
        }
    }
}
