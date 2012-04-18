using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RazorPad.UI.Wpf
{
    public class DelayedNotifyPropertyChangedTextBox : TextBox
    {
        public static readonly int DefaultTextChangedEventDelay = (int)TimeSpan.FromSeconds(.5).TotalMilliseconds;

        private readonly Timer _textChangedTimer;

        public double TextChangedEventDelay
        {
            get { return _textChangedTimer.Interval; }
            set { _textChangedTimer.Interval = value; }
        }

        public DelayedNotifyPropertyChangedTextBox()
        {
            _textChangedTimer = new Timer { Interval = DefaultTextChangedEventDelay };
            InitializeTextChangedTimer();
            TextChanged += OnTextChanged;
        }

        private void InitializeTextChangedTimer()
        {
            _textChangedTimer.Elapsed += (x, y) =>
            {
                var bindingExpression = GetBindingExpression(TextProperty);

                if(bindingExpression != null)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.DataBind, 
                        new Action(bindingExpression.UpdateSource));
                }

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
