using System;
using System.Net.Http;
using System.Web.Http.Routing;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class LinkFactory
    {
        private readonly UrlHelper _urlHelper;
        private readonly string _controllerName;
        
        public class Rels
        {
            public const string Self = "self";
        }
 
        public LinkFactory(HttpRequestMessage request, string controllerName)
        {
            _urlHelper = new UrlHelper(request);
            _controllerName = controllerName;
        }
 
        protected Uri GetUri(object routeValues, string route = "DefaultApi")
        {
            return new Uri(_urlHelper.Link(route, routeValues));
        }
 
        public Link Self(string id, string route = "DefaultApi")
        {
            return new Link{Rel=Rels.Self, Href=GetUri(new {controller=_controllerName, id=id}, route)};
        } 
 
    }
}