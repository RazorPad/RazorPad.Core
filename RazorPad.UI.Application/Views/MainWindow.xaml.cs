using System.IO;
using System.Windows;
using Microsoft.Win32;
using RazorPad.Framework;
using RazorPad.ViewModels;

namespace RazorPad.Views
{
    public partial class MainWindow : Window
    {
        protected MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)DataContext; }
            private set { DataContext = value; }
        }

        public MainWindow()
        {
            ServiceLocator.Initialize();

            ViewModel = ServiceLocator.Get<MainWindowViewModel>();

            ViewModel.Error += OnViewModelOnError;
            ViewModel.GetSaveAsFilename += GetSaveAsFilename;

            InitializeComponent();
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

        private void OnViewModelOnError(object sender, EventArgs<string> args)
        {
            MessageBox.Show(args.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            ViewModel.SaveCurrentTemplate();
        }

        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            var filename = GetSaveAsFilename(ViewModel.CurrentTemplate);
            ViewModel.SaveCurrentTemplate(filename);
        }
    }
}
