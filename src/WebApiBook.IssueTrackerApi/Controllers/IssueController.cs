using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Controllers
{
    public class IssueController : ApiController
    {
        private readonly IIssueStore _store;
        private readonly IStateFactory<Issue, IssueState> _stateFactory;
        private readonly IssueLinkFactory _linkFactory;

        public IssueController(IIssueStore store, IStateFactory<Issue, IssueState> stateFactory, IssueLinkFactory linkFactory )
        {
            _store = store;
            _stateFactory = stateFactory;
            _linkFactory = linkFactory;
        }

        public async Task<HttpResponseMessage> Get()
        {
            var issues = await _store.FindAsync();
            var issuesState = new IssuesState();
            issuesState.Issues = issues.Select(i => _stateFactory.Create(i));
            issuesState.Links.Add(new Link{Href=Request.RequestUri, Rel = LinkFactory.Rels.Self});
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        } 

        public async Task<HttpResponseMessage> Get(string id)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(issue));
        }

        public async Task<HttpResponseMessage> GetSearch(string searchText)
        {
            var issues = await _store.FindAsyncQuery(searchText);
            var issuesState = new IssuesState();
            issuesState.Issues = issues.Select(i => _stateFactory.Create(i));
            issuesState.Links.Add(new Link { Href = Request.RequestUri, Rel = LinkFactory.Rels.Self });
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        }

        public async Task<HttpResponseMessage> Post(dynamic newIssue)
        {
            var issue = new Issue {Title = newIssue.title, Description = newIssue.description};
            await _store.CreateAsync(issue);
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = _linkFactory.Self(issue.Id).Href;
            return response;
        }

        public async Task<HttpResponseMessage> Patch(string id, dynamic issueUpdate)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            foreach (JProperty prop in issueUpdate)
            {
                if (prop.Name == "title")
                    issue.Title = prop.Value.ToObject<string>();
                else if (prop.Name == "description")
                    issue.Description = prop.Value.ToObject<string>();
            }
            await _store.UpdateAsync(issue);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
      
        public async Task<HttpResponseMessage> Delete(string id)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            await _store.DeleteAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}