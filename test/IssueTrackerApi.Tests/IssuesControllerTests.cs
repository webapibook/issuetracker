using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IssueTrackerApi.Controllers;
using IssueTrackerApi.Infrastructure;
using IssueTrackerApi.Models;
using Moq;
using Xunit;

namespace IssueTrackerApi.Tests
{
    public class IssuesControllerTests
    {
        private Mock<IIssueSource> _mockIssueSource = new Mock<IIssueSource>();

        [Fact]
        public void WhenGettingAllShouldCallFindAsync()
        {
            _mockIssueSource = new Mock<IIssueSource>();
            _mockIssueSource.Setup(i => i.FindAsync()).Returns(new Task<IEnumerable<Issue>>(()=>new [] {new Issue()}));
            var controller = new IssuesController(_mockIssueSource.Object);
            controller.Get();
            _mockIssueSource.Verify(i=>i.FindAsync());
        }
    }
}
