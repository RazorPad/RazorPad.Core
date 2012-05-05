using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace RazorPad.UI.Theming
{
    [Export(typeof(ThemeLoader))]
    public class ThemeLoader
    {
        public List<Theme> LoadThemes(string selectedTheme = null)
        {
            var themeDirectory = Path.Combine(Environment.CurrentDirectory, "themes");

            if (string.IsNullOrWhiteSpace(themeDirectory))
                return new List<Theme>();

            var themes =
                from file in Directory.GetFiles(themeDirectory, "*.xaml")
                let name = Path.GetFileNameWithoutExtension(file)
                let selected = string.Equals(name, selectedTheme, StringComparison.OrdinalIgnoreCase)
                select new Theme
                           {
                               FilePath = file,
                               Name = name,
                               Selected = selected,
                           };

            return themes.ToList();
        }
    }
}