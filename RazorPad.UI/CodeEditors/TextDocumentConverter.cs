using System;
using System.Windows.Data;
using System.Windows.Markup;
using ICSharpCode.AvalonEdit.Document;

namespace RazorPad.UI.CodeEditors
{
    public class TextDocumentConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
                return new TextDocument(value as string);
            else if (value is TextDocument)
                return ((TextDocument) value).Text;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }


        private static volatile TextDocumentConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance = _instance ?? new TextDocumentConverter();
        }
    }
}