using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RazorPad.Framework;
using RazorPad.Persistence;
using RazorPad.Providers;
using RazorPad.UI;
using RazorPad.UI.ModelBuilders;
using RazorPad.UI.Wpf;

namespace RazorPad.ViewModels
{
    [Export]
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly RazorDocumentManager _documentManager;
        private readonly ModelProviders _modelProviders;
        private readonly ModelBuilders _modelBuilders;

        public event EventHandler<EventArgs<string>> Error;

        public ICommand AnchorableCloseCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }

        // Use thunks to create test seams
        internal Func<RazorTemplateEditorViewModel, MessageBoxResult> ConfirmSaveDirtyDocumentThunk =
            MessageBoxHelpers.ShowConfirmSaveDirtyDocumentMessageBox;

        internal Func<string> GetOpenFilenameThunk =
            MessageBoxHelpers.ShowOpenFileDialog;

        internal Func<RazorTemplateEditorViewModel, string> GetSaveAsFilenameThunk =
            MessageBoxHelpers.ShowSaveAsDialog;

        internal Action<string> ShowErrorThunk =
            MessageBoxHelpers.ShowErrorMessageBox;


        public RazorTemplateEditorViewModel CurrentTemplate
        {
            get { return _currentTemplate; }
            set
            {
                if (_currentTemplate == value)
                    return;

                _currentTemplate = value;
                OnPropertyChanged("CurrentTemplate");
                OnPropertyChanged("HasCurrentTemplate");
            }
        }
        private RazorTemplateEditorViewModel _currentTemplate;

        public bool HasCurrentTemplate
        {
            get { return CurrentTemplate != null; }
        }

        public ObservableCollection<RazorTemplateEditorViewModel> TemplateEditors
        {
            get;
            private set;
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage == value)
                    return;

                _statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        public ObservableTextWriter Messages { get; set; }

        private string _statusMessage;


        [ImportingConstructor]
        public MainWindowViewModel(RazorDocumentManager documentManager, ModelProviders modelProviders, ModelBuilders modelBuilders)
        {
            _documentManager = documentManager;
            _modelBuilders = modelBuilders;
            _modelProviders = modelProviders;

            TemplateEditors = new ObservableCollection<RazorTemplateEditorViewModel>();

            RegisterCommands();
        }


        private void RegisterCommands()
        {
            AnchorableCloseCommand = new RelayCommand(
                OnAnchorableClosed, 
                () => false);

            CloseCommand = new RelayCommand(
                    p => Close(CurrentTemplate),
                    p => HasCurrentTemplate
                );

            NewCommand = new RelayCommand(() => AddNewTemplateEditor());

            OpenCommand = new RelayCommand(p =>
            {
                var filename = GetOpenFilenameThunk();
                AddNewTemplateEditor(filename);
            });

            SaveCommand = new RelayCommand(
                    p => Save(CurrentTemplate),
                    p => HasCurrentTemplate
                );

            SaveAsCommand = new RelayCommand(
                    p => SaveAs(CurrentTemplate.Document),
                    p => HasCurrentTemplate && CurrentTemplate.CanSaveAsNewFilename
                );
        }

        private void OnAnchorableClosed()
        {
            
        }

        internal void AddNewTemplateEditor(string filename = null, bool current = true)
        {
            RazorTemplateEditorViewModel loadedTemplate =
                TemplateEditors
                    .Where(x => !string.IsNullOrWhiteSpace(x.Filename))
                    .SingleOrDefault(x => x.Filename.Equals(filename, StringComparison.OrdinalIgnoreCase));

            if (loadedTemplate != null)
            {
                if (current)
                    CurrentTemplate = loadedTemplate;

                return;
            }

            RazorDocument document;

            if (string.IsNullOrWhiteSpace(filename))
                document = new RazorDocument();
            else
                document = _documentManager.Load(filename);

            AddNewTemplateEditor(document, current);
        }

        internal void AddNewTemplateEditor(RazorDocument document, bool current = true)
        {
            AddNewTemplateEditor(new RazorTemplateEditorViewModel(document, _modelBuilders, _modelProviders), current);
        }

        internal void AddNewTemplateEditor(RazorTemplateEditorViewModel templateEditor, bool current = true)
        {
            templateEditor.OnStatusUpdated += (sender, args) =>
                                                  {
                                                      Trace.TraceInformation(args.Message);
                                                      StatusMessage = args.Message;
                                                  };

            templateEditor.Messages = Messages;

            TemplateEditors.Add(templateEditor);

            if (current)
                CurrentTemplate = templateEditor;

            templateEditor.Execute();
        }


        public void Close(RazorTemplateEditorViewModel document, bool? save = null)
        {
            if (document.IsDirty && save.GetValueOrDefault(true))
            {
                var shouldSave = ConfirmSaveDirtyDocumentThunk(document);

                switch (shouldSave)
                {
                    case MessageBoxResult.Cancel:
                        return;

                    case MessageBoxResult.Yes:
                        Save(document);
                        break;
                }
            }

            TemplateEditors.Remove(document);
        }

        public void Save(RazorTemplateEditorViewModel document)
        {
            var filename = document.Filename;

            if (document.CanSaveToCurrentlyLoadedFile)
                filename = null;

            SaveAs(document.Document, filename);
        }

        public void SaveAs(RazorDocument document, string filename = null)
        {
            try
            {
                if (filename == null)
                    filename = GetSaveAsFilenameThunk(CurrentTemplate);

                if (string.IsNullOrWhiteSpace(filename))
                {
                    Trace.TraceWarning("Filename is empty - skipping save");
                    return;
                }

                _documentManager.Save(document, filename);

                if (!filename.Equals(document.Filename, StringComparison.OrdinalIgnoreCase))
                    document.Filename = filename;
            }
            catch (Exception ex)
            {
                Error.SafeInvoke(ex.Message);
            }
        }
    }
}
