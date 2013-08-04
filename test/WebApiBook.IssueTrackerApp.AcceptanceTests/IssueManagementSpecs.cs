using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Should;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiContrib.Testing;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public class IssueManagementSpecs
    {
        private readonly Mock<IIssueStore> _mockIssueStore;
        private readonly IStateFactory<Issue, IssueState> _stateFactory;
        private readonly IssueController _controller;
        private readonly IEnumerable<Issue> _fakeIssues;
        private readonly HttpRequestMessage _request;

        public IssueManagementSpecs()
        {
            _mockIssueStore = new Mock<IIssueStore>();
            _request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/issue/1");
            var issueLinks = new IssueLinkFactory(_request);
            _stateFactory = new IssueStateFactory(issueLinks);
            _controller = new IssueController(_mockIssueStore.Object, _stateFactory);
            _controller.ConfigureForTesting(_request);
            _fakeIssues = GetFakeIssues();
        }

        [Scenario]
        public void RetrievingIssues()
        {
            IEnumerable<IssueState> issues = null;
            "Given existing issues".
                f(() => _mockIssueStore.Setup(i => i.FindAsync()).Returns(Task.FromResult(_fakeIssues)));
            "When all issues are retrieved".
                f(() => issues = _controller.Get().Result);
            "Then they are returned".
                f(() =>
                    {
                        issues.FirstOrDefault(i => i.Id == "1").ShouldNotBeNull();
                        issues.FirstOrDefault(i => i.Id == "2").ShouldNotBeNull();
                    });
        }

        [Scenario]
        public void RetrievingAnIssue()
        {
            IssueState issue = null;
            var fakeIssue = _fakeIssues.FirstOrDefault();
            "Given an existing issue".
                f(() => _mockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() => issue = _controller.Get("1").Result);
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
                        link.Href.ShouldEqual("http://localhost/issue/1");
                    });
            "Then it should have a transition link".
                f(() =>
                    {
                        var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.IssueProcessor && l.Action == IssueLinkFactory.Actions.Transition);
                        link.ShouldNotBeNull();
                        link.Href.ShouldEqual("http://localhost/issueprocessor/1?action=transition");
                    });
        }

        [Scenario]
        public void RetrievingAnOpenIssue()
        {
            var fakeIssue = _fakeIssues.Single(i => i.Id == "1");
            IssueState issue = null;

            "Given an existing open issue".
                f(() => _mockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() => issue = _controller.Get("1").Result);
            "Then it should have a close action link".
                f(() =>
                {
                    var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.IssueProcessor && l.Action == IssueLinkFactory.Actions.Close);
                    link.ShouldNotBeNull();
                    link.Href.ShouldEqual("http://localhost/issueprocessor/1?action=close");
                });
        }

        [Scenario]
        public void RetrievingAClosedIssue()
        {
            _request.RequestUri = new Uri("http://localhost/issue/2");
            var fakeIssue = _fakeIssues.Single(i => i.Id == "2");
            IssueState issue = null;

            "Given an existing closed issue".
                f(() => _mockIssueStore.Setup(i => i.FindAsync("2")).Returns(Task.FromResult(fakeIssue)));
            "When it is retrieved".
                f(() => issue = _controller.Get("2").Result);
            "Then it should have an close action link".
                f(() =>
                {
                    var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.IssueProcessor && l.Action == IssueLinkFactory.Actions.Open);
                    link.ShouldNotBeNull();
                    link.Href.ShouldEqual("http://localhost/issueprocessor/2?action=open");

                });
        }

        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>();
            fakeIssues.Add(new Issue { Id = "1", Title="An issue", Description="This is an issue", Status=IssueStatus.Open });
            fakeIssues.Add(new Issue { Id = "2", Title = "Another issue", Description = "This is another issue", Status = IssueStatus.Closed });
            return fakeIssues;
        } 
    }
}
