using System;
using System.Collections.ObjectModel;

namespace RazorPad
{
    public interface IModelProvider
    {
        event EventHandler ModelChanged;

        ObservableCollection<RazorPadError> Errors { get; }

        dynamic GetModel();

        string Serialize();
        void Deserialize(string serialized);
    }
}
