using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;
using Should;
using Newtonsoft.Json.Linq;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class ChangeAuditing : IssuesFeature
    {
        private Uri _issues = new Uri("http://localhost/issue");
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void CreatingANewIssue()
        {
            Issue issue = null;

            "Given a new issue".
                f(() =>
                {
                    issue = new Issue { Description = "A new issue", Title = "A new issue" };
                    var newIssue = new Issue { Id = "1" };
                    MockIssueStore.Setup(i => i.CreateAsync(issue, "test")).Returns(Task.FromResult(newIssue));
                });
            "When a POST request is made with an Authorization header containing the user identifier".
                f(() =>
                {
                    Request.Method = HttpMethod.Post;
                    Request.RequestUri = _issues;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '201 Created' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.Created));
            "Then the issue should be added with auditing information".
                f(() => MockIssueStore.Verify(i => i.CreateAsync(issue, "test")));
            "Then the response location header will be set to the resource location".
                f(() => Response.Headers.Location.AbsoluteUri.ShouldEqual("http://localhost/issue/1"));
        }

        [Scenario]
        public void UpdatingAnIssue()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();

            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", It.IsAny<Object>(), It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a PATCH request is made with an Authorization header containing the user identifier".
                f(() =>
                {
                    var issue = new Issue();
                    issue.Title = "Updated title";
                    issue.Description = "Updated description";
                    Request.Method = new HttpMethod("PATCH");
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified;
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue should be updated with auditing information".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync("1", It.IsAny<JObject>(), "test")));
        }
    }
}
