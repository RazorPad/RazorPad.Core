using System.IO;
using System.Windows;
using Microsoft.Win32;
using RazorPad.ViewModels;

namespace RazorPad
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

            CompileButton.Click += (sender, args) => ViewModel.CurrentTemplate.Parse();
            ExecuteButton.Click += (sender, args) =>
            {
                ViewModel.CurrentTemplate.Execute();
                OutputTab.IsSelected = true;
            };
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

            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                ViewModel.AddNewTemplateEditor(dlg.FileName);
            }
        }

        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            string currentFilename = ViewModel.CurrentTemplate.Filename;

            if (!string.IsNullOrWhiteSpace(currentFilename))
                dlg.InitialDirectory = Path.GetDirectoryName(currentFilename);

            if (dlg.ShowDialog().GetValueOrDefault())
                ViewModel.CurrentTemplate.SaveToFile(dlg.FileName);
        }
    }
}
