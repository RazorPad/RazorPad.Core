using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Razor;
using RazorPad.Compilation;
using RazorPad.Framework;
using RazorPad.UI;

namespace RazorPad.ViewModels
{
    public class RazorTemplateEditorViewModel : ViewModelBase
    {
        public ITemplateCompiler TemplateCompiler { get; set; }

        public event EventHandler<EventArgs<string>> OnStatusUpdated;

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Filename))
                    return "New File";
                
                return new FileInfo(Filename).Name;
            }
        }

        public TemplateCompilationParameters TemplateCompilationParameters
        {
            get
            {
                return TemplateCompiler == null ? null : TemplateCompiler.CompilationParameters;
            }
        }


        public ModelBuilder ModelBuilder
        {
            get { return _modelBuilder; }
            set
            {
                _modelBuilder = value;

                if (value != null && ModelProvider == null && _modelBuilder.ModelProvider != null)
                    ModelProvider = _modelBuilder.ModelProvider;
                
                OnPropertyChanged("ModelBuilder");
            }
        }
        private ModelBuilder _modelBuilder;

        public IModelProvider ModelProvider
        {
            get { return _modelProvider; }
            set
            {
                if (_modelProvider == value)
                    return;

                if (_modelProvider != null)
                    _modelProvider.ModelChanged -= TriggerRefresh;

                if (value != null)
                    value.ModelChanged += TriggerRefresh;

                if(ModelBuilder != null)
                    ModelBuilder.ModelProvider = value;

                _modelProvider = value;

                OnPropertyChanged("ModelProvider");
            }
        }
        private IModelProvider _modelProvider;

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

        public GeneratorResults GeneratorResults
        {
            get { return _generatorResults; }
            set
            {
                if (_generatorResults == value)
                    return;

                _generatorResults = value;
                OnPropertyChanged("GeneratorResults");
            }
        }
        private GeneratorResults _generatorResults;

        public string TemplateText
        {
            get { return _templateText; }
            set
            {
                if (_templateText == value)
                    return;

                _templateText = value;
                OnPropertyChanged("TemplateText");
                Refresh();
            }
        }
        private string _templateText;

        public bool CanSaveToCurrentlyLoadedFile
        {
            get { return !string.IsNullOrWhiteSpace(Filename); }
        }

        public bool CanSaveAsNewFilename
        {
            get { return true; }
        }



        public RazorTemplateEditorViewModel(string filename = null)
        {
            TemplateCompiler = new TemplateCompiler();

            if(!string.IsNullOrWhiteSpace(filename))
                LoadFromFile(filename);

            Execute();
        }


        public void Parse()
        {
            UpdateStatus("Parsing template...");

            GeneratedTemplateCode = string.Empty;

            using (StringWriter writer = new StringWriter())
            {
                GeneratorResults = TemplateCompiler.GenerateCode(TemplateText, writer);
                
                var generatedCode = writer.ToString();
                generatedCode = Regex.Replace(generatedCode, "//.*", string.Empty);
                generatedCode = Regex.Replace(generatedCode, "#.*", string.Empty);

                GeneratedTemplateCode = generatedCode.Trim();
            }

            if (GeneratorResults != null && GeneratorResults.Success)
            {
                UpdateStatus("Template successfully parsed");
            }
            else
            {
                UpdateStatus("Template parsing failed!");

                ErrorMessages.WriteLine("***  Template Parsing Failed  ***");
                if (GeneratorResults != null)
                {
                    var errorBuilder = new StringBuilder();
                    foreach (var error in GeneratorResults.ParserErrors)
                    {
                        errorBuilder.AppendFormat("\nLine {0}: {1}", error.Location.LineIndex, error.Message);
                    }

                    var errors = errorBuilder.ToString();
                    ErrorMessages.WriteLine(errors);
                    ExecutedTemplateOutput = errors;
                }
            }
        }

        public void Execute()
        {
            UpdateStatus("Parsing template...");
            Parse();

            try
            {
                UpdateStatus("Retrieving model...");
                var model = ModelProvider.GetModel();

                UpdateStatus("Executing template...");
                ExecutedTemplateOutput = TemplateCompiler.Execute(TemplateText, model);
                UpdateStatus("Success!");
            }
            catch (Exception ex)
            {
                ErrorMessages.WriteLine(ex);
                UpdateStatus(ex.Message);
            }
        }

        public void LoadFromFile(string fileName)
        {
            try
            {
                using (var reader = new StreamReader(File.OpenRead(fileName)))
                    TemplateText = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                ErrorMessages.WriteLine(ex);
                UpdateStatus(ex.Message);
            }

            Filename = fileName;
        }

        public void SaveToFile(string fileName = null)
        {
            var targetFilename = fileName ?? Filename;

            try
            {
                if (string.IsNullOrWhiteSpace(targetFilename))
                    throw new ApplicationException("No filename specified!");

                Filename = targetFilename;

                using (var writer = new StreamWriter(File.OpenWrite(Filename)))
                    writer.Write(TemplateText);
            }
            catch (Exception ex)
            {
                ErrorMessages.WriteLine(ex);
                UpdateStatus(ex.Message);
            }
        }

        private void UpdateStatus(string statusMessage)
        {
            OnStatusUpdated.SafeInvoke(statusMessage);
        }

        public void Refresh()
        {
            Execute();
        }

        private void TriggerRefresh(object sender, EventArgs args)
        {
            Refresh();
        }
    }
}