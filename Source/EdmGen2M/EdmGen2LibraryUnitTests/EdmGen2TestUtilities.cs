using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EdmGen2LibraryUnitTests
{
    public class EdmGen2TestUtilities
    {

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        public DirectoryInfo TestDataFilesDirectory
        {
            //Back from bin\Debug
            get { return new DirectoryInfo(@"..\..\..\EdmGen2LibraryUnitTests\TestDataFiles"); }
        }

        public DirectoryInfo EdmGen2ExeDirectory
        {
            //Back from bin\Debug
            get { return new DirectoryInfo(@"..\..\..\EdmGen2\bin\Debug\EdmGen2.exe"); }
        }

        public string ProviderName
        {
            get { return "System.Data.SqlClient"; }
        }

        public string ConnectionString
        {
            get { return @"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True;MultipleActiveResultSets=True"; }
        }

        public bool CompareRuntimeNode(string f1, string f2)
        {
            // load the files into xdocuments to strip out all insignificant whitespace
            XDocument x1 = XDocument.Load(f1, LoadOptions.None);
            XDocument x2 = XDocument.Load(f2, LoadOptions.None);

            //Compare only the Runtime nodes, to avoid issues with the Designer section
            var runtimeNode1 = x1.Root.Descendants().Where(z => z.Name.LocalName.Equals("Runtime")).FirstOrDefault();
            var runtimeNode2 = x2.Root.Descendants().Where(z => z.Name.LocalName.Equals("Runtime")).FirstOrDefault();
            var s1 = runtimeNode1.ToString();
            var s2 = runtimeNode2.ToString();

            Assert.AreEqual<String>(s1,s2);

            return true;
        }

        public bool CompareXMLFiles(string f1, string f2)
        {
            // load the files into xdocuments to strip out all insignificant whitespace
            XDocument x1 = XDocument.Load(f1, LoadOptions.None);
            XDocument x2 = XDocument.Load(f2, LoadOptions.None);

            StringWriter sw1 = new StringWriter();
            StringWriter sw2 = new StringWriter();

            x1.Root.Save(sw1);
            x2.Root.Save(sw2);

            File.WriteAllText("sw1.txt", sw1.ToString());
            File.WriteAllText("sw2.txt", sw2.ToString());

            return sw1.ToString().Equals(sw2.ToString());
        }

        public bool CompareFiles(string f1, string f2)
        {
            String s1 = File.ReadAllText(f1);
            String s2 = File.ReadAllText(f2);
            return s1.Equals(s2);
        }
    }
}

