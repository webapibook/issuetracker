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
using System.Net.Http.Headers;

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
            
            var response = Request.CreateResponse(HttpStatusCode.OK, issuesState);
            
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.Public = true;
            response.Headers.CacheControl.MaxAge = TimeSpan.FromMinutes(5);

            return response;
        } 

        public async Task<HttpResponseMessage> Get(string id)
        {
            var result = await _store.FindAsync(id);
            if (result == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            HttpResponseMessage response = null;

            if( Request.Headers.IfModifiedSince.HasValue &&
                Request.Headers.IfModifiedSince == result.LastModified)
            {
                response = Request.CreateResponse(HttpStatusCode.NotModified);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(result));
                response.Content.Headers.LastModified = result.LastModified;
            }
            
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.Public = true;
            response.Headers.CacheControl.MaxAge = TimeSpan.FromMinutes(5);

            return response;
        }

        public async Task<HttpResponseMessage> Post(Issue issue)
        {
            var newIssue = await _store.CreateAsync(issue, User.Identity.Name);
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = _linkFactory.Self(newIssue.Id).Href;
            return response;
        }

        public async Task<HttpResponseMessage> Patch(string id, JObject issueUpdate)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            if (!Request.Headers.IfModifiedSince.HasValue)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Missing IfModifiedSince header");

            if (Request.Headers.IfModifiedSince != issue.LastModified)
               return Request.CreateResponse(HttpStatusCode.Conflict);

            await _store.UpdateAsync(id, issueUpdate, User.Identity.Name);
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