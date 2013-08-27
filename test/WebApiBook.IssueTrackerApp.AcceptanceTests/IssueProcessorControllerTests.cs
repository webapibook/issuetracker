using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Controllers;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public class IssueProcessorControllerTests : IssueApiTests<IssueProcessorController>
    {
        protected override IssueProcessorController GetController()
        {
            return new IssueProcessorController(MockIssueStore.Object);
        }

        protected override System.Net.Http.HttpRequestMessage GetRequest()
        {
            return new HttpRequestMessage(HttpMethod.Post, "http://localhost/issueprocessor/1");
        }
    }
}
