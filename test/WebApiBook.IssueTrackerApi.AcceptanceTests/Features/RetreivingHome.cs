using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Should;
using Tavis.Home;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class RetreivingHome : HomeFeature
    {
        private Uri _uriHome = new Uri("http://localhost/");

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
            "Then it is returned".
                f(() => home.ShouldNotBeNull());
            "Then it should have an issueprocessor link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.IssueProcessor).ShouldNotBeNull());
            "Then it should have an issue link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.Issue).ShouldNotBeNull());
            "Then it should have an issues link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.Issues).ShouldNotBeNull());
            "Then it should have an search link".
                f(() => home.Resources.FirstOrDefault(l => l.Relation == IssueLinkFactory.Rels.SearchQuery).ShouldNotBeNull());

        }
    }
}
