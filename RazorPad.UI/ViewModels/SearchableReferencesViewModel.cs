using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using RazorPad.UI;

namespace RazorPad.ViewModels
{
	public class SearchableReferencesViewModel : ViewModelBase
	{
	    public TheRealObservableCollection<AssemblyReference> References { get; private set; }

	    public string FilterText
	    {
	        get { return _filterText; }
	        set
	        {
                if (_filterText == value)
                    return;

	            _filterText = value;
	            UpdateFilter();
                OnPropertyChanged("FilterText");
            }
	    }
        private string _filterText;

	    private void UpdateFilter()
	    {
            var view = CollectionViewSource.GetDefaultView(References);
         
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                view.Filter = null;
                return;
            }

            var regex = new Regex(FilterText, RegexOptions.IgnoreCase);
            view.Filter = item => regex.IsMatch(((AssemblyReference)item).Name);
        }

	    public string Summary
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(FilterText))
                    return "No filter applied. To search for a reference by name, enter few letters in the textbox.";
	            
                return string.Format("{0} item{1} containing \"{2}\".",
	                                 References.Count,
	                                 References.Count == 1 ? "" : "s",
	                                 FilterText);
	        }
	    }

	    public SearchableReferencesViewModel(IEnumerable<AssemblyReference> references)
	    {
	        var orderedReferences = references.Distinct().OrderBy(r => r.Name).ThenBy(r => r.Version);
	        References = new TheRealObservableCollection<AssemblyReference>(orderedReferences.ToList());
	    }
	}
}
