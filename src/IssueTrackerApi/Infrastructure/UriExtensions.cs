using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace IssueTrackerApi.Infrastructure
{
    public static class UriExtensions
    {
        public static dynamic GetQueryStringObject(this Uri uri)
        {
            var qsValues = uri.ParseQueryString();
            IDictionary<string,object> obj = new ExpandoObject();
            foreach (string key in qsValues.Keys)
            {
                var val = qsValues[key];
                if (val.IndexOf(",") > -1)
                {
                    var values = val.Split(',');
                    obj[key] = values;
                }
                else
                    obj[key] = val;
                
            }
            return obj;
        }
    }
}