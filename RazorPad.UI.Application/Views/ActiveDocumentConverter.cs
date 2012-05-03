using System;
using System.Windows.Data;
using RazorPad.ViewModels;

namespace RazorPad.Views
{
    public class ActiveDocumentConverter : IValueConverter
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
    }
}