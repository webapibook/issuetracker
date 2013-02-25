using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using Xunit;

namespace WebApiBook.IssueTrackerApi.Tests
{
    public class IssuesControllerTests
    {
        private Mock<IIssueSource> _mockIssueSource = new Mock<IIssueSource>();

        [Fact]
        public void ShouldCallFindAsyncWhenGettingAllIssues()
        {
            var controller = new IssuesController(_mockIssueSource.Object);
            controller.Get();
            _mockIssueSource.Verify(i=>i.FindAsync());
        }

        [Fact]
        public void ShouldCallFindAsyncWhenGettingASpecificIssue()
        {
            _mockIssueSource.Setup(i => i.FindAsync(1)).Returns(new Task<IEnumerable<Issue>>(() => new[] { new Issue() }));
            var controller = new IssuesController(_mockIssueSource.Object);
            controller.Get("1");
            _mockIssueSource.Verify(i => i.FindAsync(It.Is<String>(id=>id.Equals("1"))));
        }

    }
}
