using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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

        public static IHtmlString SeoRelPrevNextLinks<TParameters>(this UrlHelper urlHelper,
            IPaginationOptions<TParameters> paginationOptions) where TParameters : Parameters, new()
        {
            var prev = SeoRelPreviousPageLink(urlHelper, paginationOptions);
            var next = SeoRelNextPageLink(urlHelper, paginationOptions);
            var sb = new StringBuilder();
            sb.AppendLine(prev.ToString());
            sb.AppendLine(next.ToString());
            return MvcHtmlString.Create(sb.ToString());
        }

        public static IHtmlString SeoRelNextPageLink<TParameters>(this UrlHelper urlHelper, IPaginationOptions<TParameters> paginationOptions) where TParameters : Parameters, new()
        {
            if (paginationOptions.HasNextPage)
            {
                var uri = SearchNextPage(urlHelper, paginationOptions);
                var tag = new TagBuilder("link");
                tag.MergeAttribute("rel", "next");
                tag.MergeAttribute("href", uri);
                return MvcHtmlString.Create(tag.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Empty;
        }

        public static IHtmlString SeoRelPreviousPageLink<TParameters>(this UrlHelper urlHelper, IPaginationOptions<TParameters> paginationOptions) where TParameters : Parameters, new()
        {
            if (paginationOptions.HasPreviousPage)
            {
                var uri = SearchPreviousPage(urlHelper, paginationOptions);
                var tag = new TagBuilder("link");
                tag.MergeAttribute("rel", "prev");
                tag.MergeAttribute("href", uri);
                return MvcHtmlString.Create(tag.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Empty;
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