using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueTrackerApi.Models
{
    public class Issue
    {
        public string Id { get; set; }

        public Uri Href { get; set; }

        public string Title { get; set; }
        
        public string Description { get; set; }

        public IEnumerable<ActionLink> Actions { get; set; }

        public IEnumerable<Link> Links { get; set; } 

        public string State { get; set; }
    }
}