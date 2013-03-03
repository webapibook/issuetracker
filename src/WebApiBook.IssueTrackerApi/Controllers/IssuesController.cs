using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Controllers
{
    public class IssuesController : ApiController
    {
        private readonly IIssueSource _issueSource;

        public IssuesController(IIssueSource issueSource )
        {
            _issueSource = issueSource;
        }

        public async Task<IEnumerable<Issue>> Get()
        {
            return await _issueSource.FindAsync();
        } 

        public async Task<Issue> Get(string id)
        {
            var issue = await _issueSource.FindAsync(id);
            if(issue == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return issue;
        }

        public async Task<HttpResponseMessage> Post(Issue issue)
        {
            var createdIssue = await _issueSource.CreateAsync(issue);
            var link = Url.Link("DefaultApi", new {Controller = "issues", id = createdIssue.Id});
            var response = Request.CreateResponse(HttpStatusCode.Created, createdIssue);
            response.Headers.Location = new Uri(link);
            return response;
        }

        public async Task Delete(string id)
        {
            await _issueSource.DeleteAsync(id);
        }

        public async Task Patch(string id, JObject issue)
        {
            await _issueSource.UpdateAsync(id, issue);
        }  
    }
}