using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Moq;
using Should;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class ConflictDetection : IssuesFeature
    {
         private readonly Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void UpdatingAnIssueWithNoConflict()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();
            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                    MockIssueStore.Setup(i => i.UpdateAsync(It.IsAny<Issue>())).Returns(Task.FromResult(""));
                });
            "When a PATCH request is made with IfModifiedSince".
                f(() =>
                {
                    var issue = new Issue {Title = "Updated title", Description = "Updated description"};
                    Request.Method = new HttpMethod("PATCH");
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue should be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync(It.IsAny<Issue>())));
        }

        [Scenario]
        public void UpdatingAnIssueWithConflicts()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();
            "Given an existing issue".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue)));
            "When a PATCH request is made with IfModifiedSince".
                f(() =>
                {
                    var issue = new Issue { Title = "Updated title", Description = "Updated description" };
                    Request.Method = new HttpMethod("PATCH");
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified.AddDays(1);
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '409 Conflict' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.Conflict));
            "Then the issue should not be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync(It.IsAny<Issue>()), Times.Never()));
        }
    }
}