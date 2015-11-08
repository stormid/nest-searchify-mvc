using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Nest.Searchify.Queries;

namespace Nest.Searchify.Mvc.Config
{
    public class SearchifyParametersModelBinder : DefaultModelBinder
    {
        protected override PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var toReturn = base.GetModelProperties(controllerContext, bindingContext);

            var additional = new List<PropertyDescriptor>();

            foreach (var p in GetTypeDescriptor(controllerContext, bindingContext).GetProperties().Cast<PropertyDescriptor>())
            {
                foreach (var attr in p.Attributes.OfType<ParameterAttribute>())
                {
                    additional.Add(new ParameterPropertyDescriptor(attr.Name, p));

                    if (bindingContext.PropertyMetadata.ContainsKey(p.Name)) bindingContext.PropertyMetadata.Add(attr.Name, bindingContext.PropertyMetadata[p.Name]);
                }
            }

            return new PropertyDescriptorCollection(toReturn.Cast<PropertyDescriptor>().Concat(additional).ToArray());
        }
    }
}