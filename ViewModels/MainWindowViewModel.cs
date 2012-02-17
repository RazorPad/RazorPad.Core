using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RazorPad.Framework;

namespace RazorPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
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

        public InMemoryTextWriter ErrorMessages
        {
            get { return _errorMessages; }
            set
            {
                if (_errorMessages == value)
                    return;

                _errorMessages = value;
                OnPropertyChanged("ErrorMessages");
            }
        }
        private InMemoryTextWriter _errorMessages;

        public MainWindowViewModel()
        {
            InitializeTemplateEditors();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            RegisterCommand(ApplicationCommands.Save,
                x => CurrentTemplate != null && CurrentTemplate.CanSaveToCurrentlyLoadedFile,
                x => CurrentTemplate.SaveToFile());

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
            AddNewTemplateEditor();
        }

        internal RazorTemplateEditorViewModel AddNewTemplateEditor(string filename = null, bool setAsCurrentTemplate = true)
        {
            RazorTemplateEditorViewModel loadedTemplate =
                TemplateEditors
                    .Where(x => !string.IsNullOrWhiteSpace(x.Filename))
                    .SingleOrDefault(x => x.Filename.Equals(filename, StringComparison.OrdinalIgnoreCase));

            if (loadedTemplate != null)
            {
                loadedTemplate.LoadFromFile(filename);

                if (setAsCurrentTemplate)
                    CurrentTemplate = loadedTemplate;

                return loadedTemplate;
            }

            var templateEditor = new RazorTemplateEditorViewModel(filename);
            templateEditor.ErrorMessages = ErrorMessages;
            templateEditor.OnStatusUpdated += (sender, args) => StatusMessage = args.Message;

            TemplateEditors.Add(templateEditor);

            if (setAsCurrentTemplate)
                CurrentTemplate = templateEditor;

            return templateEditor;
        }
    }
}
