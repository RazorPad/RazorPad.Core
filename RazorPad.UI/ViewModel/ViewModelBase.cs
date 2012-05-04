using System.ComponentModel;
using System.Windows.Threading;

namespace RazorPad.UI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected internal Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
