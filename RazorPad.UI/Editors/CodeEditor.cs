using System;
using System.Diagnostics;
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
        public bool SuspendEditorUpdate { get; private set; }
        public TextEditor Editor { get; private set; }

        private DispatcherTimer _textChangedTimer;

        public CodeEditor()
        {
            InitializeEditor();
            InitializeTextChangedTimer();
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

        private void InitializeTextChangedTimer()
        {
            _textChangedTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _textChangedTimer.Tick += (sender, args) =>
            {
                //Debug.Print("codeEditor: TimerTick");
                SuspendEditorUpdate = true;
                Text = Editor.Text;
                SuspendEditorUpdate = false;
            };
        }

        protected void InitializeFolding(AbstractFoldingStrategy foldingStrategy)
        {
            var foldingManager = FoldingManager.Install(Editor.TextArea);
            foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);

            var foldingUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            foldingUpdateTimer.Tick += (o, args) => foldingStrategy.UpdateFoldings(foldingManager, Editor.Document);

            foldingUpdateTimer.Start();
        }

        #region TextProperty

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        public static void OnTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Print("codeEditor: OnTextChanged");
            var codeEditor = sender as CodeEditor;

            if (!codeEditor.SuspendEditorUpdate)
                codeEditor.Editor.Text = (e.NewValue ?? string.Empty).ToString();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(String), typeof(CodeEditor), new FrameworkPropertyMetadata(string.Empty, OnTextChanged)
            {
                BindsTwoWayByDefault = true,
            }); 

        #endregion
    }
}
