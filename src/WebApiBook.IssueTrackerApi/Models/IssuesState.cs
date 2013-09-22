using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class IssuesState
    {
        public IssuesState()
        {
            Links = new List<Link>();
        }

        public IEnumerable<IssueState> Issues { get; set; }
        public IList<Link> Links { get; private set; } 
    }
}