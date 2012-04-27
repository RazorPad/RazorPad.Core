using System;
using System.Linq;
using System.Xml.Linq;
using RazorPad.Framework;

namespace RazorPad.Extensions.Xml.ModelProvider
{
    public class XmlModelProvider : RazorPad.ModelProvider
    {
        public string Xml
        {
            get { return _xml; }
            set
            {
                if (_xml == value)
                    return;

                _xml = value;
                TriggerModelChanged();
            }
        }
        private string _xml;


        public XmlModelProvider(Type modelType = null, string model = null)
            : base(modelType)
        {
            Xml = model ?? "<Model>\r\n\r\n</Model>";
        }


        public override string Serialize()
        {
            return Xml;
        }

        public override void Deserialize(string serialized)
        {
            Xml = serialized;
        }

        protected override dynamic RebuildModel()
        {
            var xdoc = XDocument.Parse(Xml, LoadOptions.None);

            var values = xdoc.Root.Elements().ToDictionary(x => x.Name.LocalName, y => (object)y.Value);
            
            return new DynamicDictionary(values);
        }
    }
}