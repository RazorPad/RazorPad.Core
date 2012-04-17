using System.IO;
using System.Windows;
using Microsoft.Win32;
using RazorPad.ViewModels;

namespace RazorPad.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)DataContext; }
        }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel.GetSaveAsFilename += GetSaveAsFilename;
        }

        private static string GetSaveAsFilename(RazorTemplateEditorViewModel template)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            string currentFilename = template.Filename;

            if (!string.IsNullOrWhiteSpace(currentFilename))
                dlg.InitialDirectory = Path.GetDirectoryName(currentFilename);

            if (dlg.ShowDialog().GetValueOrDefault())
                return dlg.FileName;
            else
                return null;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".cshtml";
            dlg.Filter = "C# Razor Documents|*.cshtml";
            dlg.Filter = "VB Razor Documents|*.vbhtml";
            dlg.Filter = "All Files|*.*";

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                ViewModel.AddNewTemplateEditor(dlg.FileName);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentTemplate.SaveToFile();
        }

        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            var filename = GetSaveAsFilename(ViewModel.CurrentTemplate);
            ViewModel.CurrentTemplate.SaveToFile(filename);
        }
    }
}
