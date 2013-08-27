using System;
using System.Collections.Generic;
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
            var result = await _store.FindAsync();
            var issuesState = new IssuesState();
            issuesState.Issues = result.Select(i => _stateFactory.Create(i));
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        } 

        public async Task<HttpResponseMessage> Get(string id)
        {
            var result = await _store.FindAsync(id);
            if (result == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var issuesState = new IssuesState();
            issuesState.Issues = new [] {_stateFactory.Create(result)};

            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        }

        public async Task<HttpResponseMessage> Post(Issue issue)
        {
            var newIssue = await _store.CreateAsync(issue);
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = _linkFactory.Self(newIssue.Id).Href;
            return response;
        }

        public async Task<HttpResponseMessage> Patch(string id, JObject issueUpdate)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            await _store.UpdateAsync(id, issueUpdate);
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