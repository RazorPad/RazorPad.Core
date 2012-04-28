using System;
using System.Diagnostics.Contracts;

namespace RazorPad.UI.ModelBuilders
{
    public abstract class ModelBuilderViewModel : ViewModelBase
    {
        public IModelProvider ModelProvider
        {
            get { return _modelProvider; }
            set
            {
                if (_modelProvider == value)
                    return;

                if (_modelProvider != null)
                    _modelProvider.ModelChanged -= InternalOnModelChangedHandler;

                _modelProvider = value;

                if (_modelProvider != null)
                    _modelProvider.ModelChanged += InternalOnModelChangedHandler;

                OnPropertyChanged("ModelProvider");
            }
        }
        private IModelProvider _modelProvider;

        protected ModelBuilderViewModel(IModelProvider modelProvider)
        {
            Contract.Requires(modelProvider != null);
            ModelProvider = modelProvider;
        }

        private void InternalOnModelChangedHandler(object sender, EventArgs args)
        {
            OnModelChanged();
        }

        protected virtual void OnModelChanged()
        {
        }
    }

    public abstract class ModelBuilderViewModel<TModelProvider> : ModelBuilderViewModel
        where TModelProvider : class, IModelProvider
    {
        public new TModelProvider ModelProvider
        {
            get { return (TModelProvider)base.ModelProvider; }
            set { base.ModelProvider = value; }
        }

        protected ModelBuilderViewModel(TModelProvider modelProvider)
            : base(modelProvider)
        {
        }
    }
}