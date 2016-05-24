using System.ComponentModel;
using System.IO;
using System.Text;

namespace RazorPad.UI
{
    public class ObservableTextWriter : StringWriter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string MessageBuffer
        {
            get { return base.GetStringBuilder().ToString(); }
            set {}
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);

            OnPropertyChanged();
        }

        private void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("MessageBuffer"));
        }

        public void Clear()
        {
            base.GetStringBuilder().Clear();
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }
}
