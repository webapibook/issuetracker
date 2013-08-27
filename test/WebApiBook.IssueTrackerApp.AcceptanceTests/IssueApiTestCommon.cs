using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Autofac.Integration.WebApi;
using Moq;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiContrib.Testing;
using Autofac;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class IssueApiTestCommon
    {
        public Mock<IIssueStore> MockIssueStore;
        public HttpResponseMessage Response;
        public IssueLinkFactory IssueLinks;
        public IssueStateFactory StateFactory;
        public IEnumerable<Issue> FakeIssues;
        public HttpRequestMessage Request { get; private set; }
        public HttpClient Client;

        public IssueApiTestCommon()
        {
            MockIssueStore = new Mock<IIssueStore>();
            Request = new HttpRequestMessage();
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            FakeIssues = GetFakeIssues();
            var server = new HttpServer(GetConfiguration());
            Client = new HttpClient(server);
        }

        private HttpConfiguration GetConfiguration()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(typeof(IssueController).Assembly);
            builder.RegisterInstance(MockIssueStore.Object).As<IIssueStore>();
            builder.RegisterType<IssueStateFactory>().As<IStateFactory<Issue, IssueState>>().InstancePerLifetimeScope();
            builder.RegisterType<IssueLinkFactory>().InstancePerLifetimeScope();
            builder.RegisterInstance(Request);
            var container = builder.Build();
            //var controller = container.Resolve<IssueController>();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return config;
        }

        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>();
            fakeIssues.Add(new Issue { Id = "1", Title = "An issue", Description = "This is an issue", Status = IssueStatus.Open });
            fakeIssues.Add(new Issue { Id = "2", Title = "Another issue", Description = "This is another issue", Status = IssueStatus.Closed });
            return fakeIssues;
        }
    }
}