using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Should;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public class IssueCreationTests : IssueControllerTests
    {
        protected override HttpRequestMessage GetRequest()
        {
            var request = base.GetRequest();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("http://localhost/issue");
            return request;
        }

        [Scenario]
        public void CreatingAnIssue()
        {
            Issue issue = null;

            "Given a new issue".
                f(() =>
                    {
                        issue = new Issue {Description = "A new issue", Title = "A new issue"};
                        issue.Description = "A new issue";
                        issue.Title = "A new issue";
                        var newIssue = new Issue {Id = "1"};
                        MockIssueStore.Setup(i => i.CreateAsync(issue)).Returns(Task.FromResult(newIssue));
                    });
            "When a POST request is made".
                f(() => Response = Controller.Post(issue).Result);
            "Then a '201 Created' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.Created));
            "Then the issue should be added".
                f(() => MockIssueStore.Verify(i => i.CreateAsync(issue)));
            "Then the response location header will be set to the resource location".
                f(() => Response.Headers.Location.AbsoluteUri.ShouldEqual("http://localhost/issue/1"));
        }
    }
}
