using System;

namespace RazorPad
{
    public interface IModelProvider
    {
        event EventHandler ModelChanged;

        Type ModelType { get; }

        dynamic GetModel();

        string Serialize();
        void Deserialize(string serialized);
    }
}
