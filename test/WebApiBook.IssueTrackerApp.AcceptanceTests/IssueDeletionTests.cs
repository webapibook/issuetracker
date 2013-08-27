using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Should;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public class IssueDeletionTests : IssueApiTestCommon
    {
        private Uri _uriIssue = new Uri("http://localhost/issue/1");

        [Scenario]
        public void DeletingAnIssue()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();

            "Given an existing issue".
                f(() =>
                    {
                        MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                        MockIssueStore.Setup(i => i.DeleteAsync("1")).Returns(Task.FromResult(""));
                    });
            "When a DELETE request is made".
                f(() =>
                    {
                        Request.RequestUri = _uriIssue;
                        Request.Method = HttpMethod.Delete;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then the issue should be removed".
                f(() => MockIssueStore.Verify(i => i.DeleteAsync("1")));
            "Then a '200 OK status' is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
        }

        [Scenario]
        public void DeletingAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue) null)));
            "When a DELETE request is made".
                f(() =>
                    {
                        Request.RequestUri = _uriIssue;
                        Request.Method = HttpMethod.Delete;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }
    }
}
