using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using RazorPad.Compilation.Hosts;
using RazorPad.Framework;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using RazorPad.Compilation.Hosts;
using RazorPad.ViewModels;

namespace RazorPad.Views
{
    public partial class MainWindow : Window
    {
        private IEnumerable<string> _coreReferences;

        protected MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel) DataContext; }
            private set { DataContext = value; }
        }

        protected IEnumerable<string> CoreReferences
        {
            get { return _coreReferences ?? (_coreReferences = RazorPadHost.DefaultIncludes.Select(di => di.Location)); }
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


        private void ManageReference_Click(object sender, RoutedEventArgs e)
        {
            var loadedReferencesTempArray =
                new string[
                    ViewModel.CurrentTemplate.TemplateCompiler.CompilationParameters.CompilerParameters.
                        ReferencedAssemblies.Count];

            // get the loaded assembly names from the stupid collection to an enumerable one
            ViewModel.CurrentTemplate.TemplateCompiler.CompilationParameters.CompilerParameters.ReferencedAssemblies.
                CopyTo(
                    loadedReferencesTempArray, 0);

            var loadedReferences = loadedReferencesTempArray
                .Select(s =>
                        new Reference(s)
                            {
                                //Filters = 
                                //{
                                IsNotReadOnly = !CoreReferences.Contains(s),
                                IsInstalled = true,
                                //}
                            });

            var dialogDataContext = new ReferencesViewModel(loadedReferences);
            var dlg = new ReferencesDialogWindow
                          {
                              Owner = this,
                              DataContext = dialogDataContext
                          };

            dlg.ShowDialog();

            if (dlg.DialogResult != true) return;

            // clear existing ones
            ViewModel.CurrentTemplate.TemplateCompiler.CompilationParameters.CompilerParameters.ReferencedAssemblies.
                Clear();

            foreach (var reference in dialogDataContext.InstalledReferences.References)
                ViewModel.CurrentTemplate.TemplateCompiler.CompilationParameters.AddAssemblyReference(reference.Location);
        }
    }
}
