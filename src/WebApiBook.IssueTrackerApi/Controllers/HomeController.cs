using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Tavis;
using Tavis.Home;
using Tavis.IANA;
using WebApiBook.IssueTrackerApi.Infrastructure;

namespace WebApiBook.IssueTrackerApi.Controllers
{
    public class HomeController : ApiController
    {
        private readonly IssueLinkFactory _linkFactory;

        public HomeController(IssueLinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public HttpResponseMessage Get()
        {
            var home = new Tavis.Home.HomeDocument();

            var issueLink = new Link()
                {
                    Relation = IssueLinkFactory.Rels.Issue,
                    Target = new Uri("/issue/{id}", UriKind.Relative)
                };
            issueLink.AddHint<AllowHint>(h => h.AddMethod(HttpMethod.Get));
            issueLink.AddHint<FormatsHint>(h => {
                h.AddMediaType("application/json");
                h.AddMediaType("application/vnd.issue+json");
            });

    

            home.AddResource(issueLink);

            var issuesLink = new Link()
                {
                    Relation = IssueLinkFactory.Rels.Issues, Target = new Uri("/issue", UriKind.Relative)
                };
            issuesLink.AddHint<AllowHint>(h => h.AddMethod(HttpMethod.Get));
            issuesLink.AddHint<FormatsHint>(h =>
            {
                h.AddMediaType("application/json");
                h.AddMediaType("application/vnd.collection+json");
            });

            home.AddResource(issuesLink);

            var issueProcessorLink = new Link()
                {
                    Relation = IssueLinkFactory.Rels.IssueProcessor, Target = new Uri("/issueprocessor/{id}{?action}", UriKind.Relative)
                };
            issueProcessorLink.AddHint<AllowHint>(h => h.AddMethod(HttpMethod.Post));
            
            home.AddResource(issueProcessorLink);

            return new HttpResponseMessage()
                {
                    RequestMessage = Request,
                    Content = new HomeContent(home)
                };
        }
    }

    //public class HomeContent : HttpContent
    //{
    //    private readonly MemoryStream _stream = new MemoryStream();

    //    public HomeContent(HomeDocument document)
    //    {
    //        document.Save(_stream);
    //        _stream.Position = 0;
    //    }

    //    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    //    {
    //        return _stream.CopyToAsync(stream);
    //    }

    //    protected override bool TryComputeLength(out long length)
    //    {
    //        length = _stream.Length;
    //        return true;
    //    }
    //}
}
