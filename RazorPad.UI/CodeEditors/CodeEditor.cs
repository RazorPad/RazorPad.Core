using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;

namespace RazorPad.UI.CodeEditors
{
	public class CodeEditor : TextEditor
	{
        public static readonly DependencyProperty DelayedTextProperty = 
            DependencyProperty.Register(
                "DelayedText", typeof(String), typeof(CodeEditor),
                new FrameworkPropertyMetadata(string.Empty) { BindsTwoWayByDefault = false }
            );

        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register(
                "Text", typeof(String), typeof(CodeEditor),
                new FrameworkPropertyMetadata(string.Empty, OnTextPropertyChanged) { BindsTwoWayByDefault = false }
            );

        private readonly Timer _textChangedTimer;

        public double DelayedTextChangedDelay
        {
            get { return _textChangedTimer.Interval; }
            set { _textChangedTimer.Interval = value; }
        }

	    public string DelayedText
	    {
            get { return (string)GetValue(DelayedTextProperty); }
            set { SetValue(DelayedTextProperty, value); }
	    }

	    public new string Text
	    {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
	    }

        
        public CodeEditor()
		{
            ShowLineNumbers = true;

            _textChangedTimer = new Timer { Interval = TimeSpan.FromSeconds(.5).TotalMilliseconds };
            _textChangedTimer.Elapsed += (x, y) => {
                    Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(UpdateDelayedText));
                    _textChangedTimer.Stop();
                };

            TextChanged += OnTextChanged;
        }

	    private void UpdateDelayedText()
	    {
	        if (Document != null)
	        {
	            DelayedText = Document.Text;
	        }
	    }

	    private void OnTextChanged(object sender, EventArgs e)
        {
            if (_textChangedTimer.Enabled)
                _textChangedTimer.Stop();

            _textChangedTimer.Start();
        }

        private static void OnTextPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var editor = sender as CodeEditor;
            if (editor == null || editor.Document == null) return;
            editor.Document.Text = e.NewValue as string ?? string.Empty;
        }

        protected void InitializeFolding(AbstractFoldingStrategy foldingStrategy)
        {
            // TODO
        }
	}
}
