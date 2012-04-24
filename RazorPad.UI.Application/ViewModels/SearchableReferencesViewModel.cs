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
		private readonly ObservableCollection<Reference> _recentReferences;
		private Reference _selectedReference;

		public SearchableReferencesViewModel()
		{
			_recentReferences = GetRecentReferences();
		}

		private ObservableCollection<Reference> GetRecentReferences()
		{
			return new ObservableCollection<Reference> {
				new Reference("System") { Version = "4.0.0.0"},
				new Reference("System.Web") { Version = "4.0.0.0"},
				new Reference("Microsoft.Design") { Version = "3.5.0.0"}
			};
		}

		public ObservableCollection<Reference> RecentReferences
		{
			get { return _recentReferences; }
		}

		public Reference SelectedReference
		{
			get { return _selectedReference ?? _recentReferences.First(); }
			set { _selectedReference = value; }
		}
	}
}
