using System;
using System.ComponentModel;

namespace RazorPad.Framework
{
    public abstract class ViewModelBase : CommandSink, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SafeInvoke<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs args = null)
            where TEventArgs : EventArgs
        {
            if (handler != null)
                handler(this, args);
        }
    }
}
