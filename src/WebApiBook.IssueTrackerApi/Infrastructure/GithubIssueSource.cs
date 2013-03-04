using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class GithubIssueSource : IssueSource
    {
        public GithubIssueSource(HttpMessageHandler handler)
            :base(handler)
        {
            
        }
    }
}