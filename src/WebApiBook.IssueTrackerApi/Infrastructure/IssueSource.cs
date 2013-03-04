using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public abstract class IssueSource : IIssueSource
    {
        protected IssueSource(HttpMessageHandler handler = null)
        {
            if (handler != null)
                Client = new HttpClient(handler);
            else
                Client = new HttpClient();
        }

        protected HttpClient Client { get; private set; }

        public Task<IEnumerable<Issue>> FindAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Issue>> FindAsyncQuery(dynamic values)
        {
            throw new NotImplementedException();
        }

        public Task<Issue> FindAsync(string issueId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(string issueId, dynamic values)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string issueId)
        {
            throw new NotImplementedException();
        }

        public Task<Issue> CreateAsync(Issue issue)
        {
            throw new NotImplementedException();
        }

        public Task Action(string issueId, string action, dynamic values)
        {
            throw new NotImplementedException();
        }
    }
}