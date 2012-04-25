using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            References = new ObservableCollection<Reference>(references.Distinct().OrderBy(r => r.Name).ThenBy(r => r.Version));
		}

		public ObservableCollection<Reference> References { get; private set; }
	}
}
