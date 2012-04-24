using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazorPad.ViewModels
{
	public class ReferencesViewModel
	{
		public SearchableReferencesViewModel FrameworkReferences { get; set; }
		public SearchableReferencesViewModel RecentReferences { get; set; }

		public IEnumerable<Reference> SelectedReferences
		{
			get
			{
				return
					FrameworkReferences.References.Where(r => r.Selected)
						.Union(RecentReferences.References.Where(r => r.Selected));
			}
		}

		public ReferencesViewModel()
		{
			FrameworkReferences = new SearchableReferencesViewModel(GetMockFrameworkReferences());
			RecentReferences = new SearchableReferencesViewModel(GetMockRecentReferences());
		}

		private static IEnumerable<Reference> GetMockRecentReferences()
		{
			return new[] {
				new Reference("System", "4.0.0.0", null, null),
				new Reference("System.Web", "4.0.0.0", null, null),
				new Reference("System.Data", "2.0.0.0", null, null),
				new Reference("Microsoft.Design", "3.5.0.0", null, null)
			};
		}

		private static IEnumerable<Reference> GetMockFrameworkReferences()
		{
			return new[] {
				new Reference("System.Data", "2.0.0.0", null, null),
				new Reference("Microsoft.Design", "3.5.0.0", null, null)
			};
		}
	}
}
