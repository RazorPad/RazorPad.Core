using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RazorPad.ViewModels
{
    public class ReferencesViewModel
    {
        public SearchableReferencesViewModel StandardReferences { get; set; }
        public SearchableReferencesViewModel RecentReferences { get; set; }
        public SearchableReferencesViewModel InstalledReferences { get; set; }

        public ReferencesViewModel(IEnumerable<Reference> loadedReferences)
        {
            Trace.TraceInformation("Loaded references: " + string.Join(", ", loadedReferences));

            var standardReferences = LoadStandardReferences().ToList();
            var recentReferences = GetRecentReferences().ToList();
            var allReferences = loadedReferences.Union(standardReferences).Union(recentReferences).ToList();


            StandardReferences = new SearchableReferencesViewModel(standardReferences);
            StandardReferences.References.ItemPropertyChanged += StandardReferences_ListChanged;

            RecentReferences = new SearchableReferencesViewModel(recentReferences);
            RecentReferences.References.ItemPropertyChanged += RecentReferences_ListChanged;

            InstalledReferences = new SearchableReferencesViewModel(allReferences.Where(r => r.IsInstalled));
        }



        void StandardReferences_ListChanged(object sender, PropertyChangedEventArgs e)
        {
            // prevent stack overflow
            StandardReferences.References.ItemPropertyChanged -= StandardReferences_ListChanged;
            RecentReferences.References.ItemPropertyChanged -= RecentReferences_ListChanged;

            var reference = sender as Reference;
            if (reference == null) return;

            if (reference.IsInstalled)
            {
                if (!InstalledReferences.References.Contains(reference))
                {
                    InstalledReferences.References.Add(reference);
                }

                // find the handle by equality operator
                var recentReferenceIndex = RecentReferences.References.IndexOf(reference);

                // if not found, add it
                if (recentReferenceIndex == -1)
                {
                    RecentReferences.References.Add(reference);
                }
                // reassign the reference for auto syncing
                else
                {
                    RecentReferences.References.RemoveAt(recentReferenceIndex);
                    RecentReferences.References.Add(reference);
                }

            }
            else
            {
                var index = InstalledReferences.References.IndexOf(reference);
                if (index >= 0) InstalledReferences.References.RemoveAt(index);

            }

            StandardReferences.References.ItemPropertyChanged += StandardReferences_ListChanged;
            RecentReferences.References.ItemPropertyChanged += RecentReferences_ListChanged;
        }

        void RecentReferences_ListChanged(object sender, PropertyChangedEventArgs e)
        {
            // prevent stack overflow
            RecentReferences.References.ItemPropertyChanged -= RecentReferences_ListChanged;

            var reference = sender as Reference;
            if (reference == null) return;

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

            RecentReferences.References.ItemPropertyChanged += RecentReferences_ListChanged;
        }



        private static IEnumerable<Reference> LoadStandardReferences()
        {
            var paths = (StandardDotNetReferencesLocator.GetStandardDotNetReferencePaths() ?? Enumerable.Empty<string>()).ToArray();

            Trace.TraceInformation("Standard .NET References: " + string.Join(", ", paths));

            foreach (var path in paths)
            {
                Reference reference;
                string message;
                
                Trace.Write(string.Format("Loading standard reference {0}... ", path));

                var isLoadable = Reference.TryLoadReference(path, out reference, out message);

                if (!isLoadable)
                {
                    Trace.WriteLine("NOT loaded.");
                    continue;
                }

                Trace.WriteLine("loaded.");

                reference.IsStandard = true;

                yield return reference;
            }
        }

        private static IEnumerable<Reference> GetRecentReferences()
        {
            const string recentReferencesFilePath = "RecentReferences.txt";

            Trace.TraceInformation("Getting recent assembly references from " + recentReferencesFilePath);

            if (File.Exists(recentReferencesFilePath))
            {
                try
                {
                    return File
                            .ReadAllLines(recentReferencesFilePath)
                            .Where(File.Exists)
                            .Select(r => new Reference(r)
                                            {
                                                IsRecent = true
                                            });
                }

                catch (Exception ex)
                {
                    Trace.TraceError("Error getting recent references: {0}", ex);
                }
            }

            return Enumerable.Empty<Reference>();
        }
    }
}
