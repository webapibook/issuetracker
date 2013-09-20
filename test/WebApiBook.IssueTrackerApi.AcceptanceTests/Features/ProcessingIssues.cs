using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Should;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;
using Moq;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class ProcessingIssues : IssuesFeature
    {
        private string _uriProcessor = "http://localhost/issueprocessor/1?";

        [Scenario]
        public void ClosingAnOpenIssue()
        {
            var issue = FakeIssues.FirstOrDefault(i=>i.Status == IssueStatus.Open);

            "Given an existing open issue".
                f(() =>
                    {
                        MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                        MockIssueStore.Setup(i => i.UpdateAsync("1", issue, It.IsAny<string>())).Returns(Task.FromResult(""));
                    });
            "When a POST request is made to the issue processor AND the action is 'close'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=close");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue is closed".
                f(() =>
                    {
                        issue.Status.ShouldEqual(IssueStatus.Closed);
                        MockIssueStore.Verify(i => i.UpdateAsync("1", issue, It.IsAny<string>()));
                    });
        }

        [Scenario]
        public void TransitioningAnOpenIssue()
        {
            var issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Open);

            "Given an existing open issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", issue, It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'transition'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=transition");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue is closed".
                f(() =>
                {
                    issue.Status.ShouldEqual(IssueStatus.Closed);
                    MockIssueStore.Verify(i => i.UpdateAsync("1", issue, It.IsAny<string>()));
                });
        }

        [Scenario]
        public void ClosingAClosedIssue()
        {
            var issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Closed);

            "Given an existing closed issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", issue, It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'close'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=close");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public void OpeningAClosedIssue()
        {
            var issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Closed);

            "Given an existing closed issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", issue, It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'open'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=open");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue is closed".
                f(() =>
                {
                    issue.Status.ShouldEqual(IssueStatus.Open);
                    MockIssueStore.Verify(i => i.UpdateAsync("1", issue, It.IsAny<string>()));
                });
        }

        [Scenario]
        public void TransitioningAClosedIssue()
        {
            var issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Closed);

            "Given an existing closed issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", issue, It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'open'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=open");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue is closed".
                f(() =>
                {
                    issue.Status.ShouldEqual(IssueStatus.Open);
                    MockIssueStore.Verify(i => i.UpdateAsync("1", issue, It.IsAny<string>()));
                });
        }

        [Scenario]
        public void OpeningAnOpenIssue()
        {
            var issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Closed);

            "Given an existing open issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", issue, It.IsAny<string>())).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'open'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=close");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '400 Bad Request' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public void OpeningAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a POST request is made to the issue processor AND the action is 'open'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=open");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

        [Scenario]
        public void ClosingAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a POST request is made to the issue processor AND the action is 'close'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=close");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

        [Scenario]
        public void TransitioningAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a POST request is made to the issue processor AND the action is 'transition'".
                f(() =>
                    {
                        Request.RequestUri = new Uri(_uriProcessor + "action=transition");
                        Request.Method = HttpMethod.Post;
                        Response = Client.SendAsync(Request).Result;
                    });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
            
        }
        
    }
}
