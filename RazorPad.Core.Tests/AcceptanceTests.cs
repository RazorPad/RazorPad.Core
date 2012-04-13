using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Compilation;

namespace RazorPad.Core.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        private TemplateCompiler _templateCompiler;

        [TestInitialize]
        public void TestInitialize()
        {
            _templateCompiler = new TemplateCompiler();
        }

        [TestMethod]
        public void ShouldSupportFunctions()
        {
            const string template = @"@functions {  int MyValue = 1; }  @MyValue";

            var results = _templateCompiler.Execute(template);

            Assert.AreEqual(results.Trim(), "1");
        }
       
       
        [TestMethod]
        public void ShouldSupportHelpers()
        {
            const string template = @"
@helper DisplayCurrency(decimal value)  {  
    @value.ToString(""C2"")
}

@DisplayCurrency(3.4m)
";
            var code = _templateCompiler.GenerateCode(template);
            var results = _templateCompiler.Execute(template);

            Assert.AreEqual(results.Trim(), 3.4m.ToString("C2"));
        }


    }
}
