using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tavis;
using Tavis.Home;

namespace IssueTrackerConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var exploreMission = new ExploreMission(new HttpClient());

            
            var link = new Link() {Target = new Uri("http://localhost:8080/")};
            string input = null;
            while (input != "exit")
            {
                 
                if (link != null)
                {
                    exploreMission.GoAsync(link).Wait();
                    Console.WriteLine(exploreMission.CurrentRepresentation.ReadAsStringAsync().Result);
                }
                {
                    Console.WriteLine("Unknown link");
                }
                Console.WriteLine("----");
                Console.Write("Where to next : ");
                input = Console.ReadLine();
                link = exploreMission.AvailableLinks[input];
            }
        }
    }

    public class ExploreMission
    {
        private readonly HttpClient _httpClient;
        
        public Link ContextLink { get; set; }
        public HttpContent CurrentRepresentation { get; set; }
        public Dictionary<string,Link> AvailableLinks { get; set; }

        public ExploreMission(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GoAsync(Link link)
        {
            var response = await _httpClient.SendAsync(link.CreateRequest());
            if (response.IsSuccessStatusCode)
            {
                ContextLink = link;
                CurrentRepresentation = response.Content;
                AvailableLinks = ParseLinks(CurrentRepresentation);
            }
        }
        
        private Dictionary<string,Link> ParseLinks(HttpContent currentRepresentation)
        {
            switch (currentRepresentation.Headers.ContentType.MediaType)
            {
                case "application/home+json":
                    var home = HomeDocument.Parse(currentRepresentation.ReadAsStreamAsync().Result);
                    return home.Resources.ToDictionary(k => k.Relation, c => { c.Target = new Uri(ContextLink.Target,c.Target); return c; });
            }
            return new Dictionary<string, Link>();
        }
    }
}
