using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.IntegrationTests.GitHub.SecretCredentials;
using Xunit;

namespace WebApiBook.IssueTrackerApi.IntegrationTests.GitHub
{
    public class BasicAuthnClientHandlerTests
    {
        public class TheBasicAuthnClientHandler
        {
            [Fact]
            public async Task ShouldAuthenticateOutboundGitHubRequests()
            {
                const string username = TestSecretCredentials.UserName;
                const string password = TestSecretCredentials.Password;
                using(var client = new HttpClient(
                    new BasicAuthnClientHandler(req => Task.FromResult(new BasicCredentials(username, password)))
                                                      {
                                                          InnerHandler = new HttpClientHandler()
                                                      }))
                {
                    var resp = await client.GetAsync("https://api.github.com/orgs/webapibook/issues");
                    Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
                }
            }
        }
    }
}
