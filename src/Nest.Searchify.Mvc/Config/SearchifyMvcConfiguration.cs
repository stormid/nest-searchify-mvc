using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Nest.Searchify.Abstractions;
using Nest.Searchify.Mvc.Binder;

namespace Nest.Searchify.Mvc.Config
{
    public interface IRegisterParameterBindersAction
    {
        IRegisterParameterBindersAction FromThisAssembly();
        IRegisterParameterBindersAction FromAssemblyContaining<TType>();
        IRegisterParameterBindersAction For<TType>();
        IRegisterParameterBindersAction FromAssemblyContaining(Type type);
        IRegisterParameterBindersAction For(Type type);
    }

    internal class RegisterParameterBindersAction : IRegisterParameterBindersAction
    {
        public IRegisterParameterBindersAction FromThisAssembly()
        {
            return RegisterParametersBindersInAssembly(Assembly.GetCallingAssembly());
        }

        public IRegisterParameterBindersAction FromAssemblyContaining<TType>()
        {
            return FromAssemblyContaining(typeof (TType));
        }

        public IRegisterParameterBindersAction For<TType>()
        {
            return For(typeof(TType));
        }

        public IRegisterParameterBindersAction FromAssemblyContaining(Type type)
        {
            var assembly = type?.Assembly;
            return RegisterParametersBindersInAssembly(assembly);
        }

        public IRegisterParameterBindersAction For(Type type)
        {
            if (IsParametersType(type) && !ModelBinders.Binders.ContainsKey(type))
            {
                ModelBinders.Binders.Add(type, new JsonPropertyModelBinder());
            }
            return this;
        }

        private IRegisterParameterBindersAction RegisterParametersBindersInAssembly(Assembly assembly)
        {
            if (assembly != null)
            {
                foreach (
                    var t in
                        assembly.GetExportedTypes()
                            .Where(IsParametersType)
                            .Where(t => !ModelBinders.Binders.ContainsKey(t))
                            .ToList()
                            .Select(parameterTypes => parameterTypes))
                {
                    For(t);
                }
            }
            return this;
        }


        private static bool IsParametersType(Type t)
        {
            return !t.IsAbstract
                   && t.IsClass
                   &&
                   (typeof(IPagingParameters).IsAssignableFrom(t) || typeof(ISortingParameters).IsAssignableFrom(t));
        }
    }

    public interface ISearchifyMvcConfigurer
    {
        ISearchifyMvcConfigurer ParameterBinding(Action<IRegisterParameterBindersAction> binders);
    }

    internal class SearchifyMvcConfigurer : ISearchifyMvcConfigurer
    {
        public ISearchifyMvcConfigurer ParameterBinding(Action<IRegisterParameterBindersAction> binders)
        {
            binders(new RegisterParameterBindersAction());
            return this;
        }
    }

    public static class SearchifyMvcConfiguration
    {
        public static void Configure(Action<ISearchifyMvcConfigurer> configure)
        {
            configure(new SearchifyMvcConfigurer());
        }
    }
}