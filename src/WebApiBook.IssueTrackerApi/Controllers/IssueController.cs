using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var issuesState = new IssuesState {Issues = issues.Select(i => _stateFactory.Create(i))};
            issuesState.Links.Add(new Link{Href=Request.RequestUri, Rel = LinkFactory.Rels.Self});
            var response = Request.CreateResponse(HttpStatusCode.OK, issuesState);

            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromMinutes(5)
            };

            return response;
        } 

        public async Task<HttpResponseMessage> Get(string id)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            HttpResponseMessage response = null;

            if (Request.Headers.IfModifiedSince.HasValue && Request.Headers.IfModifiedSince == issue.LastModified)
                response = Request.CreateResponse(HttpStatusCode.NotModified);
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(issue));
                response.Content.Headers.LastModified = issue.LastModified;
            }

            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromMinutes(5)
            };

            return response;
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

            if (! Request.Headers.IfModifiedSince.HasValue)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Missing IfModifiedSince header");

            if (Request.Headers.IfModifiedSince != issue.LastModified)
                return Request.CreateResponse(HttpStatusCode.Conflict);

            //foreach (JProperty prop in issueUpdate)
            //{
            //    switch (prop.Name)
            //    {
            //        case "title":
            //            issue.Title = prop.Value.ToObject<string>();
            //            break;
            //        case "description":
            //            issue.Description = prop.Value.ToObject<string>();
            //            break;
            //        case "lastmodified":
            //            issue.LastModified = prop.Value.ToObject<DateTimeOffset>();
            //            break;
            //        case "status":
            //        {
            //            IssueStatus issueStatus;
            //            if (Enum.TryParse(prop.Value.ToObject<String>(), true, out issueStatus))
            //                issue.Status = issueStatus;
            //        }
            //            break;
            //    }

            //}
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