using System.Web.Http;
using Owin;

namespace WebApiBook.IssueTrackerApi.SelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            WebApiConfiguration.Configure(config);
            appBuilder.UseWebApi(config);
        }
    }
}