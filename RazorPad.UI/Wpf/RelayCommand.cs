using System;
using System.Windows.Input;

namespace RazorPad.UI.Wpf
{
    public class RelayCommand : ICommand
    {
        private static readonly Func<bool> True = () => true;

        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private readonly Predicate<object> _canExecute;


        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(p => execute(), null)
        {
            if (canExecute != null)
                _canExecute = p => canExecute();
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}