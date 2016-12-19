using System;
using RazorPad.Framework;

namespace RazorPad
{
    public static class EventHandlerExtensions
    {
        public static void SafeInvoke(this EventHandler handler, EventArgs args = null, object sender = null)
        {
            if (handler != null)
                handler(sender, args ?? EventArgs.Empty);
        }

        public static void SafeInvoke<TData>(this EventHandler<EventArgs<TData>> handler, TData data, object sender = null)
        {
            if (handler != null)
                handler(sender, new EventArgs<TData>(data));
        }

        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> handler, TEventArgs args = null, object sender = null)
            where TEventArgs : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }
    }
}
