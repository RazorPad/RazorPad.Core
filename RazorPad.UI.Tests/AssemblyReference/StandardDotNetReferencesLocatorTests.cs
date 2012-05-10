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
            var references = new List<ViewModels.AssemblyReference>();
            var messages = new List<string>();

            foreach (var path in paths)
            {
                string message;
                ViewModels.AssemblyReference assemblyReference;
                if(ViewModels.AssemblyReference.TryLoadReference(path, out assemblyReference, out message))
                    references.Add(assemblyReference);
                else
                {
                    messages.Add(message);
                }
            }

            Assert.IsTrue(references.Any());
        }
    }
}
