using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class GitHubTokenClient
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        
        public GitHubTokenClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<GitHubToken> GetBearerTokenAsync(BasicCredentials creds, string[] scopes)
        {
            using(var client = new HttpClient(new BasicAuthnClientHandler(req => Task.FromResult(creds))
                                                      {
                                                          InnerHandler = new HttpClientHandler()
                                                      }))
            {
                var resp = await client.PostAsJsonAsync("https://api.github.com/authorizations", new TokenRequestModel
                                                                                    {
                                                                                        scopes = scopes,
                                                                                        client_id = _clientId,
                                                                                        client_secret = _clientSecret
                                                                                    });
                resp.EnsureSuccessStatusCode();
                var respModel = await resp.Content.ReadAsAsync<TokenResponseModel>();
                return new GitHubToken(respModel.token, respModel.url);
            }
        }

        public async Task<HttpStatusCode> DeleteBearerTokenAsync(BasicCredentials creds, string url)
        {
            using (var client = new HttpClient(new BasicAuthnClientHandler(req => Task.FromResult(creds))
            {
                InnerHandler = new HttpClientHandler()
            }))
            {
                return (await client.DeleteAsync(url)).StatusCode;
            }   
        }
    }

    public class GitHubToken
    {
        public string Value { get; private set; }
        public string Uri { get; private set; }

        public GitHubToken(string value, string uri)
        {
            Value = value;
            Uri = uri;
        }
    }

    public class TokenRequestModel
    {
        public string[] scopes { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class TokenResponseModel
    {
        public int id { get; set; }
        public string url { get; set; }
        public string token { get; set; }
    }
}