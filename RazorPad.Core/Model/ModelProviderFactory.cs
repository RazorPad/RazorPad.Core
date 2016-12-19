using System;

namespace RazorPad.Providers
{
    public class ModelProviderFactory<TModelProvider> : IModelProviderFactory
        where TModelProvider : IModelProvider
    {
        public virtual IModelProvider Create()
        {
            return Activator.CreateInstance<TModelProvider>();
        }
    }
}