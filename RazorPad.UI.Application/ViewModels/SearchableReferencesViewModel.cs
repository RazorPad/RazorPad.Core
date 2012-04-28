using System.Collections.Generic;
using System.Linq;
using RazorPad.UI;
using RazorPad.UI.Wpf;

namespace RazorPad.ViewModels
{
	public class SearchableReferencesViewModel : ViewModelBase
	{
		public SearchableReferencesViewModel(IEnumerable<Reference> references)
		{
            References = new TheRealObservableCollection<Reference>(references.Distinct().OrderBy(r => r.Name).ThenBy(r => r.Version).ToList());
		}

        public TheRealObservableCollection<Reference> References { get; set; }

		
	}
}
