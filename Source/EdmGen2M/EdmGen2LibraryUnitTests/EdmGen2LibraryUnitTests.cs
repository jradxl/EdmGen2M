/**
 * Copyright (C)  All Rights Reserved
 * 
 * EdmGen2Library Unit tests. No files are written
 * 
 * Contributors:
 *	Jiri Cincura (jiri@cincura.net)
 *	John Radley (jradley@jsrsoft.co.uk), based on tests by Jiri, with thanks
 */

// Some CommandLines for convenience of testing 
// /FromEdmx "C:\DB\Dropbox\VS\VS12\EF\CodeGenerationToolsExperiments\EdmGen2\EdmGen2M\EdmGen2LibraryUnitTests\TestDataFiles\Northwind_v40.edmx"
// /ToEdmx "C:\DB\Dropbox\VS\VS12\EF\CodeGenerationToolsExperiments\EdmGen2\EdmGen2M\EdmGen2\bin\Debug\Northwind_v40.csdl" "C:\DB\Dropbox\VS\VS12\EF\CodeGenerationToolsExperiments\EdmGen2\EdmGen2M\EdmGen2\bin\Debug\Northwind_v40.ssdl" "C:\DB\Dropbox\VS\VS12\EF\CodeGenerationToolsExperiments\EdmGen2\EdmGen2M\EdmGen2\bin\Debug\Northwind_v40.msl" "C:\DB\Dropbox\VS\VS12\EF\CodeGenerationToolsExperiments\EdmGen2\EdmGen2M\EdmGen2\bin\Debug\Northwind_v40.des" 
// /CodeGen
// /ModelGen "Data Source=.\SQLExpress;Initial Catalog=Northwind;Integrated Security=True;MultipleActiveResultSets=True" "System.Data.SqlClient" "Northwind" "3.0"
// /Validate "C:\DB\Dropbox\VS\VS12\EF\CodeGenerationToolsExperiments\EdmGen2\EdmGen2M\EdmGen2LibraryUnitTests\TestDataFiles\Northwind_v40.edmx"
// /ViewGen cs C:\DB\Dropbox\VS\VS12\EF\CodeGenerationToolsExperiments\EdmGen2\EdmGen2M\EdmGen2LibraryUnitTests\TestDataFiles\Northwind_v40.edmx

namespace EdmGen2LibraryUnitTests
{

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;

    [TestClass]
    public class EdmGen2LibraryUnitTests
    {
        private EdmGen2TestUtilities _testUtilities = null;

        public EdmGen2LibraryUnitTests()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _testUtilities = new EdmGen2TestUtilities();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void FromEdmxToEdmxRuntimeNodeOnlyV1()
        {
            FromEdmxToEdmxRuntimeNodeOnly("Northwind_v35");
        }

        [TestMethod]
        public void FromEdmxToEdmxRuntimeNodeOnlyV2()
        {
            FromEdmxToEdmxRuntimeNodeOnly("Northwind_v40");
        }

        [TestMethod]
        public void FromEdmxToEdmxRuntimeNodeOnlyV3()
        {
            FromEdmxToEdmxRuntimeNodeOnly("Northwind_v40");
        }

        [TestMethod]
        public void FromEdmxToEdmxV1()
        {
            var name = "Northwind_v35";
            FromEdmxToEdmx(name);
        }

        [TestMethod]
        public void FromEdmxToEdmxV2()
        {
            FromEdmxToEdmx("Northwind_v40");
        }

        [TestMethod]
        public void FromEdmxToEdmxV3()
        {
            FromEdmxToEdmx("Northwind_v40");
        }

        private void FromEdmxToEdmxRuntimeNodeOnly(string edmxFileNameSansExtensions)
        {
            // first, split an existing edmx apart
            string edmxName = edmxFileNameSansExtensions + ".edmx";
            string csdlName = edmxFileNameSansExtensions + ".csdl";
            string ssdlName = edmxFileNameSansExtensions + ".ssdl";
            string mslName = edmxFileNameSansExtensions + ".msl";
            string desName = edmxFileNameSansExtensions + ".des";

            var di = _testUtilities.TestDataFilesDirectory;

            if (!di.Exists)
                throw new FileNotFoundException("TestDataFilesDirectory cannot be found");

            string originalEdmx = Path.Combine(di.FullName, edmxName);
            string args = "/FromEdmx " + originalEdmx;

            EdmGen2.EdmGen2.Main(args.Split(' '));

            // next, combine the pieces into a new edmx
            string[] args2 = { "/ToEdmx", csdlName, ssdlName, mslName, desName };
            EdmGen2.EdmGen2.Main(args2);

            _testUtilities.CompareRuntimeNode(originalEdmx, edmxName);
        }

        private void FromEdmxToEdmx(string edmxFileNameSansExtensions)
        {
            // first, split an existing edmx apart
            string edmxName = edmxFileNameSansExtensions + ".edmx";
            string csdlName = edmxFileNameSansExtensions + ".csdl";
            string ssdlName = edmxFileNameSansExtensions + ".ssdl";
            string mslName = edmxFileNameSansExtensions + ".msl";
            string desName = edmxFileNameSansExtensions + ".des";

            var di = _testUtilities.TestDataFilesDirectory;

            if (!di.Exists)
                throw new FileNotFoundException("TestDataFilesDirectory cannot be found");

            string originalEdmx = Path.Combine(di.FullName, edmxName);
            string args = "/FromEdmx " + originalEdmx;

            EdmGen2.EdmGen2.Main(args.Split(' '));

            // next, combine the pieces into a new edmx
            string[] args2 = { "/ToEdmx", csdlName, ssdlName, mslName, desName };
            EdmGen2.EdmGen2.Main(args2);

            if (!_testUtilities.CompareRuntimeNode(originalEdmx, edmxName))
            {
                //throw new Exception("Test failed!  edmx files didn't compare");
            }

            if (!_testUtilities.CompareXMLFiles(originalEdmx, edmxName))
            {
                throw new Exception("Test failed!  edmx files didn't compare");
            }
        }
    }
}
