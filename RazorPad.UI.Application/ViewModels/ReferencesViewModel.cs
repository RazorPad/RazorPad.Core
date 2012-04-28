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

        public SearchableReferencesViewModel StandardReferences { get; set; }
        public SearchableReferencesViewModel RecentReferences { get; set; }
        public SearchableReferencesViewModel InstalledReferences { get; set; }

        public ReferencesViewModel(IEnumerable<Reference> loadedReferences)
        {
            var standardReferences = LoadStandardReferences().ToList();
            var recentReferences = GetRecentReferences().ToList();
            var allReferences = loadedReferences.Union(standardReferences).Union(recentReferences).ToList();


            StandardReferences = new SearchableReferencesViewModel(standardReferences);
            StandardReferences.References.ListChanged += StandardReferences_ListChanged;

            RecentReferences = new SearchableReferencesViewModel(recentReferences);
            RecentReferences.References.ListChanged += RecentReferences_ListChanged;

            InstalledReferences = new SearchableReferencesViewModel(allReferences.Where(r => r.IsInstalled));
        }



        void StandardReferences_ListChanged(object sender, ListChangedEventArgs e)
        {
            // prevent stack overflow
            StandardReferences.References.ListChanged -= StandardReferences_ListChanged;
            RecentReferences.References.ListChanged -= RecentReferences_ListChanged;

            var reference = StandardReferences.References.ElementAt(e.NewIndex);
            if (reference.IsInstalled)
            {
                if (!InstalledReferences.References.Contains(reference))
                {
                    InstalledReferences.References.Add(reference);
                }
                if (!RecentReferences.References.Contains(reference))
                {
                    RecentReferences.References.Add(reference);
                }
            }
            else
            {
                var index = InstalledReferences.References.IndexOf(reference);
                if (index >= 0) InstalledReferences.References.RemoveAt(index);
                
            }

            StandardReferences.References.ListChanged += StandardReferences_ListChanged;
            RecentReferences.References.ListChanged += RecentReferences_ListChanged;
        }

        void RecentReferences_ListChanged(object sender, ListChangedEventArgs e)
        {
            // prevent stack overflow
            RecentReferences.References.ListChanged -= RecentReferences_ListChanged;

            var reference = RecentReferences.References.ElementAt(e.NewIndex);
            if (reference.IsInstalled)
            {
                if (!InstalledReferences.References.Contains(reference))
                {
                    InstalledReferences.References.Add(reference);
                }
            }
            else
            {
                var index = InstalledReferences.References.IndexOf(reference);
                if (index >= 0) InstalledReferences.References.RemoveAt(index);
            }

            RecentReferences.References.ListChanged += RecentReferences_ListChanged;
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
                reference.IsStandard = true;
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
