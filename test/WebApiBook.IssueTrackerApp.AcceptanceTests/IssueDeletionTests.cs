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
    public class IssueDeletionTests : IssueTests
    {
        public IssueDeletionTests()
        {
            Request.Method = HttpMethod.Delete;
        }

        [Scenario]
        public void DeletingAnIssue()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();
            HttpResponseMessage response = null;

            "Given an existing issue".
                f(() =>
                    {
                        MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                        MockIssueStore.Setup(i => i.DeleteAsync("1")).Returns(Task.FromResult(""));
                    });
            "When a DELETE request is made".
                f(() => response = Controller.Delete("1").Result);
            "Then the issue should be removed".
                f(() => MockIssueStore.Verify(i => i.DeleteAsync("1")));
            "Then a '200 OK status' is returned".
                f(() => response.StatusCode.ShouldEqual(HttpStatusCode.OK));
        }

        [Scenario]
        public void DeletingAnIssueThatDoesNotExist()
        {
            HttpResponseMessage response = null;

            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue) null)));
            "When a DELETE request is made".
                f(() => response = Controller.Delete("1").Result);
            "Then a '404 Not Found' status is returned".
                f(() => response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }
    }
}
