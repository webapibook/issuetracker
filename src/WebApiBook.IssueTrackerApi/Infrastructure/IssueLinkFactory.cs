using System.Net.Http;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class IssueLinkFactory : LinkFactory
    {
        private const string Prefix = "http://webapibook.net/rels#";
        
        public new class Rels : LinkFactory.Rels {
            public const string IssueProcessor = Prefix + "issue-processor";
            public const string SearchQuery = Prefix + "search";
            public const string Issue = Prefix + "issue";
            public const string Issues = Prefix + "issues";
        }
        
        public class Actions {
            public const string Open="open";
            public const string Close="close";
            public const string Transition="transition";
        }
        
        public IssueLinkFactory(HttpRequestMessage request)
            : base(request, "issue")
        {
        }
   
        public Link Transition(string id)
        {
            return new Link { Rel = Rels.IssueProcessor, Action = Actions.Transition, Href = GetUri(new { controller = "issueprocessor", id = id, action = Actions.Transition }) };
        }
        
        public Link Open(string id) {
            return new Link { Rel = Rels.IssueProcessor, Action = Actions.Open, Href = GetUri(new { controller = "issueprocessor", id = id, action = Actions.Open }) };
        }
        
        public Link Close(string id) {
            return new Link { Rel = Rels.IssueProcessor, Action = Actions.Close, Href = GetUri(new { controller = "issueprocessor", id = id, action = Actions.Close }) };
        }
    }
}