using System;
using System.Timers;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;

namespace RazorPad.UI.CodeEditors
{
	public class CodeEditor : TextEditor
	{
        private readonly Timer _textChangedTimer;

        public double DelayedTextChangedDelay
        {
            get { return _textChangedTimer.Interval; }
            set { _textChangedTimer.Interval = value; }
        }

	    public event EventHandler DelayedTextChanged;

        
        public CodeEditor()
		{
            ShowLineNumbers = true;

            _textChangedTimer = new Timer { Interval = TimeSpan.FromSeconds(.5).TotalMilliseconds };
            _textChangedTimer.Elapsed += (x, y) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                    new Action(() => DelayedTextChanged.SafeInvoke(sender: this)));

                _textChangedTimer.Stop();
            };
            TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (_textChangedTimer.Enabled)
                _textChangedTimer.Stop();

            _textChangedTimer.Start();
        }

        protected void InitializeFolding(AbstractFoldingStrategy foldingStrategy)
        {
            // TODO
        }
	}
}
