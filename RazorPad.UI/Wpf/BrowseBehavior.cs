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
        public static string GetHtml(WebBrowser browser)
        {
            return (string)browser.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser browser, string value)
        {
            browser.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebBrowser;
            if (browser == null) return;
            
            var text = e.NewValue as string;

            if (string.IsNullOrWhiteSpace(text))
                text = "<html/>";

            dynamic document = browser.Document;

            if (document == null || document.documentElement == null)
            {
                browser.NavigateToString("<html/>");
                document = browser.Document;
            }

            document.close();
            document.write(text);
        }
    }
}
