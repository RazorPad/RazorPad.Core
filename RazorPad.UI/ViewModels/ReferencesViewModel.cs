using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using NLog;

namespace RazorPad.ViewModels
{
    public class ReferencesViewModel
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public SearchableReferencesViewModel StandardReferences { get; set; }
        public SearchableReferencesViewModel RecentReferences { get; set; }
        public SearchableReferencesViewModel InstalledReferences { get; set; }

        public ReferencesViewModel(IEnumerable<AssemblyReference> loadedReferences)
        {
            Log.Info(() => string.Format("Loaded references: {0}", string.Join(", ", loadedReferences)));

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

            var reference = sender as AssemblyReference;
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

            var reference = sender as AssemblyReference;
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



        private static IEnumerable<AssemblyReference> LoadStandardReferences()
        {
            var paths = (StandardDotNetReferencesLocator.GetStandardDotNetReferencePaths() ?? Enumerable.Empty<string>()).ToArray();

            Log.Debug("Standard .NET References: " + string.Join(", ", paths));

            foreach (var path in paths)
            {
                AssemblyReference assemblyReference;
                string message;

                Log.Debug("Loading standard reference {0}... ", path);

                var isLoadable = AssemblyReference.TryLoadReference(path, out assemblyReference, out message);

                if (!isLoadable)
                {
                    Log.Warn("Reference {0} NOT loaded.", path);
                    continue;
                }

                Log.Info("Standard reference {0} loaded.", path);

                assemblyReference.IsStandard = true;

                yield return assemblyReference;
            }
        }

        private static IEnumerable<AssemblyReference> GetRecentReferences()
        {
            const string recentReferencesFilePath = "RecentReferences.txt";

            Log.Info("Getting recent assembly references from " + recentReferencesFilePath);

            if (File.Exists(recentReferencesFilePath))
            {
                try
                {
                    return File
                            .ReadAllLines(recentReferencesFilePath)
                            .Where(File.Exists)
                            .Select(r => new AssemblyReference(r)
                                            {
                                                IsRecent = true
                                            });
                }
                catch (Exception ex)
                {
                    Log.ErrorException("Error getting recent references: {0}", ex);
                }
            }

            return Enumerable.Empty<AssemblyReference>();
        }
    }
}
