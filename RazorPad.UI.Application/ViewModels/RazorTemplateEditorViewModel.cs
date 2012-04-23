using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Razor;
using RazorPad.Compilation;
using RazorPad.Framework;
using RazorPad.Persistence;
using RazorPad.UI;
using RazorPad.UI.Json;
using RazorPad.UI.ModelBuilders;

namespace RazorPad.ViewModels
{
    public class RazorTemplateEditorViewModel : ViewModelBase
    {
        private readonly ModelBuilderFactory _modelBuilderFactory;
        private readonly RazorDocumentLoader _documentLoader;

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

        public InMemoryTextWriter Messages
        {
            get { return _messages; }
            set
            {
                if (_messages == value)
                    return;

                _messages = value;
                OnPropertyChanged("Messages");
            }
        }
        private InMemoryTextWriter _messages;

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


        public RazorTemplateEditorViewModel(string filename = null, RazorDocumentLoader documentLoader = null, ModelBuilderFactory modelBuilderFactory = null)
        {
            _modelBuilderFactory = modelBuilderFactory ?? new ModelBuilderFactory();
            _documentLoader = documentLoader ?? new RazorDocumentLoader();

            Messages = new InMemoryTextWriter();
            TemplateCompiler = new TemplateCompiler();

            if(!string.IsNullOrWhiteSpace(filename))
                LoadFromFile(filename);

            Execute();
        }


        public void Parse()
        {
            UpdateStatus("Parsing template...");

            GeneratedTemplateCode = string.Empty;

            using (var writer = new StringWriter())
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

                Log("***  Template Parsing Failed  ***");

                if (GeneratorResults != null)
                {
                    var errorBuilder = new StringBuilder();
                    foreach (var error in GeneratorResults.ParserErrors)
                    {
                        errorBuilder.AppendFormat("\nLine {0}: {1}", error.Location.LineIndex, error.Message);
                    }

                    var errors = errorBuilder.ToString();
                    Log(errors);
                    ExecutedTemplateOutput = errors;
                }
            }
        }

        public void Execute()
        {
            Messages.Flush();

            Log("Parsing template...");
            Parse();

            try
            {
                Log("Retrieving model...");
                var model = ModelProvider.GetModel();

                Log("Executing template...");
                ExecutedTemplateOutput = TemplateCompiler.Execute(TemplateText, model);
                Log("Success!");

                UpdateStatus("Success!");
            }
            catch (Exception ex)
            {
                Log(ex);
                UpdateStatus(ex.Message);
            }
        }

        private void Log(string message)
        {
            Messages.WriteLine("[{0}]  {1}", DateTime.Now.ToShortTimeString(), message);
        }

        private void Log(Exception ex)
        {
            Messages.WriteLine("[{0}]  {1}\r\n{2}", DateTime.Now.ToShortTimeString(), ex.Message, ex.StackTrace);
        }

        public void LoadFromFile(string fileName)
        {
            try
            {
                var document = _documentLoader.Load(fileName);
                Filename = document.Filename;
                ModelBuilder = _modelBuilderFactory.Create(document.ModelProvider);
                TemplateText = document.Template;
                ModelProvider.TriggerModelChanged();
            }
            catch (Exception ex)
            {
                Log(ex);
                UpdateStatus(ex.Message);
            }
        }

        public void SaveToFile(string fileName = null)
        {
            var targetFilename = fileName ?? Filename;

            try
            {
                if (string.IsNullOrWhiteSpace(targetFilename))
                    throw new ApplicationException("No filename specified!");

                if (targetFilename.EndsWith(".razorpad", StringComparison.OrdinalIgnoreCase))
                    throw new NotImplementedException("Saving .razorpad documents has not been implemented yet -- coming soon!");

                Filename = targetFilename;

                using (var writer = new StreamWriter(File.OpenWrite(Filename)))
                    writer.Write(TemplateText);
            }
            catch (Exception ex)
            {
                Log(ex);
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