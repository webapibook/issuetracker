using System;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class Link
    {
        public string Rel { get; set; }
        public Uri Href { get; set; }
        public string Action { get; set; }
    }
}