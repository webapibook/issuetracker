using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApiBook.IssueTrackerApi.Tests.Handlers
{
    public class FakeGithubHandler : HttpMessageHandler
    {
        public JArray Issues { get; private set; }

        public FakeGithubHandler()
        {
            var assembly = typeof (FakeGithubHandler).Assembly;
            var reader =
                new StreamReader(assembly.GetManifestResourceStream("WebApiBook.IssueTrackerApi.Tests.GithubIssues.json"));
            var jsonReader = new JsonTextReader(reader);
            Issues = JArray.Load(jsonReader);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            JArray issues = null;

            if (request.RequestUri.AbsolutePath.EndsWith("issues"))
            {
                issues = GetAllIssues();
            }

            var response = GetResponse();
            response.Content = new ObjectContent<JArray>(issues, new JsonMediaTypeFormatter());
            return Task.FromResult(response);
        }

        private HttpResponseMessage GetResponse()
        {
            var response = new HttpResponseMessage();
            response.Headers.TryAddWithoutValidation("Server", "Github.com");
            response.Headers.Connection.Add("keep-alive");
            response.Headers.TryAddWithoutValidation("X-Github-Media-Type", "github.beta; param=3.text; format=json");
            response.Headers.Date = new DateTimeOffset(DateTime.Now);
            return response;
        }

        private JArray GetAllIssues()
        {
            return Issues;
        }
    }
}
