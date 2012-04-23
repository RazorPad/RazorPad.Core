using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using RazorPad.Persistence;
using RazorPad.Providers;
using RazorPad.UI.Wpf;

namespace RazorPad.ViewModels
{
    public class MainWindowViewModel : CommandSink
    {
        private readonly RazorDocumentLoader _documentLoader;

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


        public MainWindowViewModel()
        {
            _documentLoader = new RazorDocumentLoader();
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

            AddNewTemplateEditor(new RazorTemplateEditorViewModel(defaultDocument)).Execute();
        }

        internal RazorTemplateEditorViewModel AddNewTemplateEditor(string filename = null, bool current = true)
        {
            RazorTemplateEditorViewModel loadedTemplate =
                TemplateEditors
                    .Where(x => !string.IsNullOrWhiteSpace(x.Filename))
                    .SingleOrDefault(x => x.Filename.Equals(filename, StringComparison.OrdinalIgnoreCase));

            if (loadedTemplate != null)
            {
                if (current)
                    CurrentTemplate = loadedTemplate;

                return loadedTemplate;
            }

            var document = _documentLoader.Load(filename);

            return AddNewTemplateEditor(new RazorTemplateEditorViewModel(document));
        }

        internal RazorTemplateEditorViewModel AddNewTemplateEditor(RazorTemplateEditorViewModel templateEditor, bool current = true)
        {
            templateEditor.OnStatusUpdated += (sender, args) => StatusMessage = args.Message;

            TemplateEditors.Add(templateEditor);

            if (current)
                CurrentTemplate = templateEditor;

            templateEditor.Execute();

            return templateEditor;
        }

        public void SaveCurrentTemplate(string fileName = null)
        {
            var template = CurrentTemplate;
            var targetFilename = fileName ?? template.Filename;

            try
            {
                if (string.IsNullOrWhiteSpace(targetFilename))
                    throw new ApplicationException("No filename specified!");

                if (targetFilename.EndsWith(".razorpad", StringComparison.OrdinalIgnoreCase))
                    throw new NotImplementedException("Saving .razorpad documents has not been implemented yet -- coming soon!");

                template.Filename = targetFilename;

                using (var writer = new StreamWriter(File.OpenWrite(template.Filename)))
                    writer.Write(template.TemplateText);
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

    }
}
