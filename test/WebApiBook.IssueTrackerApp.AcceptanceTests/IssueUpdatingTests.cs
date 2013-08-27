using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Should;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public class IssueUpdatingTests : IssueControllerTests
    {
        protected override HttpRequestMessage GetRequest()
        {
            var request = base.GetRequest();
            request.Method = new HttpMethod("PATCH");
            return request;
        }

        [Scenario]
        public void UpdatingAnIssue()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();
            JObject issue = null;
            HttpResponseMessage response = null;

            "Given an existing issue".
                f(() =>
                    {
                        MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                        MockIssueStore.Setup(i => i.UpdateAsync("1", It.IsAny<Object>())).Returns(Task.FromResult(""));
                    });
            "When a PATCH request is made".
                f(() =>
                    {
                        issue = new JObject();
                        issue["Title"] = "Updated title";
                        issue["Description"] = "Updated description";                        
                        response = Controller.Patch("1", issue).Result;
                    });
            "Then a '200 OK' status is returned".
                f(() => response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue should be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync("1", issue)));
        }

        [Scenario]
        public void UpdatingAnIssueThatDoesNotExist()
        {
            HttpResponseMessage response = null;

            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a PATCH request is made".
                f(() => response = Controller.Patch("1", null).Result);
            "Then a 404 Not Found status is reutnred".
                f(() => response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

    }
}
