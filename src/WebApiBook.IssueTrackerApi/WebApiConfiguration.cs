using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiContrib.Formatting.CollectionJson.Client;

namespace WebApiBook.IssueTrackerApi
{
    public static class WebApiConfiguration
    {
        public static void Configure(HttpConfiguration config, IIssueStore issueStore = null)
        {
            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });
            config.Routes.MapHttpRoute("Home", "", new { controller = "home" });
            ConfigureFormatters(config);
            ConfigureAutofac(config, issueStore);
        }
       
        private static void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.Add(new CollectionJsonFormatter());
            JsonSerializerSettings settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.issue+json"));
        }

        private static void ConfigureAutofac(HttpConfiguration config, IIssueStore issueStore)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(typeof(IssueController).Assembly);

            if (issueStore == null)
                builder.RegisterType<InMemoryIssueStore>().As<IIssueStore>().InstancePerLifetimeScope();
            else
                builder.RegisterInstance(issueStore);

            builder.RegisterType<IssueStateFactory>().As<IStateFactory<Issue, IssueState>>().InstancePerLifetimeScope();
            builder.RegisterType<IssueLinkFactory>().InstancePerLifetimeScope();
            builder.RegisterHttpRequestMessage(config);
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
