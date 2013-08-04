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
        private readonly IIssueStore _issueStore;

        public IssueProcessorController(IIssueStore issueStore)
        {
            _issueStore = issueStore;
        }

        public Task Post(string id, string action)
        {
            return null;
        }

    }
}