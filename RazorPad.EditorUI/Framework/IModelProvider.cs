using System;

namespace RazorPad.Framework
{
    public interface IModelProvider
    {
        event EventHandler ModelChanged;

        Type ModelType { get; }

        dynamic GetModel();
    }
}
