using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Moq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using WebApiContrib.Testing;
using Xunit;
using Should;

namespace WebApiBook.IssueTrackerApi.Tests
{
    public class IssuesControllerTests
    {
        private Mock<IIssueSource> _mockIssueSource = new Mock<IIssueSource>();
        private IssuesController _controller;

        public IssuesControllerTests()
        {
            _controller = new IssuesController(_mockIssueSource.Object);
        }

        [Fact]
        public void ShouldCallFindAsyncWhenGETForAllIssues()
        {
            _controller.Get();
            _mockIssueSource.Verify(i=>i.FindAsync());
        }

        [Fact]
        public async void ShouldCallFindAsyncWhenGETForIssue()
        {
            _mockIssueSource.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(new Issue(){Id="1"}));
            _mockIssueSource.Verify(i => i.FindAsync("1"));
        }

        [Fact]
        public void ShouldReturnIssueWhenGETForExistingIssue()
        {
            var issue = new Issue();
            _mockIssueSource.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
            var foundIssue = _controller.Get("1").Result;
            foundIssue.Equals(issue);
        }

        [Fact]
        public void ShouldReturnNotFoundWhenGETForNonExistingIssue()
        {
            _mockIssueSource.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null));
            var ex = Assert.Throws<AggregateException>(() =>
                {
                    var task = _controller.Get("1");
                    var result = task.Result;
                });
            ex.InnerException.ShouldBeType<HttpResponseException>();
            ((HttpResponseException) ex.InnerException).Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void ShouldCallCreateAsyncWhenPOSTForNewIssue()
        {
            _controller.ConfigureForTesting(HttpMethod.Post, "http://test.com/issues");
            var issue = new Issue();
            _mockIssueSource.Setup(i => i.CreateAsync(issue)).Returns(() => Task.FromResult(issue));
            var response = _controller.Post(issue).Result;
            _mockIssueSource.Verify(i=>i.CreateAsync(It.Is<Issue>(iss=>iss.Equals(issue))));
        }

        [Fact]
        public async void ShouldReturnIssueWhenPOSTForNewIssue()
        {
            _controller.ConfigureForTesting(HttpMethod.Post, "http://test.com/issues");
            var issue = new Issue();
            _mockIssueSource.Setup(i => i.CreateAsync(issue)).Returns(() => Task.FromResult(issue));
            var response = _controller.Post(issue).Result;
            var readIssue = response.Content.ReadAsAsync<Issue>().Result;
            readIssue.ShouldEqual(issue);
        }

        [Fact]
        public async void ShouldReturnCreatedWhenPOSTForNewIssue()
        {
            _controller.ConfigureForTesting(HttpMethod.Post, "http://test.com/issues");
            var issue = new Issue();
            _mockIssueSource.Setup(i => i.CreateAsync(issue)).Returns(() => Task.FromResult(issue));
            var response = _controller.Post(issue).Result;
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);            
        }

        [Fact]
        public void ShouldReturnLocationeHeaderWhenPOSTForNewIssue()
        {
            _controller.ConfigureForTesting(HttpMethod.Post, "http://test.com/issues");
            var issue = new Issue();
            var createdIssue = new Issue();
            createdIssue.Id = "1";
            _mockIssueSource.Setup(i => i.CreateAsync(issue)).Returns(() => Task.FromResult(createdIssue));
            var response = _controller.Post(issue).Result;            
            response.Headers.Location.AbsoluteUri.ShouldEqual("http://test.com/issues/1");            
        }

        [Fact]
        public void ShouldCallDeleteAsyncWhenDELETEForExistingIssue()
        {
            _mockIssueSource.Setup(i => i.DeleteAsync("1")).Returns(() => Task.FromResult(""));
            var task = _controller.Delete("1");
            _mockIssueSource.Verify(i => i.DeleteAsync("1"));
        }

        [Fact]
        public void ShouldCallUpdateAsyncWhenPATCHForExistingIssue()
        {
            var value = new JObject();
            _mockIssueSource.Setup(i => i.UpdateAsync("1", value)).Returns(() => Task.FromResult(""));
            var task = _controller.Patch("1", value);
            _mockIssueSource.Verify(i=>i.UpdateAsync("1", value));
        }

        [Fact]
        public void ShouldReturnNotFoundWhenPATCHForNonExistingIssue()
        {
            var payload = new JObject();
            _controller.ConfigureForTesting(new HttpMethod("PATCH"), "http://test.com/issues");
            _mockIssueSource.Setup(i => i.UpdateAsync("1", payload)).Throws(new ArgumentException());
            var ex = Assert.Throws<AggregateException>(() =>
                {
                    var task = _controller.Patch("1", payload);
                    task.Wait();
                });
            ex.InnerException.ShouldBeType<HttpResponseException>();
            ((HttpResponseException)ex.InnerException).Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ShouldReturnNotFoundWhenDELETEForNonExistingIssue()
        {
            _controller.ConfigureForTesting(HttpMethod.Delete, "http://test.com/issues");
            _mockIssueSource.Setup(i => i.DeleteAsync("1")).Throws(new ArgumentException());
            var ex = Assert.Throws<AggregateException>(() =>
            {
                var task = _controller.Delete("1");
                task.Wait();
            });
            ex.InnerException.ShouldBeType<HttpResponseException>();
            ((HttpResponseException)ex.InnerException).Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

    }
}
