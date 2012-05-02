using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
            get
            {
                return _coreReferences ?? (_coreReferences = RazorPadHost.DefaultIncludes.Select(di => di.Location));
            }
        }

        public MainWindow()
        {
            ServiceLocator.Initialize();

            ViewModel = ServiceLocator.Get<MainWindowViewModel>();

            InitializeComponent();
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
