using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using RazorPad.UI.Json;

namespace RazorPad.UI.ModelBuilders
{
    public class ModelBuilderFactory
    {
        private readonly IDictionary<string, ModelBuilder> _modelBuilders;

        public ModelBuilder DefaultBuilder { get; set; }

        [ImportingConstructor]
        public ModelBuilderFactory(IEnumerable<ModelBuilder> modelBuilders = null)
        {
            DefaultBuilder = new JsonModelBuilder();
            _modelBuilders = (modelBuilders ?? new [] { DefaultBuilder })
                .ToDictionary(x => x.GetType().Name.Replace("ModelBuilder", string.Empty), y => y);
        }

        public ModelBuilder Create(IModelProvider modelProvider)
        {
            var providerName = modelProvider.GetType().Name.Replace("ModelProvider", string.Empty);
            
            var builder = Create(providerName);
            builder.ModelProvider = modelProvider;

            return builder;
        }

        public ModelBuilder Create(string providerName)
        {
            if (_modelBuilders.ContainsKey(providerName))
                return _modelBuilders[providerName];

            return DefaultBuilder;
        }
    }
}
