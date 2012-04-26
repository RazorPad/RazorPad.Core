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
		public SearchableReferencesViewModel InstalledReferences { get; set; }

		public IEnumerable<Reference> SelectedReferences
		{
			get
			{
				return
					FrameworkReferences.References.Where(r => r.Selected)
						.Union(RecentReferences.References.Where(r => r.Selected));
			}
		}

		public ReferencesViewModel(IEnumerable<Reference> loadedReferences)
		{

			var standardReferences = LoadStandardReferences().ToList();
			var recentReferences = GetMockRecentReferences().ToList();
			var installedReferences = loadedReferences.ToList();
			
			foreach (var source in standardReferences.Where(installedReferences.Contains))
			{
				source.Selected = true;
			}

			foreach (var installedReference in installedReferences)
			{
				installedReference.Selected = true;
			}

			FrameworkReferences = new SearchableReferencesViewModel(standardReferences);
			RecentReferences = new SearchableReferencesViewModel(recentReferences);
			InstalledReferences = new SearchableReferencesViewModel(installedReferences);
		}



		private static IEnumerable<Reference> LoadStandardReferences()
		{
			var paths = StandardDotNetReferencesLocator.GetStandardDotNetReferencePaths() ?? Enumerable.Empty<string>();

			foreach (var path in paths)
			{
				Reference reference;
				string message;
				if (Reference.TryLoadReference(path, out reference, out message)) yield return reference;
			}
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
	}
}
