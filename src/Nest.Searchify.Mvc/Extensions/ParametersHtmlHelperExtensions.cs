using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Nest.Searchify.Queries;

namespace Nest.Searchify.Mvc.Extensions
{
    public static class ParametersHtmlHelperExtensions
    {
        /// <summary>
        /// Gets the name value associated with the specified property if the <see cref="ParameterAttribute"/> has been set for the property, otherwise returns the Id value determined by the IdFor helper
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string ParameterNameFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : Parameters
        {
            var member = expression.Body as MemberExpression;
            var pa = member?.Member.GetCustomAttribute<ParameterAttribute>();
            var id = pa?.Name ?? htmlHelper.IdFor(expression).ToHtmlString();

            return id;
        }
        
        /// <summary>
        /// Outputs GET request compatible set of hidden input fields representing the current state of the parameters object
        /// </summary>
        /// <typeparam name="TModel">object containing parameters (i.e. SearchResult)</typeparam>
        /// <typeparam name="TParameters">parameters</typeparam>
        /// <returns>Html string containing a set of hidden input types representing the current state of the parameters object</returns>
        public static IHtmlString HiddenParameters<TModel, TParameters>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TParameters>> expression) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(expression.Compile()(htmlHelper.ViewData.Model));

            var tags = string.Join(Environment.NewLine, nvc.AllKeys.Select(key => HiddenParameter(key, nvc.Get((string) key))).ToList());

            return MvcHtmlString.Create(tags);
        }

        private static string HiddenParameter(string key, string value)
        {
            var tag = new TagBuilder("input");
            tag.MergeAttributes(new Dictionary<string, object>
            {
                {"type", "hidden"},
                {"id", key},
                {"name", key},
                {"value", value}
            });
            return tag.ToString();
        }
    }
}