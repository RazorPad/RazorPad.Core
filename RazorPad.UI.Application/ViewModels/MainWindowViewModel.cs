using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using RazorPad.Framework;
using RazorPad.Persistence;
using RazorPad.Providers;
using RazorPad.UI.ModelBuilders;
using RazorPad.UI.Wpf;

namespace RazorPad.ViewModels
{
    [Export]
    public class MainWindowViewModel : CommandSink
    {
        private readonly RazorDocumentManager _documentManager;
        private readonly ModelProviders _modelProviders;
        private readonly ModelBuilders _modelBuilders;

        public event EventHandler<EventArgs<string>> Error;

        public Func<RazorTemplateEditorViewModel, string> GetSaveAsFilename
        {
            get { return _getSaveAsFilename; }
            set { _getSaveAsFilename = value; }
        }
        private Func<RazorTemplateEditorViewModel, string> _getSaveAsFilename = 
            template => template.Filename;

        public RazorTemplateEditorViewModel CurrentTemplate
        {
            get { return _currentTemplate; }
            set
            {
                if (_currentTemplate == value)
                    return;

                _currentTemplate = value;
                OnPropertyChanged("CurrentTemplate");
            }
        }
        private RazorTemplateEditorViewModel _currentTemplate;

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
        private string _statusMessage;

        [ImportingConstructor]
        public MainWindowViewModel(RazorDocumentManager documentManager, ModelProviders modelProviders, ModelBuilders modelBuilders)
        {
            _documentManager = documentManager;
            _modelBuilders = modelBuilders;
            _modelProviders = modelProviders;
            
            InitializeTemplateEditors();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            RegisterCommand(ApplicationCommands.Save,
                x => CurrentTemplate != null && CurrentTemplate.CanSaveToCurrentlyLoadedFile,
                x => SaveCurrentTemplate());

            RegisterCommand(ApplicationCommands.SaveAs,
                x => CurrentTemplate != null && CurrentTemplate.CanSaveAsNewFilename,
                x => GetSaveAsFilename.Invoke(CurrentTemplate));

            RegisterCommand(ApplicationCommands.New,
                            x => CanAddNewTemplate,
                            x => AddNewTemplateEditor());
        }

        protected bool CanAddNewTemplate
        {
            get { return true; }
        }

        private void InitializeTemplateEditors()
        {
            TemplateEditors = new ObservableCollection<RazorTemplateEditorViewModel>();

            var defaultDocument = new RazorDocument {
                    Template = "<h1>Welcome to @Model.Name!</h1>\r\n<div>Start typing some text to get started.</div>\r\n<div>Or, try adding a property called 'Message' and see what happens...</div>\r\n\r\n<h3>@Model.Message</h3>",
                    ModelProvider = new JsonModelProvider(json: "{\r\n\tName: 'RazorPad'\r\n}")
                };

            AddNewTemplateEditor(new RazorTemplateEditorViewModel(defaultDocument, _modelBuilders, _modelProviders));
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

            var document = _documentManager.Load(filename);

            AddNewTemplateEditor(new RazorTemplateEditorViewModel(document, _modelBuilders, _modelProviders));
        }

        internal void AddNewTemplateEditor(RazorTemplateEditorViewModel templateEditor, bool current = true)
        {
            templateEditor.OnStatusUpdated += (sender, args) => StatusMessage = args.Message;

            TemplateEditors.Add(templateEditor);

            if (current)
                CurrentTemplate = templateEditor;

            templateEditor.Execute();
        }

        public void SaveCurrentTemplate(string fileName = null)
        {
            try
            {
                var document = CurrentTemplate.Document;

                _documentManager.Save(document, fileName);

                if(fileName != null)
                {
                    if(!fileName.Equals(document.Filename, StringComparison.OrdinalIgnoreCase))
                        CurrentTemplate.Filename = fileName;
                }
            }
            catch (Exception ex)
            {
                Error.SafeInvoke(ex.Message);
            }
        }
    }
}
