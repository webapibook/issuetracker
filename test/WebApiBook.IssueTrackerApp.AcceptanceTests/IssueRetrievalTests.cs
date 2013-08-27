using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Moq;
using Should;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiContrib.Testing;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public class IssueRetrievalTests : IssueTests
    {
        [Scenario]
        public void RetrievingIssues()
        {
            IssuesState issuesState = null;

            "Given existing issues".
                f(() => MockIssueStore.Setup(i => i.FindAsync()).Returns(Task.FromResult(FakeIssues)));
            "When all issues are retrieved".
                f(() =>
                    {
                        Response = Controller.Get().Result;
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
                        Response = Controller.Get("1").Result;
                        var issuesState = Response.Content.ReadAsAsync<IssuesState>().Result;
                        issue = issuesState.Issues.FirstOrDefault();
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
            "Then it should have a self link".
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
            var fakeIssue = FakeIssues.Single(i => i.Id == "1");
            IssueState issue = null;

            "Given an existing open issue".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() =>
                    {
                        var issuesState = Controller.Get("1").Result.Content.ReadAsAsync<IssuesState>().Result;
                        issue = issuesState.Issues.FirstOrDefault();
                    });
            "Then it should have a close action link".
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
            Request.RequestUri = new Uri("http://localhost/issue/2");
            var fakeIssue = FakeIssues.Single(i => i.Id == "2");
            IssueState issue = null;

            "Given an existing closed issue".
                f(() => MockIssueStore.Setup(i => i.FindAsync("2")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() =>
                    {
                        var issuesState = Controller.Get("2").Result.Content.ReadAsAsync<IssuesState>().Result;
                        issue = issuesState.Issues.FirstOrDefault();
                    });
            "Then it should have an close action link".
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
            HttpResponseMessage response = null;

            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When it is retrieved".
                f(() => response = Controller.Get("1").Result);
            "Then a '404 Not Found' status is returned".
                f(() => response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

    }
}
