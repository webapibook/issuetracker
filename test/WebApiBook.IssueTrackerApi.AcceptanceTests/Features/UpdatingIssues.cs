using System;
using System.Linq;
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
    public class UpdatingIssues : IssuesFeature
    {
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

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
            "When a PATCH request is made".
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
            "Then the issue should be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync("1", It.IsAny<JObject>(), It.IsAny<string>())));
        }

        [Scenario]
        public void UpdatingAnIssueThatDoesNotExist()
        {
            HttpResponseMessage response = null;

            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a PATCH request is made".
                f(() =>
                    {
                        Request.Method = new HttpMethod("PATCH");
                        Request.RequestUri = _uriIssue1;
                        Request.Content = new ObjectContent<Issue>(new Issue(), new JsonMediaTypeFormatter());
                        response = Client.SendAsync(Request).Result;
                    });
            "Then a 404 Not Found status is reutnred".
                f(() => response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

    }
}
