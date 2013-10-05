using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Should;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class CreatingIssues : IssuesFeature
    {
        private Uri _issues = new Uri("http://localhost/issue");

        [Scenario]
        public void CreatingANewIssue(Issue issue)
        {
            "Given a new issue".
                f(() =>
                    {
                        issue = new Issue {Description = "A new issue", Title = "A new issue"};
                        issue.Description = "A new issue";
                        issue.Title = "A new issue";
                        MockIssueStore.Setup(i => i.CreateAsync(issue)).Returns(() =>
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
                        Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '201 Created' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.Created));
            "Then the issue should be added".
                f(() => MockIssueStore.Verify(i => i.CreateAsync(issue)));
            "Then the response location header will be set to the resource location".
                f(() => Response.Headers.Location.AbsoluteUri.ShouldEqual("http://localhost/issue/1"));
        }
    }
}
