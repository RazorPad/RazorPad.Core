using System.Text;

namespace RazorPad.Compilation
{
    public abstract class TemplateBase
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