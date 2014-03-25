using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");
            config.EnableSystemDiagnosticsTracing();
            WebApiConfiguration.Configure(config);
            Trace.Listeners.Add(new ConsoleTraceListener());

            
            
            var host = new HttpSelfHostServer(config);
            host.OpenAsync().Wait();
            Console.WriteLine("IssueApi hosted at: {0}", config.BaseAddress);
            Console.ReadLine();
        }
    }
}
