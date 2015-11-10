using System;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nest.Searchify.Abstractions;
using Nest.Searchify.Extensions;
using Nest.Searchify.Queries;

namespace Nest.Searchify.Mvc.HtmlHelpers
{
    public static class UrlHelperSearchActionExtensions
    {
        private static NameValueCollection MergeAndSortNameValueCollection<TParameters>(NameValueCollection @this, NameValueCollection that) where TParameters : Parameters, new()
        {
            foreach (string key in that)
            {
                var values = that.GetValues(key)?.ToList();
                if (values != null && values.Count > 1)
                {
                    var thisValues = (@this.GetValues(key) ?? Enumerable.Empty<string>()).ToList();
                    foreach (var value in values)
                    {
                        if(!thisValues.Contains(value)) 
                            @this.Add(key, value);
                    }
                }
                else
                {
                    @this.Set(key, that.Get(key));
                }
            }
            return QueryStringParser<TParameters>.TypeParsers.Sort(@this);
        }

        public static string SearchNextPage<TParameters>(this UrlHelper urlHelper, IPaginationOptions<TParameters> paginationOptions) where TParameters : Parameters, new()
        {
            var uri = paginationOptions.GetNextPageUri(new Uri($"http://tempuri.org{urlHelper.Action()}"));
            return uri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        }

        public static string SearchPreviousPage<TParameters>(this UrlHelper urlHelper, IPaginationOptions<TParameters> paginationOptions) where TParameters : Parameters, new()
        {
            var uri = paginationOptions.GetPreviousPageUri(new Uri($"http://tempuri.org{urlHelper.Action()}"));
            return uri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        }

        public static string SearchAction<TParameters>(this UrlHelper urlHelper, TParameters parameters) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(parameters);
            var currentNvc = HttpUtility.ParseQueryString(urlHelper.RequestContext.HttpContext.Request.Url?.Query ?? "");

            var searchNvc = MergeAndSortNameValueCollection<TParameters>(currentNvc, nvc);
            return $"{(searchNvc.HasKeys() ? "?" : "")}{searchNvc}";
        }

        public static string SearchAction<TParameters>(this UrlHelper urlHelper, string actionName, TParameters parameters) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(parameters);
            var baseUri = new Uri($"http://tempuri.org{urlHelper.Action(actionName)}");

            var searchNvc = MergeAndSortNameValueCollection<TParameters>(HttpUtility.ParseQueryString(baseUri.Query), nvc);

            return $"/{baseUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}{(searchNvc.HasKeys()?"?" : "")}{searchNvc}";
        }

        public static string SearchAction<TParameters>(this UrlHelper urlHelper, string actionName, RouteValueDictionary routeValues, TParameters parameters) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(parameters);
            var baseUri = new Uri($"http://tempuri.org{urlHelper.Action(actionName, routeValues)}");

            var searchNvc = MergeAndSortNameValueCollection<TParameters>(HttpUtility.ParseQueryString(baseUri.Query), nvc);

            return $"/{baseUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}{(searchNvc.HasKeys() ? "?" : "")}{searchNvc}";
        }

        public static string SearchAction<TParameters>(this UrlHelper urlHelper, string actionName, object routeValues, TParameters parameters) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(parameters);
            var baseUri = new Uri($"http://tempuri.org{urlHelper.Action(actionName, routeValues)}");

            var searchNvc = MergeAndSortNameValueCollection<TParameters>(HttpUtility.ParseQueryString(baseUri.Query), nvc);

            return $"/{baseUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}{(searchNvc.HasKeys() ? "?" : "")}{searchNvc}";
        }

        public static string SearchAction<TParameters>(this UrlHelper urlHelper, string actionName, string controllerName, TParameters parameters) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(parameters);
            var baseUri = new Uri($"http://tempuri.org{urlHelper.Action(actionName, controllerName)}");

            var searchNvc = MergeAndSortNameValueCollection<TParameters>(HttpUtility.ParseQueryString(baseUri.Query), nvc);

            return $"{baseUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}{(searchNvc.HasKeys() ? "?" : "")}{searchNvc}";
        }

        public static string SearchAction<TParameters>(this UrlHelper urlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, TParameters parameters) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(parameters);
            var baseUri = new Uri($"http://tempuri.org{urlHelper.Action(actionName, controllerName, routeValues)}");

            var searchNvc = MergeAndSortNameValueCollection<TParameters>(HttpUtility.ParseQueryString(baseUri.Query), nvc);

            return $"/{baseUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}{(searchNvc.HasKeys() ? "?" : "")}{searchNvc}";
        }

        public static string SearchAction<TParameters>(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues, TParameters parameters) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(parameters);
            var baseUri = new Uri($"http://tempuri.org{urlHelper.Action(actionName, controllerName, routeValues)}");

            var searchNvc = MergeAndSortNameValueCollection<TParameters>(HttpUtility.ParseQueryString(baseUri.Query), nvc);

            return $"/{baseUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)}{(searchNvc.HasKeys() ? "?" : "")}{searchNvc}";
        }
    }
}