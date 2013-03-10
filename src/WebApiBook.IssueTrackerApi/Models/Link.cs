using System;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class Link
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Rt { get; set; }
    }

    public class ActionLink : Link 
    {
        public string Action { get; set; }
    }
}