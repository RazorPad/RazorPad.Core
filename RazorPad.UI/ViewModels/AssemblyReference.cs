﻿using System;
using System.IO;
using System.Reflection;
using System.Text;
using RazorPad.UI;

namespace RazorPad.ViewModels
{
    public class AssemblyReference : ViewModelBase, IEquatable<AssemblyReference>
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Location { get; set; }
        public string Version { get; set; }
        public string RuntimeVersion { get; set; }
        public string Culture { get; set; }
        public string PublicKeyToken { get; set; }
        public string ProcessorArchitecture { get; set; }

        public bool IsStandard;
        public bool IsNotReadOnly { get; set; }
        public bool IsRecent { get; set; }
        
        public bool IsInstalled
        {
            get { return _isInstalled; }
            set
            {
                if (_isInstalled == value)
                    return;

                _isInstalled = value;
                OnPropertyChanged("IsInstalled");
            }
        }
        private bool _isInstalled;



        public AssemblyReference(string name, string version, string culture, string publicKeyToken)
        {
            Name = name;
            Version = version;
            Culture = culture;
            PublicKeyToken = publicKeyToken;
            IsNotReadOnly = true;
        }


        public static bool TryLoadReference(string path, out AssemblyReference assemblyReference, out string message)
        {
            assemblyReference = null;
            message = null;

            if (!File.Exists(path))
            {
                message = "File doesn't exist. Make sure you pass a valid file path.";
                return false;
            }

            try
            {
                assemblyReference = new AssemblyReference(path);
            }
            catch (FileLoadException fileLoadException)
            {
                if (fileLoadException.Message.Contains("has already been loaded"))
                    message = string.Format("{0} has already been loaded from a different path", path);
                else
                    message = string.Format("Unexpected error happened while loading {0}", path);
            }
            catch (BadImageFormatException)
            {
                message = string.Format("Not a .NET Assembly or is created with a later version of .NET: {0}", path);
            }
            catch (Exception)
            {
                message = string.Format("Unexpected error happened while loading {0}", path);
            }

            return assemblyReference != null;
        }


        public AssemblyReference(string path)
        {
            var assemblyName = AssemblyName.GetAssemblyName(path);
            Name = assemblyName.Name;
            Version = assemblyName.Version.ToString();
            PublicKeyToken = GetHumanReadablePublicKeyToken(assemblyName.GetPublicKeyToken());
            Culture = assemblyName.CultureInfo.Name == "" ? "neutral" : assemblyName.CultureInfo.Name;
            ProcessorArchitecture = assemblyName.ProcessorArchitecture.ToString();
            FullName = assemblyName.FullName;
            Location = path;
            IsNotReadOnly = true;

        }

        public AssemblyReference(Assembly assembly)
            : this(assembly.Location)
        {
        }


        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(AssemblyReference other)
        {
            //Check whether the compared object is null.
            if (ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return Name.Equals(other.Name) && Version.Equals(other.Version);

        }

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null.
            int hashName = Name == null ? 0 : Name.GetHashCode();

            //Get hash code for the Code field.
            int hashVersion = Version == null ? 0 : Version.GetHashCode();

            //Calculate the hash code for the product.
            return hashName ^ hashVersion;
        }

        protected string GetHumanReadablePublicKeyToken(byte[] tokenBytes)
        {
            const byte mask = 15;
            const string hex = "0123456789abcdef";
            var pkt = new StringBuilder();
            foreach (var b in tokenBytes)
                pkt.Append(hex[b / 16 & mask])
                    .Append(hex[b & mask]);

            return pkt.ToString();
        }
    }
}
