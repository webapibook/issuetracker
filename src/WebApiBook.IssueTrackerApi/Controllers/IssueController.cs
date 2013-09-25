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
using System.Web.Http.Tracing;

namespace WebApiBook.IssueTrackerApi.Controllers
{
    public class IssueController : ApiController
    {
        private readonly IIssueStore _store;
        private readonly IStateFactory<Issue, IssueState> _stateFactory;
        private readonly IssueLinkFactory _linkFactory;
        private static readonly string TraceCategory = typeof(IssueController).FullName;

        public IssueController(IIssueStore store, 
            IStateFactory<Issue, IssueState> stateFactory, 
            IssueLinkFactory linkFactory)
        {
            _store = store;
            _stateFactory = stateFactory;
            _linkFactory = linkFactory;

        }

        public async Task<HttpResponseMessage> Get()
        {
            var tracer = this.Configuration.Services.GetTraceWriter(); 
            
            tracer.Trace(Request,
                TraceCategory, TraceLevel.Debug, "Retrieving all issues");

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
            var tracer = this.Configuration.Services.GetTraceWriter(); 

            var result = await _store.FindAsync(id);
            if (result == null)
            {
                tracer.Trace(Request, 
                    TraceCategory, TraceLevel.Debug, "Issue with id {0} not found", id);
                
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            HttpResponseMessage response = null;

            if( Request.Headers.IfModifiedSince.HasValue &&
                Request.Headers.IfModifiedSince == result.LastModified)
            {
                response = Request.CreateResponse(HttpStatusCode.NotModified);

                tracer.Trace(Request,
                    TraceCategory, TraceLevel.Debug, "Returning 'Not Modified' for Issue with id {0}", id);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(result));
                response.Content.Headers.LastModified = result.LastModified;

                tracer.Trace(Request,
                    TraceCategory, TraceLevel.Debug, "Returning 'Ok' for Issue with id {0}", id);
            }
            
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.Public = true;
            response.Headers.CacheControl.MaxAge = TimeSpan.FromMinutes(5);

            return response;
        }

        [Authorize]
        public async Task<HttpResponseMessage> Post(Issue issue)
        {
            var tracer = this.Configuration.Services.GetTraceWriter(); 

            var newIssue = await _store.CreateAsync(issue, User.Identity.Name);

            tracer.Trace(Request,
                    TraceCategory, TraceLevel.Debug, "Created new Issue with id {0}", newIssue.Id);

            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = _linkFactory.Self(newIssue.Id).Href;
            return response;
        }

        [Authorize]
        public async Task<HttpResponseMessage> Patch(string id, JObject issueUpdate)
        {
            var tracer = this.Configuration.Services.GetTraceWriter(); 

            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            if (!Request.Headers.IfModifiedSince.HasValue)
            {
                tracer.Trace(Request,
                    TraceCategory, TraceLevel.Debug, "Issue with id {0} not updated. Missing IfModifiedSince header", id);

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Missing IfModifiedSince header");
            }

            if (Request.Headers.IfModifiedSince != issue.LastModified)
            {
                tracer.Trace(Request,
                    TraceCategory, TraceLevel.Debug, "Issue with id {0} not updated. Conflict", id);

                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            await _store.UpdateAsync(id, issueUpdate, User.Identity.Name);

            tracer.Trace(Request,
                    TraceCategory, TraceLevel.Debug, "Updated Issue with id {0}", id);

            return Request.CreateResponse(HttpStatusCode.OK);
            
        }
      
        public async Task<HttpResponseMessage> Delete(string id)
        {
            var tracer = this.Configuration.Services.GetTraceWriter(); 

            var issue = await _store.FindAsync(id);
            if (issue == null)
            {
                tracer.Trace(Request,
                   TraceCategory, TraceLevel.Debug, "Issue with id {0} not deleted. Not Found", id);

                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            await _store.DeleteAsync(id);

            tracer.Trace(Request,
                   TraceCategory, TraceLevel.Debug, "Deleted Issue with id {0}", id);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}