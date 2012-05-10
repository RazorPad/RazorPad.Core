using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using RazorPad.Compilation.Hosts;
using RazorPad.Providers;
using RazorPad.UI;
using RazorPad.UI.Theming;
using RazorPad.ViewModels;

namespace RazorPad.Views
{
    public partial class MainWindow : Window
    {
        private IEnumerable<string> _coreReferences;

        protected MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)DataContext; }
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

            var themeLoader = ServiceLocator.Get<ThemeLoader>();
            var themes = themeLoader.LoadThemes();

            ViewModel = ServiceLocator.Get<MainWindowViewModel>();
            ViewModel.GetReferencesThunk = GetReferences;
            ViewModel.Messages = traceWriter;
            ViewModel.Themes = new ObservableCollection<Theme>(themes);

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


        private IEnumerable<string> GetReferences(IEnumerable<string> loadedReferences)
        {
            var references = loadedReferences.ToArray();

            var assemblyReferences = references.Select(s =>
                new AssemblyReference(s)
                {
                    IsNotReadOnly = !CoreReferences.Contains(s),
                    IsInstalled = true,
                });

            var dialogDataContext = new ReferencesViewModel(assemblyReferences);
            var dlg = new ReferencesDialogWindow
            {
                Owner = this,
                DataContext = dialogDataContext
            };

            dlg.ShowDialog();

            if (dlg.DialogResult == true)
                references = dialogDataContext.InstalledReferences.References.Select(reference => reference.Location).ToArray();

            return references;
        }
    }
}
