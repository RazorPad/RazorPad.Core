using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazorPad.ViewModels
{
	public class Reference
	{
		public string Name { get; set; }
		public string Location { get; set; }
		public string Version { get; set; }
		public string Culture { get; set; }
		public string PublicKeyToken { get; set; }
		public string ProcessorArchitecture { get; set; }

		public Reference(string name)
		{
			Name = name;
		}

		public Reference()
			: this("<name not set>") { }


	}
}
