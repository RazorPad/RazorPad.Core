using System.Windows;
using System.Windows.Controls;

namespace RazorPad.UI.Wpf
{
    public class BrowseBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(BrowseBehavior),
            new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser wb = d as WebBrowser;
            
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
