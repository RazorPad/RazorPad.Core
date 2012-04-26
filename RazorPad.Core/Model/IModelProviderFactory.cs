namespace RazorPad
{
    public interface IModelProviderFactory
    {
        IModelProvider Create(dynamic model = null);
    }
}