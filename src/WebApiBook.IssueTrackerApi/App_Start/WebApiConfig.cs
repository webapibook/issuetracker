using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json.Serialization;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;

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
/*
#if USE_FAKE_GITHUB_SOURCE
            builder.Register(
                c => new GithubIssueSource("https://api.github.com/repos/webapibook/issuetracker/issues", "milestone=1", new FakeGithubHandler())).As<IIssueStore>();
#else
            builder.Register(
                c => new GithubIssueSource("https://api.github.com/repos/webapibook/issuetracker/issues", "milestone=1")).As<IIssueSource>();
#endif
 * */

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
