using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace RazorPad.ViewModels
{
    public static class StandardDotNetReferencesLocator
    {
        private static readonly IEnumerable<string> KnownRegistryLocations = new []
                                                          {
                                                              @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.5.50131\AssemblyFoldersEx\Visual Studio v11.0 Reference Assemblies",
                                                              @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Visual Studio v11.0 Reference Assemblies",
                                                              @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Public Assemblies v11.0",
                                                              @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\ASP.NET MVC 4",
                                                              @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\ASP.NET MVC 3",
                                                              @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\AssemblyFolders\v3.5",
                                                              @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\AssemblyFolders\v3.0"

                                                          };

        private static readonly IEnumerable<string> KnownFolderLocations = new[]
                                                                              {
                                                                                  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5",
                                                                                  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0",
                                                                                  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5",
                                                                                  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5",
                                                                                  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5",
                                                                                  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0",
                                                                                  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5",
                                                                                  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5",

                                                                              };

        private static IEnumerable<string> _discoveredLocations = new List<string>();

        private static IEnumerable<string> _discoveredAssemblyPaths;

        private static void DiscoverKnownLocations()
        {
            _discoveredLocations = KnownRegistryLocations.SelectMany(GetAssemblyFolders).Union(KnownFolderLocations.Where(Directory.Exists));
        }

        private static IEnumerable<string> GetAssemblyFolders(string knownLocation)
        {
            var locations = Enumerable.Empty<string>();

            var key = Registry.LocalMachine.OpenSubKey(knownLocation);

            if(key != null && key.SubKeyCount > 0)
            {
                locations =
                    key.GetSubKeyNames().Select(key.OpenSubKey).Where(subKey => subKey != null)
                        .Select(subKey => subKey.GetValue("")).Where(
                            path => path != null && !string.IsNullOrEmpty(path.ToString())).Select(
                                path => path.ToString());
            }

            return locations;
        }

        public static IEnumerable<string> GetStandardDotNetReferencePaths()
        {
            if (_discoveredAssemblyPaths != null && _discoveredAssemblyPaths.Any())
                return _discoveredAssemblyPaths;

            if (!_discoveredLocations.Any())
                DiscoverKnownLocations();

            _discoveredAssemblyPaths = _discoveredLocations.SelectMany(l => Directory.GetFiles(l, "*.dll"));

            return _discoveredAssemblyPaths;
        }
    }
}
