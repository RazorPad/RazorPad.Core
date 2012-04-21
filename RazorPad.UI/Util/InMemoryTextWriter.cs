using System.ComponentModel;
using System.IO;
using System.Text;

namespace RazorPad.UI
{
    public class InMemoryTextWriter : TextWriter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private StringBuilder _builder;

        public string MessageBuffer
        {
            get { return _builder.ToString(); }
            set
            {
                _builder = new StringBuilder(value);
                OnPropertyChanged();
            }
        }

        public InMemoryTextWriter()
        {
            _builder = new StringBuilder();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _builder.Append(buffer, index, count);

            OnPropertyChanged();
        }

        private void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("MessageBuffer"));
        }

        public override void Flush()
        {
            _builder.Clear();
            base.Flush();
        }

        public override void Close()
        {
            _builder.Clear();
            base.Close();
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }
}
