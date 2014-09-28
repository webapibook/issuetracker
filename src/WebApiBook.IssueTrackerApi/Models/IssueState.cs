using System.Collections.Generic;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class IssueState
    {
        public IssueState()
        {
            Links = new List<Link>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public IList<Link> Links { get; private set; } 
    }
}