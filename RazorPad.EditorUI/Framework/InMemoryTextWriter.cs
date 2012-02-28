using System.ComponentModel;
using System.IO;
using System.Text;

namespace RazorPad.Framework
{
    public class InMemoryTextWriter : TextWriter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly StringBuilder _builder;

        public InMemoryTextWriter()
        {
            _builder = new StringBuilder();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _builder.Append(buffer, index, count);

            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Buffer"));
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
