using System;

namespace RazorPad
{
    public class RazorPadError
    {
        public int Line { get; set; }

        public int Column { get; set; }

        public string Message { get; set; }

        public RazorPadError(string message = null)
        {
            Message = message;
        }

        public RazorPadError(Exception exception)
        {
            if (exception == null)
                return;

            Message = string.Format("EXCEPTION: {0}", exception.Message);
        }
    }
}