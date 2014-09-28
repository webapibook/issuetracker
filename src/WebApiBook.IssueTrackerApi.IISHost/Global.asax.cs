using System;
using System.Web.Http;

namespace WebApiBook.IssueTrackerApi.IISHost
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            WebApiConfiguration.Configure(GlobalConfiguration.Configuration);
        }
    }
}