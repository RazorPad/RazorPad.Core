using System.IO;
using NLog;
using NLog.Targets;

namespace RazorPad.UI.Util
{
    [Target("TextWriter")]
    public sealed class TextWriterTarget : TargetWithLayout
    {
        public TextWriter Writer
        {
            get { return _writer; }
        }
        private readonly TextWriter _writer;


        public TextWriterTarget(TextWriter writer)
        {
            _writer = writer;
        }


        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = Layout.Render(logEvent);
            _writer.WriteLine(logMessage);
        }
    } 
}
