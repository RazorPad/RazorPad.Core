namespace RazorPad.UI
{
    public interface IModelBuilderFactory
    {
        bool CanBuild(IModelProvider provider);
        IModelBuilder Build(IModelProvider provider = null);
    }
}