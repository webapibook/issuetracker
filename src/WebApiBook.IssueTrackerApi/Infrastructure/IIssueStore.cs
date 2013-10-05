using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public interface IIssueStore
    {
        Task<IEnumerable<Issue>> FindAsync();
        Task<Issue> FindAsync(string issueId);
        Task<IEnumerable<Issue>> FindAsyncQuery(string searchText);
        Task UpdateAsync(string issueId, JObject values);
        Task UpdateAsync(Issue issue);
        Task DeleteAsync(string issueId);
        Task CreateAsync(Issue issue);
    }
}
