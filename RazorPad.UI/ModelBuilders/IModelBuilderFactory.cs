namespace RazorPad.UI
{
    public interface IModelBuilderFactory
    {
        bool CanBuild(IModelProvider provider);
        ModelBuilder Build(IModelProvider provider = null);
    }
}