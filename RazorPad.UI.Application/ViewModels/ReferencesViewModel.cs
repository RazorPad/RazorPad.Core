using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace RazorPad.ViewModels
{
	public class ReferencesViewModel
	{
		//public SearchableReferencesViewModel AssemblyReferences { get; set; }

		public SearchableReferencesViewModel StandardReferences { get; set; }
		public SearchableReferencesViewModel RecentReferences { get; set; }
		public SearchableReferencesViewModel InstalledReferences { get; set; }

		public ReferencesViewModel(IEnumerable<Reference> loadedReferences)
		{
			var standardReferences = LoadStandardReferences().ToList();
			var recentReferences = GetRecentReferences().ToList();
			//var allReferences = loadedReferences.Union(standardReferences).Union(recentReferences).ToList();


			//AssemblyReferences = new SearchableReferencesViewModel(allReferences);
			
			StandardReferences = new SearchableReferencesViewModel(standardReferences);
			StandardReferences.References.ListChanged += StandardReferences_ListChanged;
	
			RecentReferences = new SearchableReferencesViewModel(recentReferences);
			RecentReferences.References.ListChanged += RecentReferences_ListChanged;

			InstalledReferences = new SearchableReferencesViewModel(loadedReferences);
			InstalledReferences.References.ListChanged += InstalledReferences_ListChanged;
		}

	

		void StandardReferences_ListChanged(object sender, ListChangedEventArgs e)
		{
			var items = e.ListChangedType;

			var reference = StandardReferences.References.ElementAt(e.NewIndex);
			InstalledReferences.References.Add(reference);

			// update assemblye refs
		}

		void RecentReferences_ListChanged(object sender, ListChangedEventArgs e)
		{
			var items = e.ListChangedType;

			// update assemblye refs
		}

		void InstalledReferences_ListChanged(object sender, ListChangedEventArgs e)
		{
			var items = e.ListChangedType;

			// update assemblye refs
		}


		private static IEnumerable<Reference> LoadStandardReferences()
		{
			var paths = StandardDotNetReferencesLocator.GetStandardDotNetReferencePaths() ?? Enumerable.Empty<string>();

			foreach (var path in paths)
			{
				Reference reference;
				string message;
				var assmeblyLoadable = Reference.TryLoadReference(path, out reference, out message);
				if (!assmeblyLoadable) continue;
				//reference.Filters.IsStandard = true;
				yield return reference;
			}
		}

		private static IEnumerable<Reference> GetRecentReferences()
		{
			const string recentReferencesFilePath = "RecentReferences.txt";
			if (File.Exists(recentReferencesFilePath))
			{
				try
				{
					return File
							.ReadAllLines(recentReferencesFilePath)
							.Select(r =>
								new Reference(r)
								{
									//Filters =
									//{
										IsRecent = true
									//}
								});
				}
				catch (Exception ex)
				{
					// TODO: Log the exception maybe?
				}
			}

			return Enumerable.Empty<Reference>();
		}
	}
}
