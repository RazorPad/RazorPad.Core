using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RazorPad.ViewModels
{
	public class Reference : ISearchable
	{
		private Assembly a;

		public string Name { get; set; }
		public string FullName { get; set; }
		public string Location { get; set; }
		public string Version { get; set; }
		public string Culture { get; set; }
		public string PublicKeyToken { get; set; }
		public string ProcessorArchitecture { get; set; }
		public bool Selected { get; set; }

		public Reference(string name, string version, string culture, string publicKeyToken)
		{
			Name = name;
			Version = version;
			Culture = culture;
			PublicKeyToken = publicKeyToken;
		}

		public Reference(string path)
		{
			if (!File.Exists(path))
				throw new ArgumentException("File doesn't exist. Make sure you pass a valid file path.");

			Assembly assembly;
			try
			{
				assembly = Assembly.ReflectionOnlyLoadFrom(path);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Assembly specified is a not a .Net assembly.", ex);
			}

			Initialize(assembly);

		}

		private void Initialize(Assembly assembly)
		{
			if (assembly.IsDynamic) return;

			FullName = assembly.FullName;
			Location = assembly.Location;

			var an = new AssemblyName(FullName);
			Name = an.Name;
			Version = an.Version.ToString();
			PublicKeyToken = Encoding.UTF8.GetString(an.GetPublicKeyToken());
			Culture = an.CultureInfo.IsNeutralCulture ? "neutral" : an.CultureInfo.Name;
			ProcessorArchitecture = an.ProcessorArchitecture.ToString();
		}

		public Reference(Assembly assembly)
		{
			Initialize(assembly);
		}

		
	}
}
