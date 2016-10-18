using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Nest.Searchify.Mvc.Binder
{
    public class JsonPropertyModelBinder : DefaultModelBinder
    {
        protected override PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var toReturn = base.GetModelProperties(controllerContext, bindingContext);

            var additional = new List<PropertyDescriptor>();

            foreach (var p in GetTypeDescriptor(controllerContext, bindingContext).GetProperties().Cast<PropertyDescriptor>())
            {
                foreach (var attr in p.Attributes.OfType<JsonPropertyAttribute>())
                {
                    if (!attr.PropertyName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        additional.Add(new ParameterPropertyDescriptor(attr.PropertyName, p));

                        if (bindingContext.PropertyMetadata.ContainsKey(p.Name))
                            bindingContext.PropertyMetadata.Add(attr.PropertyName,
                                bindingContext.PropertyMetadata[p.Name]);
                    }
                }
            }

            return new PropertyDescriptorCollection(toReturn.Cast<PropertyDescriptor>().Concat(additional).ToArray());
        }
    }
}