using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.IntegrationTests.GitHub.SecretCredentials;
using Xunit;

namespace WebApiBook.IssueTrackerApi.IntegrationTests.GitHub
{
    public class GitHubTokenClientTests
    {
        public class TheGitHubTokenClient
        {
            [Fact]
            public async Task ShouldCreateOAuthTokens()
            {
                const string username = TestSecretCredentials.UserName;
                const string password = TestSecretCredentials.Password;
                const string clientId = TestSecretCredentials.ClientId;
                const string clientSecret = TestSecretCredentials.ClientSecret;
                var tokenClient = new GitHubTokenClient(clientId, clientSecret);
                var token = await tokenClient.GetBearerTokenAsync(new BasicCredentials(username, password), new string[] {"repo"});
                try
                {
                    using (
                        var client =
                            new HttpClient(
                                new BearerAuthnClientHandler(req => Task.FromResult(new BearerToken(token.Value)))
                                    {
                                        InnerHandler = new HttpClientHandler()
                                    }))
                    {
                        var httpResp = await client.GetAsync("https://api.github.com/orgs/webapibook/issues");
                        Assert.Equal(HttpStatusCode.OK, httpResp.StatusCode);
                    }
                }finally
                {
                    tokenClient.DeleteBearerTokenAsync(new BasicCredentials(username, password), token.Uri);
                }
            } 
        }
    }
}
