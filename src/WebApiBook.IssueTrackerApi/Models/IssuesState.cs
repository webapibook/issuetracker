using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiContrib.CollectionJson;
using CJLink = WebApiContrib.CollectionJson.Link;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class IssuesState : IReadDocument
    {
        public IssuesState()
        {
            Links = new List<Link>();
        }

        public IEnumerable<IssueState> Issues { get; set; }
        public IList<Link> Links { get; private set; }

        Collection IReadDocument.Collection
        {
            get
            {
                var collection = new Collection();
                collection.Href = Links.SingleOrDefault(l => l.Rel == IssueLinkFactory.Rels.Self).Href;
                collection.Links.Add(new CJLink {Rel="profile", Href = new Uri("http://webapibook.net/profile")});
                foreach (var issue in Issues)
                {
                    var item = new Item();
                    foreach (var link in issue.Links)
                    {
                        if (link.Rel == IssueLinkFactory.Rels.Self)
                            item.Href = link.Href;
                        else
                        {
                            item.Links.Add(new CJLink{Href = link.Href, Rel = link.Rel});
                        }
                    }
                    collection.Items.Add(item);
                    item.Data.Add(new Data {Name="Description", Value=issue.Description});
                    item.Data.Add(new Data {Name = "Status", Value = issue.Status});
                    item.Data.Add(new Data {Name="Title", Value = issue.Title});
                }
                var query = new Query { Rel=IssueLinkFactory.Rels.SearchQuery, Href = new Uri("/issue", UriKind.Relative), Prompt="Issue search" };
                query.Data.Add(new Data() { Name = "SearchText", Prompt = "Text to match against Title and Description" });
                collection.Queries.Add(query);
                return collection;
            }
        }
    }
}