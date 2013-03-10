using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class GithubIssueSource : IssueSource
    {
        private readonly string _issueUrl;
        private readonly string _queryParams;

        public GithubIssueSource(string issueUrl, string queryParams, HttpMessageHandler handler)
            :base(handler)
        {
            _issueUrl = issueUrl;
            _queryParams = queryParams;
        }

        public async override Task<IEnumerable<Issue>> FindAsync()
        {
            var response = await Client.GetAsync(_issueUrl + "?" + _queryParams);
            var issuesJson = await response.Content.ReadAsAsync<JArray>();
            var issues = GetIssues(issuesJson);
            return issues;
        }


        public override Task<IEnumerable<Issue>> FindAsyncQuery(dynamic values)
        {
            throw new NotImplementedException();
        }

        public override Task<Issue> FindAsync(string issueId)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(string issueId, dynamic values)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync(string issueId)
        {
            throw new NotImplementedException();
        }

        public override Task<Issue> CreateAsync(Issue issue)
        {
            throw new NotImplementedException();
        }

        public override Task Action(string issueId, string action, dynamic values)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Issue> GetIssues(JArray issuesJson)
        {
            foreach (JObject issueJson in issuesJson)
            {
                yield return GetIssue(issueJson);
            }
        }

        private Issue GetIssue(dynamic issueJson)
        {
            var issue = new Issue();
            issue.Title = issueJson.title;
            issue.State = issueJson.state;
            issue.Id = issueJson.number;
            issue.Description = issueJson.body_text;
            issue.Href = "/issues/" + issueJson.number;
            issue.Links.Add(new Link{Href=issueJson.url, Rel="http://rels.webapibook.net#issue-source-url", Rt="issue source"});
            issue.Links.Add(new Link { Href = issueJson.user.url, Rel = "http://rels.webapibook.net#created-by", Rt = "creator" });
            if (issueJson.assignee != null)
            {
                issue.Links.Add(new Link { Href = issueJson.assignee.url, Rel = "http://rels.webapibook.net#assigned-to", Rt = "assignee" });
            }
            issue.Actions.Add(new ActionLink { Action = "close", Rel = "http://rels.webapibook.net#close", Href = "/issues/" + issueJson.number + "/issueprocessor?action=close", Rt="transition" });
            return issue;
        }
    }
}