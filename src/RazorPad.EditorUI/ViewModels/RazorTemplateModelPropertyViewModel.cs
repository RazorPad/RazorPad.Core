using System.Linq;
using RazorPad.Framework;

namespace RazorPad.ViewModels
{
    public class RazorTemplateModelPropertyViewModel : ViewModelBase
    {
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private string _name;

        public object Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnPropertyChanged("Value");
            }
        }
        private object _value;

        public RazorTemplateModelPropertyViewModel(string name = null)
        {
            Name = name;
        }

        public void ApplyValueTo(object targetObject)
        {
            if (targetObject == null)
                return;

            var setMethod =
                    targetObject.GetType()
                        .GetProperties()
                        .Where(x => x.Name == Name)
                        .Select(x => x.GetSetMethod())
                        .SingleOrDefault();

            if (setMethod != null)
                setMethod.Invoke(targetObject, new[] { Value });
        }
    }
}