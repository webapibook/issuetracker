using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class HomeFeature
    {
        public HttpClient Client;
        public HttpRequestMessage Request;
        public HttpResponseMessage Response;

        public HomeFeature()
        {
            Request = new HttpRequestMessage();

            var config = new HttpConfiguration();
            WebApiBook.IssueTrackerApi.WebApiConfiguration.Configure(config);
            var server = new HttpServer(config);
            Client = new HttpClient(server);
        }


    }
}
