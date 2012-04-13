using System;
using System.Globalization;
using System.IO;

namespace RazorPad.Compilation
{
    public class HelperResult
    {
        private readonly Action<TextWriter> _action;

        public HelperResult(Action<TextWriter> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            
            _action = action;
        }

        public override string ToString()
        {
            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                _action(stringWriter);
                return stringWriter.ToString();
            }
        }
    }
}