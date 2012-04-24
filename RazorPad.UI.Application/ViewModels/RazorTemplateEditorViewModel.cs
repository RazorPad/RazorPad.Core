using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Razor;
using RazorPad.Compilation;
using RazorPad.Framework;
using RazorPad.UI;
using RazorPad.UI.ModelBuilders;

namespace RazorPad.ViewModels
{
    public class RazorTemplateEditorViewModel : ViewModelBase
    {
        private readonly ModelBuilderFactory _modelBuilderFactory;
        private readonly RazorDocument _document;

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

        public ModelBuilder ModelBuilder
        {
            get { return _modelBuilderFactory.Create(_document.ModelProvider); }
        }

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
            get { return _document.Filename; }
            set
            {
                if (_document.Filename == value)
                    return;

                _document.Filename = value;
                OnPropertyChanged("Filename");
            }
        }

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

        public string Template
        {
            get { return _document.Template; }
            set
            {
                if (_document.Template == value)
                    return;

                _document.Template = value;
                OnPropertyChanged("Template");
                Refresh();
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


        public RazorTemplateEditorViewModel(RazorDocument document = null, ModelBuilderFactory modelBuilderFactory = null)
        {
            _document = document ?? new RazorDocument();
            _modelBuilderFactory = modelBuilderFactory ?? new ModelBuilderFactory();
            Messages = new InMemoryTextWriter();
            TemplateCompiler = new TemplateCompiler();

            if (_document.ModelProvider != null)
                _document.ModelProvider.ModelChanged += (sender, args) => Refresh();
        }


        public void Parse()
        {
            UpdateStatus("Parsing template...");

            GeneratedTemplateCode = string.Empty;

            using (var writer = new StringWriter())
            {
                GeneratorResults = TemplateCompiler.GenerateCode(_document, writer);
                
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
                Log("Executing template...");
                ExecutedTemplateOutput = TemplateCompiler.Execute(_document);
                Log("Success!");

                UpdateStatus("Success!");
            }
            catch (Exception ex)
            {
                Log(ex);
                UpdateStatus(ex.Message);
            }
        }

        public void Refresh()
        {
            Execute();
        }

        private void Log(string message)
        {
            Messages.WriteLine("[{0}]  {1}", DateTime.Now.ToShortTimeString(), message);
        }

        private void Log(Exception ex)
        {
            Messages.WriteLine("[{0}]  {1}\r\n{2}", DateTime.Now.ToShortTimeString(), ex.Message, ex.StackTrace);
        }

        private void UpdateStatus(string statusMessage)
        {
            OnStatusUpdated.SafeInvoke(statusMessage);
        }
    }
}