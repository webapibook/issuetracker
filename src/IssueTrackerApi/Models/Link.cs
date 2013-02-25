using System;

namespace IssueTrackerApi.Models
{
    public class Link
    {
        public string Rel { get; set; }
        public Uri Href { get; set; }
        public string Rt { get; set; }
    }

    public class ActionLink : Link 
    {
        public string Action { get; set; }
    }
}