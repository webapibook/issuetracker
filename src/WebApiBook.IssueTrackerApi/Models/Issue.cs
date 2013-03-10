using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class Issue
    {
        public Issue()
        {
            Actions = new List<ActionLink>();
            Links = new List<Link>();
        }

        public string Id { get; set; }

        public string Href { get; set; }

        public string Title { get; set; }
        
        public string Description { get; set; }

        public IList<ActionLink> Actions { get; set; }

        public IList<Link> Links { get; set; } 

        public string State { get; set; }
    }
}