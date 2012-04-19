using System;
using RazorPad.Providers;

namespace RazorPad
{
    public class ModelProviderFactory
    {
        public virtual IModelProvider Create(string provider, dynamic model = null)
        {
            // TODO: Make this awesome

            if("Basic".Equals(provider, StringComparison.OrdinalIgnoreCase))
                return new BasicModelProvider(model);

            if("Json".Equals(provider, StringComparison.OrdinalIgnoreCase))
                return new JsonModelProvider(null, model);

            throw new NotSupportedException(string.Format("Provider {0} is not supported", provider));
        }
    }
}