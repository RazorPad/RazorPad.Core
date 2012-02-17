using System;

namespace RazorPad.Framework
{
    public class EventArgs<T> : EventArgs
    {
        public T Message { get; private set; }

        public EventArgs(T message)
        {
            Message = message;
        }
    }
}