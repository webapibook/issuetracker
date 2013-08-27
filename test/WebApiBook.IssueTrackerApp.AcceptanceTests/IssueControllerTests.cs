using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Controllers;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class IssueControllerTests : IssueApiTests<IssueController>
    {
        protected override IssueController GetController()
        {
            return new IssueController(MockIssueStore.Object, StateFactory, IssueLinks);
        }

        protected override HttpRequestMessage GetRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, "http://localhost/issue/1");
        }

    }
}
