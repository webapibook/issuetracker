using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;
using Should;
using System.Net.Http;
using Xunit;
using System.Net;
using Moq;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Linq;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class ConflictDetection : IssuesFeature
    {
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void UpdatingAnIssueWithConflict()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();

            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                });
            "When a PATCH request is made with IfModifiedSince".
                f(() =>
                {
                    var issue = new Issue();
                    issue.Title = "Updated title";
                    issue.Description = "Updated description";
                    Request.Method = new HttpMethod("PATCH");
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified.AddDays(1);
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '409 CONFLICT' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.Conflict));
            "Then the issue should be not updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync("1", It.IsAny<JObject>(), It.IsAny<string>()), Times.Never()));
        }

        [Scenario]
        public void UpdatingAnIssueWithNoConflict()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();

            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", It.IsAny<Object>(), It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a PATCH request is made with IfModifiedSince".
                f(() =>
                {
                    var issue = new Issue();
                    issue.Title = "Updated title";
                    issue.Description = "Updated description";
                    Request.Method = new HttpMethod("PATCH");
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue should be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync("1", It.IsAny<JObject>(), It.IsAny<string>())));
        }

        [Scenario]
        public void UpdatingAnIssueWithMissingIfModifiedSinceHeader()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();

            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", It.IsAny<Object>(), It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a PATCH request is made with no IfModifiedSince".
                f(() =>
                {
                    var issue = new Issue();
                    issue.Title = "Updated title";
                    issue.Description = "Updated description";
                    Request.Method = new HttpMethod("PATCH");
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '400 Bad Request' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest));
            "Then the issue should not be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync("1", It.IsAny<JObject>(), It.IsAny<string>()), Times.Never()));
        }
    }
}
