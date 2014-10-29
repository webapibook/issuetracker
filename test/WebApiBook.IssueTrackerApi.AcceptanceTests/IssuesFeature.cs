using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Moq;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class IssuesFeature
    {
        public Mock<IIssueStore> MockIssueStore;
        public HttpResponseMessage Response;
        public IssueLinkFactory IssueLinks;
        public IssueStateFactory StateFactory;
        public IEnumerable<Issue> FakeIssues;
        public HttpRequestMessage Request { get; private set; }
        public HttpClient Client;
        public IssuesFeature()
        {
            MockIssueStore = new Mock<IIssueStore>();
            Request = new HttpRequestMessage();
            Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.issue+json"));
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            FakeIssues = GetFakeIssues();
            var config = new HttpConfiguration();
            WebApiBook.IssueTrackerApi.WebApiConfiguration.Configure(config, MockIssueStore.Object);
            var server = new HttpServer(config);
            Client = new HttpClient(server);
        }

        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>
            {
                new Issue
                {
                    Description = "This is an issue",
                    Id = "1",
                    Status = IssueStatus.Open,
                    Title = "An issue",
                    LastModified = new DateTimeOffset(new DateTime(2013, 9, 4))
                },
                new Issue
                {
                    Description = "This is a another issue",
                    Id = "2",
                    Status = IssueStatus.Closed,
                    Title = "Another Issue",
                    LastModified = new DateTimeOffset(new DateTime(2014, 8, 22))
                }
            };

            return fakeIssues;
        }
    }
}