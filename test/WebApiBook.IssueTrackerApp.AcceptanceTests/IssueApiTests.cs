using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Moq;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiContrib.Testing;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class IssueApiTests<TController> where TController : ApiController
    {
        public Mock<IIssueStore> MockIssueStore;
        public HttpResponseMessage Response;
        public IssueLinkFactory IssueLinks;
        public IssueStateFactory StateFactory;
        public IEnumerable<Issue> FakeIssues;
        public TController Controller { get; private set; }
        public HttpRequestMessage Request { get; private set; }

        public IssueApiTests()
        {
            MockIssueStore = new Mock<IIssueStore>();
            Request = GetRequest();
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            Controller = GetController();
            Controller.ConfigureForTesting(Request);
            FakeIssues = GetFakeIssues();
        }

        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>();
            fakeIssues.Add(new Issue { Id = "1", Title = "An issue", Description = "This is an issue", Status = IssueStatus.Open });
            fakeIssues.Add(new Issue { Id = "2", Title = "Another issue", Description = "This is another issue", Status = IssueStatus.Closed });
            return fakeIssues;
        }

        protected abstract TController GetController();

        protected abstract HttpRequestMessage GetRequest();

    }
}