using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;

namespace RazorPad.UI.Editors
{
    public class CodeEditor : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string), typeof(CodeEditor), new FrameworkPropertyMetadata(string.Empty, OnTextChanged)
            {
                BindsTwoWayByDefault = true,
            });

        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register("ReadOnly",
            typeof(bool), typeof(CodeEditor), new FrameworkPropertyMetadata(false, OnReadOnlyChanged)
            {
                BindsTwoWayByDefault = true,
            });


        private DispatcherTimer _textChangedTimer;

        public TextEditor Editor { get; private set; }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public bool SuspendEditorUpdate { get; private set; }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public CodeEditor()
        {
            InitializeEditor();
            InitializeTextChangedTimer();
        }


        private void InitializeTextChangedTimer()
        {
            _textChangedTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _textChangedTimer.Tick += (sender, args) =>
            {
                SuspendEditorUpdate = true;
                Text = Editor.Text;
                SuspendEditorUpdate = false;
            };
        }

        private void InitializeEditor()
        {
            Editor = new TextEditor
                         {
                             ShowLineNumbers = true,
                             FontFamily = new FontFamily("Consolas"),
                             FontSize = (double)(new FontSizeConverter().ConvertFrom("10pt") ?? 10.0)
                         };

            Editor.KeyUp += (sender, args) =>
                                {
                                    if (_textChangedTimer.IsEnabled)
                                        _textChangedTimer.Stop();

                                    _textChangedTimer.Start();

                                };
            AddChild(Editor);
        }

        protected void InitializeFolding(AbstractFoldingStrategy foldingStrategy)
        {
            var foldingManager = FoldingManager.Install(Editor.TextArea);
            foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);

            var foldingUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            foldingUpdateTimer.Tick += (o, args) => foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);

            foldingUpdateTimer.Start();
        }


        static void OnReadOnlyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var codeEditor = sender as CodeEditor;
            if (codeEditor != null) 
                codeEditor.ReadOnly = (bool)e.NewValue;
        }

        static void OnTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var codeEditor = sender as CodeEditor;

            if (codeEditor != null && !codeEditor.SuspendEditorUpdate)
                codeEditor.Editor.Text = (e.NewValue ?? string.Empty).ToString();
        }
    }
}
