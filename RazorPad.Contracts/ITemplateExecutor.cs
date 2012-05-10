namespace RazorPad.Compilation
{
    public interface ITemplateExecutor
    {
        string Execute(RazorDocument document);
        string Execute(string templateText, dynamic model);
    }
}