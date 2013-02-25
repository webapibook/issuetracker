using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using IssueTrackerApi.Infrastructure;
using IssueTrackerApi.Models;
using Newtonsoft.Json.Linq;

namespace IssueTrackerApi.Controllers
{
    public class IssuesController : ApiController
    {
        private readonly IIssueSource _issueSource;

        public IssuesController(IIssueSource issueSource )
        {
            _issueSource = issueSource;
        }

        public Task<IEnumerable<Issue>> Get()
        {
            return _issueSource.FindAsync();
        } 

        public Task<Issue> Get(string id)
        {
            return _issueSource.FindAsync(id);
        }

        public Task<HttpResponseMessage> Post(Issue issue)
        {
            return null;
        }

        public Task Delete(string id)
        {
            return null;
        }

        public Task Patch(JObject issue)
        {
            return null;
        }  
    }
}