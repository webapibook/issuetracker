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
    public class BasicAuthnClientHandlerTests
    {
        public class TheBasicAuthnClientHandler
        {
            [Fact]
            public async Task ShouldAddBasicCredentialsToTheOutboundRequest()
            {
                // from RFC 2617
                const string username = "Aladdin";
                const string password = "open sesame";
                const string credentials = "QWxhZGRpbjpvcGVuIHNlc2FtZQ==";

                Func<HttpRequestMessage, Task<HttpResponseMessage>> assert = req =>
                    {
                        Assert.True(
                            req.Headers.Authorization.
                                Scheme.Equals("basic", StringComparison.InvariantCultureIgnoreCase));
                        Assert.NotNull(req.Headers.Authorization.Parameter);
                        Assert.Equal(credentials, req.Headers.Authorization.Parameter);
                        return Task.FromResult(new HttpResponseMessage());
                    };
                
                using(var client = new HttpClient(
                    new BasicAuthnClientHandler(req => Task.FromResult(new BasicCredentials(username, password)))
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
