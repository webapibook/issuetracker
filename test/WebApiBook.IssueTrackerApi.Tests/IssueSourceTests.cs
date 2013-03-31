using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiContrib.Testing;
using Xunit;
using Should;

namespace WebApiBook.IssueTrackerApi.Tests
{
    public class IssueSourceTests
    {
        public class TheConstructor
        {
            [Fact]
            public void ShouldCreateClientWhenInvoked()
            {
                var source = new TestIssueSource(null);
                source.Client.ShouldNotBeNull();
            }

            [Fact]
            public void ShouldSetHandlerWhenHandlerIsPassed()
            {
                var handler = new FakeHandler(h => null);
                var source = new TestIssueSource(handler);
                var handlerField = typeof (HttpMessageInvoker).GetField("handler",
                                                                        BindingFlags.NonPublic | BindingFlags.Instance);
                var clientHandler = (HttpMessageHandler) handlerField.GetValue(source.Client);
                clientHandler.ShouldEqual(handler);
            }
        }
    }

    public class TestIssueSource : IssueSource
    {
        public TestIssueSource(HttpMessageHandler handler)
            : base(handler)
        {

        }
    }
}
