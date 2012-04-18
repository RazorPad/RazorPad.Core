using System;

namespace RazorPad.UI
{
    public interface IModelProvider
    {
        event EventHandler ModelChanged;

        Type ModelType { get; }

        dynamic GetModel();
    }
}
