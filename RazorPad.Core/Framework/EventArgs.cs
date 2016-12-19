using System;

namespace RazorPad.Framework
{
    public class EventArgs<T> : EventArgs
    {
        public new static EventArgs<T> Empty
        {
            get { return new EventArgs<T>(default(T)); }
        }

        public T Message { get; private set; }

        public EventArgs(T message)
        {
            Message = message;
        }
    }
}