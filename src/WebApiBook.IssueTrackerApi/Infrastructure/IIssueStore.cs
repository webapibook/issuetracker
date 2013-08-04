using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public interface IIssueStore
    {
        Task<IEnumerable<Issue>> FindAsync();
        Task<Issue> FindAsync(string issueId);
        Task<IEnumerable<Issue>> FindAsyncQuery(dynamic values);
        Task UpdateAsync(string issueId, dynamic values);
        Task DeleteAsync(string issueId);
        Task<Issue> CreateAsync(Issue issue);
        Task Action(string issueId, string action, dynamic values);
    }
}
