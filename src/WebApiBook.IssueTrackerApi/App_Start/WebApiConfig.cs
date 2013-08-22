using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json.Serialization;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "IssueProcessor",
                routeTemplate: "issues/{id}/issueprocessor",
                defaults: new { controller="issueprocessor"}
            );

            var builder = new ContainerBuilder();
            
            builder.RegisterApiControllers(typeof(IssueController).Assembly);
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.MessageHandlers.Add(container.Resolve<IssueHypermediaHandler>());
        }
    }

    public class IssueHypermediaHandler : DelegatingHandler
    {
        private readonly IStateFactory<Issue, IssueState> _factory;

        public IssueHypermediaHandler(IStateFactory<Issue, IssueState> factory)
        {
            _factory = factory;
        }

        protected async override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var issue = await response.Content.ReadAsAsync<Issue>();
            var issueState = _factory.Create(issue);
            response.Content = new ObjectContent<IssueState>(issueState, null);
            return response;
        }
    }
}
