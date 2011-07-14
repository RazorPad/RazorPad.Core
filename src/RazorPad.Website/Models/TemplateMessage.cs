namespace RazorPad.Website.Models
{
    public enum TemplateMessageKind
    {
        Error,
        Warning,
        Info,
    }

    public class TemplateMessage
    {
        public TemplateMessageKind Kind { get; set; } 
        public string Text { get; set; }
    }
}