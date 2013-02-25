using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

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
                routeTemplate: "issues/{id}/issueprocessor?action={action}",
                defaults: new { controller="issueprocessor"}
            );
        }
    }
}
