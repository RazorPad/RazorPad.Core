using System.IO;
using System.Text;
using RazorPad.Framework;

namespace RazorPad.Compilation
{
    public abstract class TemplateBase : DynamicDictionary
    {
        public StringBuilder Buffer { get; set; }

        public dynamic Model { get; set; }

        protected TemplateBase()
        {
            Buffer = new StringBuilder();
        }

        public abstract void Execute();

        public virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        public virtual void WriteLiteral(object value)
        {
            Buffer.Append(value);
        }

        public static void WriteTo(TextWriter writer, object content)
        {
            writer.Write(content);
        }

        public static void WriteLiteralTo(TextWriter writer, object content)
        {
            writer.Write(content);
        }
    }

    public abstract class TemplateBase<TModel> : TemplateBase
    {
        public new TModel Model
        {
            get { return (TModel) base.Model; }
            set { base.Model = value; }
        }
    }
}