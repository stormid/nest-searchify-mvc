using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Nest.Searchify.Queries;
using Newtonsoft.Json;

namespace Nest.Searchify.Mvc.HtmlHelpers
{
    public static class ParametersHtmlHelperExtensions
    {
        /// <summary>
        /// Gets the name value associated with the specified property if the <see cref="JsonPropertyAttribute"/> has been set for the property, otherwise returns the Id value determined by the IdFor helper
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string SearchifyMvcGetParameterNameFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var member = expression.Body as MemberExpression;
            var pa = member?.Member.GetCustomAttribute<JsonPropertyAttribute>();
            var id = pa?.PropertyName ?? htmlHelper.IdFor(expression).ToHtmlString();

            return id;
        }
        
        /// <summary>
        /// Outputs GET request compatible set of hidden input fields representing the current state of the parameters object
        /// </summary>
        /// <typeparam name="TModel">object containing parameters (i.e. SearchResult)</typeparam>
        /// <typeparam name="TParameters">parameters</typeparam>
        /// <returns>Html string containing a set of hidden input types representing the current state of the parameters object</returns>
        public static IHtmlString SearchifyMvcRenderParametersAsHiddenInput<TModel, TParameters>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TParameters>> expression) where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(expression.Compile()(htmlHelper.ViewData.Model));

            var tags = string.Join(Environment.NewLine, nvc.AllKeys.SelectMany(key => HiddenParameter(key, nvc.GetValues(key))).ToList());

            return MvcHtmlString.Create(tags);
        }

        /// <summary>
        /// Determines the active parameters for the current parameters object and passes them to a defined partial view for rendering
        /// </summary>
        /// <param name="partialViewName">name of the partial to render</param>
        /// <param name="htmlHelper">Html Helper</param>
        /// <typeparam name="TModel">object containing parameters (i.e. SearchResult)</typeparam>
        /// <typeparam name="TParameters">parameters</typeparam>
        /// <returns>Html string containing a set of hidden input types representing the current state of the parameters object</returns>
        public static IHtmlString SearchifyMvcRenderParametersPartial<TModel, TParameters>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TParameters>> expression, string partialViewName = "_SearchifyMvcParametersPartial") where TParameters : Parameters, new()
        {
            var nvc = QueryStringParser<TParameters>.Parse(expression.Compile()(htmlHelper.ViewData.Model));

            var vd = new ViewDataDictionary(nvc)
            {                
                TemplateInfo =
                {
                    HtmlFieldPrefix = ""
                }
            };
            vd.Add($"SearchifyMvc_Parameters_{typeof(TParameters).Name}", htmlHelper.ViewData.Model);
            return htmlHelper.Partial(partialViewName, vd);
        }

        private static IEnumerable<string> HiddenParameter(string key, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                var tag = new TagBuilder("input");
                tag.MergeAttributes(new Dictionary<string, object>
                {
                    {"type", "hidden"},
                    {"id", key},
                    {"name", key},
                    {"value", value}
                });
                yield return tag.ToString();
            }
        }
    }
}