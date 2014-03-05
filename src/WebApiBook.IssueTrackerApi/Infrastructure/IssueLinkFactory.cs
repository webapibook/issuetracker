using System.Net.Http;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class IssueLinkFactory : LinkFactory<IssueController>
    {
        private const string Prefix = "http://webapibook.net/profile#";
        
        public new class Rels : LinkFactory.Rels {
            public const string IssueProcessor = Prefix + "issue-processor";
            public const string SearchQuery = Prefix + "search";
        }
        
        public class Actions {
            public const string Open="open";
            public const string Close="close";
            public const string Transition="transition";
        }
        
        public IssueLinkFactory(HttpRequestMessage request)
            : base(request)
        {
        }
   
        public Link Transition(string id)
        {
            return GetLink<IssueProcessorController>(Rels.IssueProcessor, id, Actions.Transition);
        }
        
        public Link Open(string id)
        {
            return GetLink<IssueProcessorController>(Rels.IssueProcessor, id, Actions.Open);
        }
        
        public Link Close(string id)
        {
            return GetLink<IssueProcessorController>(Rels.IssueProcessor, id, Actions.Close);
        }
    }
}