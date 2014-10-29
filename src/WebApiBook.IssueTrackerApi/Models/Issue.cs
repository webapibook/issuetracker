using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class Issue
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public IssueStatus Status { get; set; }
    }
}