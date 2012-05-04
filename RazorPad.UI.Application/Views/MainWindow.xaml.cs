using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using RazorPad.Compilation.Hosts;
using RazorPad.Providers;
using RazorPad.UI;
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
            get
            {
                return _coreReferences ?? (_coreReferences = RazorPadHost.DefaultIncludes.Select(di => di.Location));
            }
        }

        public MainWindow()
        {
            // TODO: Replace with real logging
            var traceWriter = new ObservableTextWriter();
            Trace.Listeners.Add(new TextWriterTraceListener(traceWriter) { TraceOutputOptions = TraceOptions.None });

            Trace.TraceInformation("Initializing application...");

            ServiceLocator.Initialize();

            ViewModel = ServiceLocator.Get<MainWindowViewModel>();
            ViewModel.Messages = traceWriter;
            
            CreateDemoTemplate();

            InitializeComponent();
            
            Trace.TraceInformation("Done initializing");
        }

        private void CreateDemoTemplate()
        {
            var demoDocument = new RazorDocument
            {
                Template = "<h1>Welcome to @Model.Name!</h1>\r\n<div>Start typing some text to get started.</div>\r\n<div>Or, try adding a property called 'Message' and see what happens...</div>\r\n\r\n<h3>@Model.Message</h3>",
                ModelProvider = new JsonModelProvider(json: "{\r\n\tName: 'RazorPad'\r\n}")
            };

            ViewModel.AddNewTemplateEditor(demoDocument);
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
                                IsNotReadOnly = !CoreReferences.Contains(s),
                                IsInstalled = true,
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
