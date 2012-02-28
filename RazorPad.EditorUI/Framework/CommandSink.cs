using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RazorPad.Framework
{
    /// <summary>
    /// This implementation of ICommandSink can serve as a base
    /// class for a ViewModel or as an object embedded in a ViewModel.  
    /// It provides a means of registering commands and their callback 
    /// methods, and will invoke those callbacks upon request.
    /// </summary>
    /// <remarks>
    /// Taken from Josh Smith's CodeProject MVVM article:
    /// http://www.codeproject.com/KB/WPF/VMCommanding.aspx
    /// </remarks>
    public class CommandSink : ICommandSink
    {
        readonly Dictionary<ICommand, CommandCallbacks> _commandToCallbacksMap = new Dictionary<ICommand, CommandCallbacks>();

        public virtual bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, "command");

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                return _commandToCallbacksMap[command].CanExecute(parameter);
            }

            return (handled = false);
        }

        public virtual void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, "command");

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                _commandToCallbacksMap[command].Execute(parameter);
            }
            else
            {
                handled = false;
            }
        }

        public void RegisterCommand(ICommand command, Predicate<object> canExecute, Action<object> execute)
        {
            VerifyArgument(command, "command");
            VerifyArgument(canExecute, "canExecute");
            VerifyArgument(execute, "execute");

            _commandToCallbacksMap[command] = new CommandCallbacks(canExecute, execute);
        }

        public void UnregisterCommand(ICommand command)
        {
            VerifyArgument(command, "command");

            if (_commandToCallbacksMap.ContainsKey(command))
                _commandToCallbacksMap.Remove(command);
        }

        static void VerifyArgument(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }



        private struct CommandCallbacks
        {
            public readonly Predicate<object> CanExecute;
            public readonly Action<object> Execute;

            public CommandCallbacks(Predicate<object> canExecute, Action<object> execute)
            {
                CanExecute = canExecute;
                Execute = execute;
            }
        }
    }
}