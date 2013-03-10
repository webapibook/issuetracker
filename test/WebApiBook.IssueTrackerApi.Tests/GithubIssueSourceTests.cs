using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiBook.IssueTrackerApi.Tests.Handlers;
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
            private IEnumerable<Issue> _issues;
            
            public TheFindAsyncMethod()
            {
                _source = new GithubIssueSource("https://api.github.com/repos/webapibook/issuetracker/issues", "milestone=1", _handler);
                _issues = _source.FindAsync().Result;
 
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
                issue.Links[1].Rel.ShouldEqual("http://rels.webapi.book.net#created-by");
                issue.Links[1].Rt.ShouldEqual("creator");
            }

            [Fact]
            public void ShouldPropertySetActionLink()
            {
                var issue = _issues.First();
                issue.Actions[0].Action.ShouldEqual("close");
                issue.Actions[0].Rel.ShouldEqual("http://rels.webapibook.net#close");
                issue.Actions[0].Href.ShouldEqual("/issues/3/issueprocessor?action=close");
                issue.Actions[0].Rt.ShouldEqual("transition");
            }
        }

    }
}
