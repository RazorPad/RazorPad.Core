using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.ViewModels;

namespace RazorPad.UI.AssemblyReference
{
    [TestClass]
    public class StandardDotNetReferencesLocatorTests
    {
        [TestMethod]
        public void ShouldLocateStandardDotNetReferences()
        {
            var paths = StandardDotNetReferencesLocator.GetStandardDotNetReferencePaths() ?? Enumerable.Empty<string>();
            Assert.IsTrue(paths.Any());
        }

        [TestMethod]
        public void ShouldLoadStandardDotNetReferences()
        {
            var paths = StandardDotNetReferencesLocator.GetStandardDotNetReferencePaths() ?? Enumerable.Empty<string>();
            var references = new List<Reference>();
            var messages = new List<string>();

            foreach (var path in paths)
            {
                string message;
                Reference reference;
                if(Reference.TryLoadReference(path, out reference, out message))
                    references.Add(reference);
                else
                {
                    messages.Add(message);
                }
            }

            Assert.IsTrue(references.Any());
        }
    }
}
