using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using WebApiBook.IssueTrackerApi.Infrastructure;

namespace WebApiBook.IssueTrackerApi.Controllers
{   
    public class IssueProcessorController : ApiController
    {
        private readonly IIssueSource _issueSource;

        public IssueProcessorController(IIssueSource issueSource)
        {
            _issueSource = issueSource;
        }

        public Task Post(string id, string action)
        {
            return null;
        }

    }
}