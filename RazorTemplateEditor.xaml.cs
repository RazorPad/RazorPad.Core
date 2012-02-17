using System;
using System.Windows.Controls;
using System.Windows.Forms;
using RazorPad.ViewModels;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace RazorPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RazorTemplateEditor : UserControl
    {
        private static readonly int TemplateTextChangedEventDelay = (int)TimeSpan.FromSeconds(.5).TotalMilliseconds;


        Timer _templateTextChangedTimer;

        protected RazorTemplateEditorViewModel ViewModel
        {
            get { return (RazorTemplateEditorViewModel) DataContext; }
        }

        public RazorTemplateEditor()
        {
            InitializeComponent();
            InitializeTemplateTextChangedTimer();
        }

        private void InitializeTemplateTextChangedTimer()
        {
            _templateTextChangedTimer = new Timer { Interval = TemplateTextChangedEventDelay };

            _templateTextChangedTimer.Tick += (x, y) =>
            {
                TemplateTextBox
                    .GetBindingExpression(TextBox.TextProperty)
                    .UpdateSource();

                _templateTextChangedTimer.Stop();
            };
        }

        private void TemplateTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_templateTextChangedTimer.Enabled)
                _templateTextChangedTimer.Stop();
            
            _templateTextChangedTimer.Start();
        }
    }
}
