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
        private Uri _uriIssue1 = new Uri("http://localhost:8080/issue/1");

        [Scenario]
        public void UpdatingAnIssue(Issue fakeIssue, string title)
        {
            "Given an existing issue".
                f(() =>
                    {
                        fakeIssue = FakeIssues.FirstOrDefault();
                        title = fakeIssue.Title;
                        MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                        MockIssueStore.Setup(i => i.UpdateAsync(It.IsAny<Issue>())).Returns(Task.FromResult(""));
                    });
            "When a PATCH request is made".
                f(() =>
                    {
                        dynamic issue = new JObject();
                        issue.description = "Updated description";  
                        Request.Method = new HttpMethod("PATCH");
                        Request.RequestUri = _uriIssue1;
                        Request.Content = new ObjectContent<dynamic>(issue, new JsonMediaTypeFormatter());
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue should be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync(It.IsAny<Issue>())));
            "Then the descripton should be updated".
                f(() => fakeIssue.Description.ShouldEqual("Updated description"));
            "Then the title should not change".
                f(() => fakeIssue.Title.ShouldEqual(title));
        }

        [Scenario]
        public void UpdatingAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a PATCH request is made".
                f(() =>
                    {
                        Request.Method = new HttpMethod("PATCH");
                        Request.RequestUri = _uriIssue1;
                        Request.Content = new ObjectContent<dynamic>(new JObject(), new JsonMediaTypeFormatter());
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a 404 Not Found status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

    }
}
