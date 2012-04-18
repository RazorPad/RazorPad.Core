using System;
using System.Windows;
using System.Windows.Input;

namespace RazorPad.UI.Wpf
{
    /// <summary>
    /// A CommandBinding subclass that will attach its
    /// CanExecute and Executed events to the event handling
    /// methods on the object referenced by its CommandSink property.  
    /// Set the attached CommandSink property on the element 
    /// whose CommandBindings collection contain CommandSinkBindings.
    /// If you dynamically create an instance of this class and add it 
    /// to the CommandBindings of an element, you must explicitly set
    /// its CommandSink property.
    /// </summary>
    /// <remarks>
    /// Taken from Josh Smith's CodeProject MVVM article:
    /// http://www.codeproject.com/KB/WPF/VMCommanding.aspx
    /// </remarks>
    public class CommandSinkBinding : CommandBinding
    {
        private ICommandSink _commandSink;

        #region CommandSinkProperty Dependency Property

        public static readonly DependencyProperty CommandSinkProperty =
            DependencyProperty.RegisterAttached(
                "CommandSink",
                typeof(ICommandSink),
                typeof(CommandSinkBinding),
                new UIPropertyMetadata(null, OnCommandSinkChanged));

        public static ICommandSink GetCommandSink(DependencyObject obj)
        {
            return (ICommandSink)obj.GetValue(CommandSinkProperty);
        }

        public static void SetCommandSink(DependencyObject obj, ICommandSink value)
        {
            obj.SetValue(CommandSinkProperty, value);
        }

        private static void OnCommandSinkChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var commandSink = e.NewValue as ICommandSink;

            if (!ConfigureDelayedProcessing(depObj, commandSink))
                ProcessCommandSinkChanged(depObj, commandSink);
        }

        #endregion

        public ICommandSink CommandSink
        {
            get { return _commandSink; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("CommandSink", "Cannot set CommandSink to null.");

                if (_commandSink != null)
                    throw new InvalidOperationException("Cannot set CommandSink more than once.");

                _commandSink = value;

                CanExecute += (s, e) =>
                                  {
                                      bool handled;
                                      e.CanExecute = _commandSink.CanExecuteCommand(e.Command, e.Parameter,
                                                                                    out handled);
                                      e.Handled = handled;
                                  };

                Executed += (s, e) =>
                                {
                                    bool handled;
                                    _commandSink.ExecuteCommand(e.Command, e.Parameter, out handled);
                                    e.Handled = handled;
                                };
            }
        }

        // This method is necessary when the CommandSink attached property is set on an element 
        // in a template, or any other situation in which the element's CommandBindings have not 
        // yet had a chance to be created and added to its CommandBindings collection.
        private static bool ConfigureDelayedProcessing(DependencyObject depObj, ICommandSink commandSink)
        {
            bool isDelayed = false;

            var elem = new CommonElement(depObj);
            if (elem.IsValid && !elem.IsLoaded)
            {
                RoutedEventHandler handler = null;
                handler = delegate
                {
                    elem.Loaded -= handler;
                    ProcessCommandSinkChanged(depObj, commandSink);
                };
                elem.Loaded += handler;
                isDelayed = true;
            }

            return isDelayed;
        }

        private static void ProcessCommandSinkChanged(DependencyObject depObj, ICommandSink commandSink)
        {
            CommandBindingCollection cmdBindings = GetCommandBindings(depObj);
            if (cmdBindings == null)
                throw new ArgumentException(
                    "The CommandSinkBinding.CommandSink attached property was set on an element that does not support CommandBindings.");

            foreach (CommandBinding cmdBinding in cmdBindings)
            {
                var csb = cmdBinding as CommandSinkBinding;
                if (csb != null && csb.CommandSink == null)
                    csb.CommandSink = commandSink;
            }
        }

        private static CommandBindingCollection GetCommandBindings(DependencyObject depObj)
        {
            var elem = new CommonElement(depObj);
            return elem.IsValid ? elem.CommandBindings : null;
        }

        #region Nested type: CommonElement

        /// <summary>
        /// This class makes it easier to write code that works 
        /// with the common members of both the FrameworkElement
        /// and FrameworkContentElement classes.
        /// </summary>
        private class CommonElement
        {
            public readonly bool IsValid;
            private readonly FrameworkContentElement _frameworkContentElement;
            private readonly FrameworkElement _frameworkElement;

            public CommonElement(DependencyObject depObj)
            {
                _frameworkElement = depObj as FrameworkElement;
                _frameworkContentElement = depObj as FrameworkContentElement;

                IsValid = _frameworkElement != null || _frameworkContentElement != null;
            }

            public CommandBindingCollection CommandBindings
            {
                get
                {
                    Verify();

                    if (_frameworkElement != null)
                        return _frameworkElement.CommandBindings;
                    else
                        return _frameworkContentElement.CommandBindings;
                }
            }

            public bool IsLoaded
            {
                get
                {
                    Verify();

                    if (_frameworkElement != null)
                        return _frameworkElement.IsLoaded;
                    else
                        return _frameworkContentElement.IsLoaded;
                }
            }

            public event RoutedEventHandler Loaded
            {
                add
                {
                    Verify();

                    if (_frameworkElement != null)
                        _frameworkElement.Loaded += value;
                    else
                        _frameworkContentElement.Loaded += value;
                }
                remove
                {
                    Verify();

                    if (_frameworkElement != null)
                        _frameworkElement.Loaded -= value;
                    else
                        _frameworkContentElement.Loaded -= value;
                }
            }

            private void Verify()
            {
                if (!IsValid)
                    throw new InvalidOperationException("Cannot use an invalid CommmonElement.");
            }
        }

        #endregion
    }
}