using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Should;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class CreatingIssues : IssuesFeature
    {
        private readonly Uri _issues = new Uri("http://localhost/issue");

        [Scenario]
        public void CreatingANewIssue(dynamic newIssue)
        {
            "Given a new issue".
                f(() =>
                {
                    newIssue = new JObject();
                    newIssue.description = "A new issue";
                    newIssue.title = "NewIssue";
                    MockIssueStore.Setup(i => i.CreateAsync(It.IsAny<Issue>())).Returns<Issue>( issue=>
                        {
                            issue.Id = "1";
                            return Task.FromResult("");
                        });
                });
            "When a POST request is made".
                f(() =>
                {
                    Request.Method = HttpMethod.Post;
                    Request.RequestUri = _issues;
                    Request.Content = new ObjectContent<dynamic>(newIssue, new JsonMediaTypeFormatter());
                    Response = Client.SendAsync(Request).Result;
                });
            "Then the issue should be added".
                f(() => MockIssueStore.Verify(i => i.CreateAsync(It.IsAny<Issue>())));
            "And a '201 Created' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.Created));
            "And the response location header will be set to the resource location".
                f(() => Response.Headers.Location.AbsoluteUri.ShouldEqual("http://localhost/issue/1"));
        }
    }
}
