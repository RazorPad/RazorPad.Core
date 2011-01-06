using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RazorPad.ViewModels;

namespace RazorPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RazorTemplateEditor : UserControl
    {
        protected RazorTemplateEditorViewModel ViewModel
        {
            get { return (RazorTemplateEditorViewModel) DataContext; }
        }

        public RazorTemplateEditor()
        {
            InitializeComponent();

        }


    }
}
