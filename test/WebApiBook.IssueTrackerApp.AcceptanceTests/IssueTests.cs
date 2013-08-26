using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiContrib.Testing;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class IssueTests
    {
        public Mock<IIssueStore> MockIssueStore;
        public HttpRequestMessage Request;
        public IssueLinkFactory IssueLinks;
        public IssueStateFactory StateFactory;
        public IssueController Controller;
        public IEnumerable<Issue> FakeIssues;

        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>();
            fakeIssues.Add(new Issue { Id = "1", Title = "An issue", Description = "This is an issue", Status = IssueStatus.Open });
            fakeIssues.Add(new Issue { Id = "2", Title = "Another issue", Description = "This is another issue", Status = IssueStatus.Closed });
            return fakeIssues;
        }

        public IssueTests()
        {
            MockIssueStore = new Mock<IIssueStore>();
            Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/issue/1");
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            Controller = new IssueController(MockIssueStore.Object, StateFactory);
            Controller.ConfigureForTesting(Request);
            FakeIssues = GetFakeIssues();
        }
    }
}
