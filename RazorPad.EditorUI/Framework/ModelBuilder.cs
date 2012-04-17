using System.Windows.Controls;

namespace RazorPad.Framework
{
    public abstract class ModelBuilder : UserControl
    {
        public virtual IModelProvider ModelProvider
        {
            get { return (IModelProvider)DataContext; }
            set { DataContext = value; }
        }
    }
}