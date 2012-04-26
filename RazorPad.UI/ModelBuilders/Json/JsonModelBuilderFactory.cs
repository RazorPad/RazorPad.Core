using System.ComponentModel.Composition;
using RazorPad.Providers;
using RazorPad.UI.ModelBuilders.Json;

namespace RazorPad.UI.Json
{
    [Export(typeof(IModelBuilderFactory))]
    public class JsonModelBuilderFactory : ModelBuilderFactory<JsonModelProvider, JsonModelBuilder>
    {
        protected override dynamic ViewModel(JsonModelProvider provider)
        {
            return new JsonModelBuilderViewModel(provider);
        }
    }
}