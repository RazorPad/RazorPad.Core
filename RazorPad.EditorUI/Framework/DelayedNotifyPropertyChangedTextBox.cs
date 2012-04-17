using System;
using System.Windows.Controls;
using System.Windows.Forms;
using TextBox = System.Windows.Controls.TextBox;

namespace RazorPad.Framework
{
    public class DelayedNotifyPropertyChangedTextBox : TextBox
    {
        private readonly Timer _textChangedTimer;

        public int TextChangedEventDelay
        {
            get { return _textChangedTimer.Interval; }
            set { _textChangedTimer.Interval = value; }
        }

        public DelayedNotifyPropertyChangedTextBox()
        {
            _textChangedTimer = new Timer { Interval = (int)TimeSpan.FromSeconds(.5).TotalMilliseconds };
            InitializeTextChangedTimer();
            TextChanged += OnTextChanged;
        }

        private void InitializeTextChangedTimer()
        {
            _textChangedTimer.Tick += (x, y) =>
            {
                GetBindingExpression(TextProperty).UpdateSource();
                _textChangedTimer.Stop();
            };
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textChangedTimer.Enabled)
                _textChangedTimer.Stop();
            
            _textChangedTimer.Start();
        }
    }
}
