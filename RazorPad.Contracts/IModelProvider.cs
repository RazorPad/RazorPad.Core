using System;

namespace RazorPad
{
    public interface IModelProvider
    {
        event EventHandler<RazorPadErrorEventArgs> Error;
        
        event EventHandler ModelChanged;

        dynamic GetModel();

        string Serialize();
        void Deserialize(string serialized);
    }
}
