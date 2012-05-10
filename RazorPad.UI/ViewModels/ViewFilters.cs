using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazorPad.ViewModels
{
	public class ReferenceViewFilters
	{
		public bool IsNotReadOnly { get; set; }
		public bool IsInstalled { get; set; }
		public bool IsRecent { get; set; }
	}
}
