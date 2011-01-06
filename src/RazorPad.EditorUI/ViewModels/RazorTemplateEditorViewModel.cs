using System;
using System.IO;
using System.Web.Razor;
using RazorPad.Compilation;
using RazorPad.Framework;

namespace RazorPad.ViewModels
{
    public class RazorTemplateEditorViewModel : ViewModelBase
    {
        public ITemplateCompiler TemplateCompiler { get; set; }

        public event EventHandler<EventArgs<string>> OnStatusUpdated;

        public TemplateCompilationParameters TemplateCompilationParameters
        {
            get
            {
                return TemplateCompiler == null ? null : TemplateCompiler.CompilationParameters;
            }
        }

        public RazorTemplateModelPropertiesViewModel TemplateModelProperties
        {
            get { return _templateModelProperties; }
            set
            {
                if (_templateModelProperties == value)
                    return;

                _templateModelProperties = value;
                OnPropertyChanged("TemplateModelProperties");
            }
        }
        private RazorTemplateModelPropertiesViewModel _templateModelProperties;

        public TextWriter ErrorMessages
        {
            get { return _errorMessages = _errorMessages ?? Console.Error; }
            set { _errorMessages = value; }
        }
        private TextWriter _errorMessages;

        public string ExecutedTemplateInput
        {
            get { return _executedTemplateInput; }
            set
            {
                if (_executedTemplateInput == value)
                    return;

                _executedTemplateInput = value;
                OnPropertyChanged("ExecutedTemplateInput");
            }
        }
        private string _executedTemplateInput;

        public string ExecutedTemplateOutput
        {
            get { return _executedTemplateOutput; }
            set
            {
                if (_executedTemplateOutput == value)
                    return;

                _executedTemplateOutput = value;
                OnPropertyChanged("ExecutedTemplateOutput");
            }
        }
        private string _executedTemplateOutput;

        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_filename == value)
                    return;

                _filename = value;
                OnPropertyChanged("Filename");
            }
        }
        private string _filename;

        public string GeneratedTemplateCode
        {
            get { return _generatedTemplateCode; }
            set
            {
                if (_generatedTemplateCode == value)
                    return;

                _generatedTemplateCode = value;
                OnPropertyChanged("GeneratedTemplateCode");
            }
        }
        private string _generatedTemplateCode;

        public string TemplateText
        {
            get { return _templateText; }
            set
            {
                if (_templateText == value)
                    return;

                _templateText = value;
                OnPropertyChanged("TemplateText");
            }
        }

        public bool CanSaveToCurrentlyLoadedFile
        {
            get { return !string.IsNullOrWhiteSpace(Filename); }
        }

        public bool CanSaveAsNewFilename
        {
            get { return true; }
        }

        private string _templateText;


        public RazorTemplateEditorViewModel(string filename = null)
        {
            TemplateCompiler = new TemplateCompiler();

            if(!string.IsNullOrWhiteSpace(filename))
                LoadFromFile(filename);

            TemplateText = "Hello, my name is @Model.Name!";
            TemplateModelProperties = new RazorTemplateModelPropertiesViewModel(typeof(object));
            TemplateModelProperties.Properties.Add("Name", "Razor Pad");
        }


        public void Parse()
        {
            UpdateStatus("Parsing template...");

            GeneratedTemplateCode = string.Empty;

            GeneratorResults results;

            using (StringWriter writer = new StringWriter())
            {
                results = TemplateCompiler.GenerateCode(TemplateText, writer);
                GeneratedTemplateCode = writer.ToString();
            }

            if (results.Success)
            {
                UpdateStatus("Template successfully parsed");

                Type templateModelType = TemplateCompiler.GetTemplateModelType(TemplateText);
                TemplateModelProperties = new RazorTemplateModelPropertiesViewModel(templateModelType);
            }
            else
            {
                UpdateStatus("Template parsing failed!");

                ErrorMessages.WriteLine("***  Template Parsing Failed  ***");
                foreach (var error in results.ParserErrors)
                {
                    ErrorMessages.WriteLine("Line {0}: {1}", error.Location.LineIndex, error.Message);
                }
            }
        }

        public void Execute()
        {
            Parse();

            UpdateStatus("Executing template...");

            var model = TemplateModelProperties.Properties;

            ExecutedTemplateOutput = TemplateCompiler.Execute(TemplateText, model);

            UpdateStatus("Success!");
        }

        public void LoadFromFile(string fileName)
        {
            using (var reader = new StreamReader(File.OpenRead(fileName)))
                TemplateText = reader.ReadToEnd();

            Filename = fileName;
        }

        public void SaveToFile(string fileName = null)
        {
            var targetFilename = fileName ?? Filename;

            if (string.IsNullOrWhiteSpace(targetFilename))
                throw new ApplicationException("No filename specified!");

            using (var writer = new StreamWriter(File.OpenWrite(targetFilename)))
                writer.Write(TemplateText);
        }

        private void UpdateStatus(string statusMessage)
        {
            SafeInvoke(OnStatusUpdated, new EventArgs<string>(statusMessage));
        }
    }
}