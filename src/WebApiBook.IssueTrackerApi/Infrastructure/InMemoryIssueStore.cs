using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class InMemoryIssueStore : IIssueStore
    {
        private readonly IList<Issue> _issues;
        private int _id = 0;
        private static Type _issueType = typeof (Issue);

        public InMemoryIssueStore()
        {
            _issues = new List<Issue>
            {
                new Issue
                {
                    Description = "This is an issue",
                    Id = "1",
                    Status = IssueStatus.Open,
                    Title = "An issue",
                    LastModified = new DateTimeOffset(new DateTime(2013, 9, 4))
                },
                new Issue
                {
                    Description = "This is a another issue",
                    Id = "2",
                    Status = IssueStatus.Closed,
                    Title = "Another Issue",
                    LastModified = new DateTimeOffset(new DateTime(2014, 8, 22))
                }
            };
            _id = _issues.Count + 1;
        }

        public Task<IEnumerable<Issue>> FindAsync()
        {
            return Task.FromResult(_issues.AsEnumerable());
        }

        public Task<Issue> FindAsync(string issueId)
        {
            return Task.FromResult(_issues.Single(i => i.Id == issueId));
        }

        public Task<IEnumerable<Issue>> FindAsyncQuery(string searchText)
        {
            return Task.FromResult(_issues.Where(
                i => i.Title.Contains(searchText) || i.Description.Contains(searchText)));
        }

        public Task UpdateAsync(Issue issue)
        {
            var oldIssue = FindAsync(issue.Id).Result;
            oldIssue.Title = issue.Title;
            oldIssue.Description = issue.Description;
            oldIssue.Status = issue.Status;
            oldIssue.LastModified = issue.LastModified;
            return Task.FromResult("");
        }

        public Task DeleteAsync(string issueId)
        {
            var issue = _issues.Single(i => i.Id == issueId);
            _issues.Remove(issue);
            return Task.FromResult("");
        }

        public Task CreateAsync(Issue issue)
        {
            issue.Id = (++_id).ToString();
            _issues.Add(issue);
            return Task.FromResult(issue);
        }
    }
}
