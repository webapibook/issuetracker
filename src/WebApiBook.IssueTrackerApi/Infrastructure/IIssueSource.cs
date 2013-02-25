using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public interface IIssueSource
    {
        Task<IEnumerable<Issue>> FindAsync();
        Task<IEnumerable<Issue>> FindAsync(dynamic values);
        Task<Issue> FindAsync(string issueId);
        Task UpdateAsync(string issueId, dynamic values);
        Task DeleteAsync(string issueId);
        Task CreateAync(Issue issue);
        Task Action(string issueId, string action, dynamic values);
    }
}
