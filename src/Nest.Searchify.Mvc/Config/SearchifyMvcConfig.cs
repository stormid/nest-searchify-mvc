using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Nest.Searchify.Abstractions;
using Nest.Searchify.Mvc.Config;

[assembly: PreApplicationStartMethod(typeof(SearchifyMvcConfig), "RegisterParametersBinders")]

namespace Nest.Searchify.Mvc.Config
{
    public static class SearchifyMvcConfig
    {
        internal static void RegisterParametersBinder<TParameters>()
            where TParameters : class, IPagingParameters, ISortingParameters, new()
        {
            RegisterParametersBinder(typeof(TParameters));
        }

        internal static void RegisterParametersBinder(Type parametersType)
        {
            if (IsParametersType(parametersType))
            {
                ModelBinders.Binders.Add(parametersType, new SearchifyParametersModelBinder());
            }
        }

        private static bool IsParametersType(Type t)
        {
            return !t.IsAbstract
                   && t.IsClass
                   &&
                   (typeof(IPagingParameters).IsAssignableFrom(t) || typeof(ISortingParameters).IsAssignableFrom(t));
        }

        internal static void RegisterParametersBindersInAssemblyOfType(params Type[] types)
        {
            var assemblies = types.Select(t => t.Assembly).Distinct();
            foreach (var assembly in assemblies)
            {
                RegisterParametersBindersInAssembly(assembly);
            }
        }

        internal static void RegisterParametersBindersInAssembly(Assembly assembly)
        {
            if (assembly != null)
            {
                foreach (
                    var t in
                        assembly.GetExportedTypes()
                            .Where(IsParametersType)
                            .ToList()
                            .Select(parameterTypes => parameterTypes))
                {
                    RegisterParametersBinder(t);
                }
            }
        }

        internal static void RegisterParametersBindersInAssemblyOfType<T>()
        {
            RegisterParametersBindersInAssemblyOfType(typeof(T));
        }

        public static void RegisterParametersBinders(params Type[] types)
        {
            RegisterParametersBindersInAssemblyOfType(types.Any()
                ? types
                : new[] { Assembly.GetExecutingAssembly().ExportedTypes.First() });
        }

        public static void RegisterParametersBinders()
        {
            bool shouldSkip;
            bool.TryParse(ConfigurationManager.AppSettings[$"{Constants.AppSettingsRoot}SkipParameterBinderRegistration"], out shouldSkip);

            if (!shouldSkip)
            {
                RegisterParametersBindersInAssembly(HttpContext.Current?.ApplicationInstance.GetType().Assembly);
            }
        }
    }
}