using System;
using System.Text.RegularExpressions;

namespace RazorPad
{
    public class SimpleName
    {
        private readonly string _extendedName;
        
        public virtual string Suffix { get; set; }

        public SimpleName(object objectOrType, string suffix = null)
        {
            if (objectOrType == null)
                throw new ArgumentNullException("objectOrType", "Could not determine Simple Name from null value");
            
            Type type;
            
            if (objectOrType is Type)
                type = (Type)objectOrType;
            else
                type = objectOrType.GetType();

            _extendedName = type.Name;
            Suffix = suffix;
        }

        public SimpleName(string extendedName, string suffix = null)
        {
            if (extendedName == null)
                throw new ArgumentNullException("extendedName", "Could not determine Simple Name from null value");

            _extendedName = extendedName;
            Suffix = suffix;
        }

        public override string ToString()
        {
            var name = _extendedName;

            if(!string.IsNullOrWhiteSpace(Suffix))
                name = _extendedName.Replace(Suffix ?? string.Empty, string.Empty);

            var match = Regex.Match(name, @"(.*\.)?(?<SimpleName>.*)");

            return match.Groups["SimpleName"].Value;
        }

        public static implicit operator string(SimpleName name)
        {
            if (name == null)
                return null;

            return name.ToString();
        }
    }
}