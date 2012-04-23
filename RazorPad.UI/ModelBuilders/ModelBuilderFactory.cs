using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using RazorPad.UI.Json;

namespace RazorPad.UI.ModelBuilders
{
    public class ModelBuilderFactory
    {
        private readonly IDictionary<string, IModelBuilderBuilder> _modelBuilders;
        private readonly IModelBuilderBuilder DefaultBuilder;


        [ImportingConstructor]
        public ModelBuilderFactory(IEnumerable<IModelBuilderBuilder> modelBuilders = null)
        {
            DefaultBuilder = new JsonModelBuilderBuilder();
            _modelBuilders = (modelBuilders ?? new [] { DefaultBuilder })
                .ToDictionary(x => x.GetType().Name.Replace("ModelBuilder", string.Empty), y => y);
        }

        public ModelBuilder Create(IModelProvider modelProvider)
        {
            IModelBuilderBuilder builder = null;

            if (modelProvider != null)
            {
                var providerName = modelProvider.GetType().Name.Replace("ModelProvider", string.Empty);

                if (_modelBuilders.ContainsKey(providerName))
                    builder = _modelBuilders[providerName];
            }
            
            if (builder == null)
                builder = DefaultBuilder;

            return builder.Build(modelProvider);
        }
    }
}
