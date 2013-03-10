using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Fakes.Handlers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using Xunit;
using Should;


namespace WebApiBook.IssueTrackerApi.Tests
{
    public class GithubIssueSourceTests
    {
        public class TheFindAsyncMethod
        {
            private GithubIssueSource _source;
            private FakeGithubHandler _handler = new FakeGithubHandler();
            private Issue[] _issues;
            
            public TheFindAsyncMethod()
            {
                _source = new GithubIssueSource("https://api.github.com/repos/webapibook/issuetracker/issues", "milestone=1", _handler);
                _issues = _source.FindAsync().Result.ToArray();
 
            }

            [Fact]
            public void ShouldRetrieveAllIssuesWhenFindAsyncIsInvoked()
            {
                _issues.Count().ShouldEqual(3);
            }

            [Fact]
            public void ShouldProperlyPopulateFirstIssueDetails()
            {
                var issue = _issues.First();
                issue.Title.ShouldEqual("Dummy feature 2");
                issue.State.ShouldEqual("open");
                issue.Id.ShouldEqual("3");
                issue.Description.ShouldEqual("Lorem ipsum feature");
                issue.Href.ShouldEqual("/issues/3");
            }

            [Fact]
            public void ShouldProperlyPopulateFirstIssueLinks()
            {
                var issue = _issues.First();
                issue.Links[0].Href.ShouldEqual("https://api.github.com/repos/webapibook/issuetracker/issues/3");
                issue.Links[0].Rel.ShouldEqual("http://rels.webapibook.net#issue-source-url");
                issue.Links[0].Rt.ShouldEqual("issue source");
                issue.Links[1].Href.ShouldEqual("https://api.github.com/users/glennblock");
                issue.Links[1].Rel.ShouldEqual("http://rels.webapibook.net#created-by");
                issue.Links[1].Rt.ShouldEqual("creator");
            }

            [Fact]
            public void ShouldProperlySetFirstIssueActionLink()
            {
                var issue = _issues[0];
                issue.Actions[0].Action.ShouldEqual("close");
                issue.Actions[0].Rel.ShouldEqual("http://rels.webapibook.net#close");
                issue.Actions[0].Href.ShouldEqual("/issues/3/issueprocessor?action=close");
                issue.Actions[0].Rt.ShouldEqual("transition");
            }
            [Fact]
            public void ShouldProperlyPopulateSecondIssueDetails()
            {
                var issue = _issues[1];
                issue.Title.ShouldEqual("Dummy feature 1");
                issue.State.ShouldEqual("open");
                issue.Id.ShouldEqual("2");
                issue.Description.ShouldEqual("Lorem ipsum feature");
                issue.Href.ShouldEqual("/issues/2");
            }

            [Fact]
            public void ShouldProperlyPopulateSecondIssueLinks()
            {
                var issue = _issues[1];
                issue.Links[0].Href.ShouldEqual("https://api.github.com/repos/webapibook/issuetracker/issues/2");
                issue.Links[0].Rel.ShouldEqual("http://rels.webapibook.net#issue-source-url");
                issue.Links[0].Rt.ShouldEqual("issue source");
                issue.Links[1].Href.ShouldEqual("https://api.github.com/users/glennblock");
                issue.Links[1].Rel.ShouldEqual("http://rels.webapibook.net#created-by");
                issue.Links[1].Rt.ShouldEqual("creator");
                issue.Links[2].Href.ShouldEqual("https://api.github.com/users/darrelmiller");
                issue.Links[2].Rel.ShouldEqual("http://rels.webapibook.net#assigned-to");
                issue.Links[2].Rt.ShouldEqual("assignee");
            }

            [Fact]
            public void ShouldProperlySetSecondIssueActionLink()
            {
                var issue = _issues[1];
                issue.Actions[0].Action.ShouldEqual("close");
                issue.Actions[0].Rel.ShouldEqual("http://rels.webapibook.net#close");
                issue.Actions[0].Href.ShouldEqual("/issues/2/issueprocessor?action=close");
                issue.Actions[0].Rt.ShouldEqual("transition");
            }

        }

    }
}
