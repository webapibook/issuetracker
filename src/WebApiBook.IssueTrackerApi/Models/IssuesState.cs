using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiBook.IssueTrackerApi.Models
{
    public class IssuesState
    {
        public IEnumerable<IssueState> Issues { get; set; }
    }
}