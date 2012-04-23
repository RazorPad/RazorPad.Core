namespace RazorPad.UI
{
    public interface IModelBuilderBuilder
    {
        ModelBuilder Build(IModelProvider provider = null);
    }
}