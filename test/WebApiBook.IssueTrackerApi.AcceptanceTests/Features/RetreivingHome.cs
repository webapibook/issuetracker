using System;
using System.Linq;
using System.Net;
using Should;
using Tavis.Home;
using WebApiBook.IssueTrackerApi.Infrastructure;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class RetreivingHome : HomeFeature
    {
        private readonly Uri _uriHome = new Uri("http://localhost/");

        [Scenario]
        public void RetrievingHomeRepresentation(HomeDocument home)
        {
            "When it is retrieved".
               f(() =>
               {
                   Request.RequestUri = _uriHome;
                   Response = Client.SendAsync(Request).Result;
                   var stream = Response.Content.ReadAsStreamAsync().Result;
                   home = HomeDocument.Parse(stream);
               });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "And it is returned".
                f(() => home.ShouldNotBeNull());
            "And it should have an issueprocessor link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.IssueProcessor).ShouldNotBeNull());
            "And it should have an issue link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.Issue).ShouldNotBeNull());
            "And it should have an issues link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.Issues).ShouldNotBeNull());
            "And it should have an search link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.SearchQuery).ShouldNotBeNull());
        }
    }
}
