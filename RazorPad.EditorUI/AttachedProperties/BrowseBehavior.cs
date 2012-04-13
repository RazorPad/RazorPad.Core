using System.Windows;
using WebBrowserControl = System.Windows.Controls.WebBrowser;

namespace RazorPad.AttachedProperties
{
    public class BrowseBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(BrowseBehavior),
            new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowserControl))]
        public static string GetHtml(WebBrowserControl d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowserControl d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowserControl wb = d as WebBrowserControl;
            
            var text = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(text))
                text = "<html/>";

            if (wb != null)
            {
                wb.NavigateToString(text);
            }
        }
    }
}
