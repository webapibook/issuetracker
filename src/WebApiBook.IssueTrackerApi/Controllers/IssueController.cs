using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Controllers
{
    public class IssueController : ApiController
    {
        private readonly IIssueStore _store;
        private readonly IStateFactory<Issue, IssueState> _stateFactory;

        public IssueController(IIssueStore store, IStateFactory<Issue, IssueState> stateFactory )
        {
            _store = store;
            _stateFactory = stateFactory;
        }

        public async Task<HttpResponseMessage> Get()
        {
            var result = await _store.FindAsync();
            var issuesState = new IssuesState();
            issuesState.Issues = result.Select(i => _stateFactory.Create(i));
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        } 

        public async Task<HttpResponseMessage> Get(string id)
        {
            var result = await _store.FindAsync(id);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var issuesState = new IssuesState();
            issuesState.Issues = new IssueState[] {_stateFactory.Create(result)};

            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        }
    }
}