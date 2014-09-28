using System;
using System.Web.Http.SelfHost;

namespace WebApiBook.IssueTrackerApi.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");
            WebApiConfiguration.Configure(config);
            var host = new HttpSelfHostServer(config);
            host.OpenAsync().Wait();
            Console.WriteLine("IssueApi hosted at: {0}", config.BaseAddress);
            Console.ReadLine();
        }
    }
}
