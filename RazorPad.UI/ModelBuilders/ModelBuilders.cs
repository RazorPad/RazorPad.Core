using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using RazorPad.UI.Json;

namespace RazorPad.UI.ModelBuilders
{
    [Export]
    public class ModelBuilders
    {
        public static ModelBuilders Current
        {
            get { return _current; }
            set { _current = value; }
        }
        private static volatile ModelBuilders _current = new ModelBuilders();


        public IList<IModelBuilderFactory> Builders
        {
            get { return _modelBuilders; }
        }
        private readonly IList<IModelBuilderFactory> _modelBuilders;


        [ImportingConstructor]
        public ModelBuilders([ImportMany]IEnumerable<IModelBuilderFactory> modelBuilders = null)
        {
            _modelBuilders = new List<IModelBuilderFactory>(modelBuilders ?? new [] {new JsonModelBuilderFactory() });
        }


        public ModelBuilder Create(IModelProvider modelProvider)
        {
            var builder = Builders.FirstOrDefault(x => x.CanBuild(modelProvider));

            if (builder == null)
                return null;

            return builder.Build(modelProvider);
        }
    }
}
