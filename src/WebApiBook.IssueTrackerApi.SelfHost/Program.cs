using System;
using Microsoft.Owin.Hosting;


namespace WebApiBook.IssueTrackerApi.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:8080/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("IssueApi hosted at: {0}", baseAddress);
                Console.ReadLine();
            }
        }
    }
}
