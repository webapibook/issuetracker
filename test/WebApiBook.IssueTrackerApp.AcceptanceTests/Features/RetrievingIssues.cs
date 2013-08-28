using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Should;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class RetrievingIssues : IssuesFeature
    {
        private Uri _uriIssues = new Uri("http://localhost/issue");
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");
        private Uri _uriIssue2 = new Uri("http://localhost/issue/2");
 
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
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then it is returned".
                f(() => issue.ShouldNotBeNull());
            "Then it should have an id".
                f(() => issue.Id.ShouldEqual(fakeIssue.Id));
            "Then it should have a title".
                f(() => issue.Title.ShouldEqual(fakeIssue.Title));
            "Then it should have a description".
                f(() => issue.Description.ShouldEqual(fakeIssue.Description));
            "Then it should have a state".
                f(() => issue.Status.ShouldEqual(fakeIssue.Status));
            "Then it should have a 'self' link".
                f(() =>
                    {
                        var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.Self);
                        link.ShouldNotBeNull();
                        link.Href.AbsoluteUri.ShouldEqual("http://localhost/issue/1");
                    });
            "Then it should have a transition link".
                f(() =>
                    {
                        var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.IssueProcessor && l.Action == IssueLinkFactory.Actions.Transition);
                        link.ShouldNotBeNull();
                        link.Href.AbsoluteUri.ShouldEqual("http://localhost/issueprocessor/1?action=transition");
                    });
        }

        [Scenario]
        public void RetrievingAnOpenIssue()
        {
            var fakeIssue = FakeIssues.Single(i => i.Status == IssueStatus.Open);
            IssueState issue = null;

            "Given an existing open issue".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() =>
                    {
                        Request.RequestUri = _uriIssue1;
                        issue = Client.SendAsync(Request).Result.Content.ReadAsAsync<IssueState>().Result;
                    });
            "Then it should have a 'close' action link".
                f(() =>
                    {
                        var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.IssueProcessor && l.Action == IssueLinkFactory.Actions.Close);
                        link.ShouldNotBeNull();
                        link.Href.AbsoluteUri.ShouldEqual("http://localhost/issueprocessor/1?action=close");
                    });
        }

        [Scenario]
        public void RetrievingAClosedIssue()
        {
            var fakeIssue = FakeIssues.Single(i => i.Status == IssueStatus.Closed);
            IssueState issue = null;

            "Given an existing closed issue".
                f(() => MockIssueStore.Setup(i => i.FindAsync("2")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() =>
                    {
                        Request.RequestUri = _uriIssue2;
                        issue = Client.SendAsync(Request).Result.Content.ReadAsAsync<IssueState>().Result;
                    });
            "Then it should have a 'open' action link".
                f(() =>
                    {
                        var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.IssueProcessor && l.Action == IssueLinkFactory.Actions.Open);
                        link.ShouldNotBeNull();
                        link.Href.AbsoluteUri.ShouldEqual("http://localhost/issueprocessor/2?action=open");

                    });
        }

        [Scenario]
        public void RetrievingAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When it is retrieved".
                f(() =>
                    {
                        Request.RequestUri = _uriIssue1;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

    }
}
