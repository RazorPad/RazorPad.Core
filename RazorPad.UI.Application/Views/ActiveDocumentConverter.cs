using System;
using System.Windows.Data;
using System.Windows.Markup;
using RazorPad.ViewModels;

namespace RazorPad.Views
{
    public class ActiveDocumentConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return RazorEditorOrDoNothing(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return RazorEditorOrDoNothing(value);
        }

        private static object RazorEditorOrDoNothing(object value)
        {
            if (value is RazorTemplateEditorViewModel)
                return value;

            return Binding.DoNothing;
        }


        private static volatile ActiveDocumentConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance = _instance ?? new ActiveDocumentConverter();
        }
    }
}