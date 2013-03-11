using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class BasicAuthnServerHandler : DelegatingHandler
    {
        private readonly string _realm;
        private readonly Func<BasicCredentials, Task<bool>> _val;
        private readonly Func<HttpRequestMessage, Task<BasicCredentials>> _prov;

        public BasicAuthnServerHandler(string realm, Func<BasicCredentials, Task<bool>> val)
        {
            _realm = realm;
            _val = val;
        }

        protected override async Task<HttpResponseMessage>
            SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            BasicCredentials creds;
            if (request.Headers.Authorization != null
                && request.Headers.Authorization.Scheme.Equals("Basic", StringComparison.InvariantCultureIgnoreCase)
                && (creds = BasicCredentials.TryParse(request.Headers.Authorization.Parameter)) != null)
            {
                if(!await _val(creds))
                {
                    return UnAuthorizedResponseMessage();
                }
                SetPrincipal(request, creds);
                return await base.SendAsync(request, cancellationToken);
            }
            var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            resp.Headers.WwwAuthenticate.Add(
                new AuthenticationHeaderValue("Basic", string.Format("realm={0}", _realm)));
            return resp;
        }

        private static void SetPrincipal(HttpRequestMessage request, BasicCredentials creds)
        {
            var princ = new ClaimsPrincipal(new ClaimsIdentity(
                                                new Claim[] {new Claim(ClaimTypes.NameIdentifier, creds.Username),}
                                                ));
            Thread.CurrentPrincipal = princ;
            if(HttpContext.Current != null)
            {
                HttpContext.Current.User = princ;
            }
        }

        private HttpResponseMessage UnAuthorizedResponseMessage()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            resp.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic",string.Format("realm= {0}",_realm)));
            return resp;
        }
    }
}