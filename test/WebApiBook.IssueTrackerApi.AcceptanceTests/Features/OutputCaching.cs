using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;
using Should;
using System.Net;
using System.Net.Http;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class OutputCaching : IssuesFeature
    {
        private Uri _uriIssues = new Uri("http://localhost/issue");
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void RetrievingAllIssues()
        {
            IssuesState issuesState = null;

            "Given existing issues".
                f(() => MockIssueStore.Setup(i => i.FindAsync()).Returns(Task.FromResult(FakeIssues)));
            "When all issues are retrieved".
                f(() =>
                {
                    Request.RequestUri = _uriIssues;
                    Response = Client.SendAsync(Request).Result;
                    issuesState = Response.Content.ReadAsAsync<IssuesState>().Result;
                });
            "Then a CacheControl header is returned".
                f(() =>
                    {
                        Response.Headers.CacheControl.Public.ShouldBeTrue();
                        Response.Headers.CacheControl.MaxAge.ShouldEqual(TimeSpan.FromMinutes(5));
                    });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then they are returned".
                f(() =>
                {
                    issuesState.Issues.FirstOrDefault(i => i.Id == "1").ShouldNotBeNull();
                    issuesState.Issues.FirstOrDefault(i => i.Id == "2").ShouldNotBeNull();
                });
        }

        [Scenario]
        public void RetrievingAnIssue()
        {
            IssueState issue = null;

            var fakeIssue = FakeIssues.FirstOrDefault();
            "Given an existing issue".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() =>
                {
                    Request.RequestUri = _uriIssue1;
                    Response = Client.SendAsync(Request).Result;
                    issue = Response.Content.ReadAsAsync<IssueState>().Result;
                });
            "Then a LastModified header is returned".
                f(() =>
                {
                    Response.Content.Headers.LastModified.ShouldEqual(new DateTimeOffset(new DateTime(2013, 9, 4)));
                });
            "Then a CacheControl header is returned".
                f(() =>
                {
                    Response.Headers.CacheControl.Public.ShouldBeTrue();
                    Response.Headers.CacheControl.MaxAge.ShouldEqual(TimeSpan.FromMinutes(5));
                });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then it is returned".
                f(() => issue.ShouldNotBeNull());
        }
    }
}
