using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Autofac.Integration.WebApi;
using Moq;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using Autofac;
using System;
using HawkNet;
using HawkNet.WebApi;
using System.Web.Http.Dispatcher;
using System.Web.Http.Tracing;

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
        public HawkCredential Credentials;
        public HttpConfiguration Configuration;
        public Mock<ITraceWriter> MockTracer;

        public IssuesFeature()
        {
            MockIssueStore = new Mock<IIssueStore>();
            Request = new HttpRequestMessage();
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            FakeIssues = GetFakeIssues();

            Credentials = new HawkCredential
            {
                Id = "TestClient",
                Algorithm = "hmacsha256",
                Key = "werxhqb98rpaxn39848xrunpaw3489ruxnpa98w4rxn",
                User = "test"
            };

            Configuration = GetConfiguration();

            var server = new HttpServer(Configuration);
            Client = new HttpClient(new HawkClientMessageHandler(server, Credentials));
        }

        private HttpConfiguration GetConfiguration()
        {
            var config = new HttpConfiguration();

            var serverHandler = new HawkMessageHandler(new HttpControllerDispatcher(config), (id) => Credentials);

            MockTracer = new Mock<ITraceWriter>(MockBehavior.Loose);

            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional }, null, serverHandler);
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(typeof(IssueController).Assembly);
            builder.RegisterInstance(MockIssueStore.Object).As<IIssueStore>();
            builder.RegisterType<IssueStateFactory>().As<IStateFactory<Issue, IssueState>>().InstancePerLifetimeScope();
            builder.RegisterType<IssueLinkFactory>().InstancePerLifetimeScope();
            builder.RegisterHttpRequestMessage(config);
            builder.RegisterInstance(MockTracer.Object).As<ITraceWriter>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.Services.Replace(typeof(ITraceWriter), MockTracer.Object);
            
            return config;
        }

        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>();
            fakeIssues.Add(new Issue { Id = "1", Title = "An issue", Description = "This is an issue", Status = IssueStatus.Open, LastModified = new DateTime(2013, 9, 4)});
            fakeIssues.Add(new Issue { Id = "2", Title = "Another issue", Description = "This is another issue", Status = IssueStatus.Closed, LastModified = new DateTime(2013, 9, 2) });
            return fakeIssues;
        }
    }
}