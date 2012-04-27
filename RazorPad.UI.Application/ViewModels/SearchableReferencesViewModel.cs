using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using RazorPad.UI;

namespace RazorPad.ViewModels
{
	public class SearchableReferencesViewModel : ViewModelBase
	{
		public SearchableReferencesViewModel(IEnumerable<Reference> references)
		{
			References = new BindingList<Reference>(references.Distinct().OrderBy(r => r.Name).ThenBy(r => r.Version).ToList());
		}

		public BindingList<Reference> References { get; set; }

		
	}
}
