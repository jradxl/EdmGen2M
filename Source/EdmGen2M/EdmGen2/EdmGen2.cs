/**
 * Copyright (C) 2008, Microsoft Corp.  All Rights Reserved
 * 
 * Contributors:
 *	Jiri Cincura (jiri@cincura.net)
 *	John Radley  (jradley@jsrsoft.co.uk) : Split into two Class files where EdmGen2Library contains all Entity related work.
 */

namespace EdmGen2
{
    using ConceptualEdmGen; //Third-party DLL, only used for RetrofitModel.
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Design;
    using System.IO;

    /// <summary>
    /// 
    /// This is a command-line program to perform some common Entity Data 
    /// Model tooling functions on EDMX files.  It is similiar in functionality 
    /// to the .net framework's EdmGen.exe, but it will operate on the ".edmx" 
    /// file format, instead of the .csdl, .ssdl & .msl file formats used by the 
    /// .net framework's EDM.
    /// 
    /// The Designer Node is the fourth idtem in the EDMX file. When working in the
    /// VS Designer it's information must be preserved in a emdx -> {csdl, ssdl, msl, des} -> edmx
    /// cycle. Hence the options /FromEdmx and /ToEdmx support reading and writing the node unchanged.
    /// If there is no Designer node, then a default one is written.
    /// In the Unit tests, there are tests which only operate on the Runtime node, thus
    /// avoiding this issue.
    /// 
    /// </summary>
    public class EdmGen2
    {
        internal enum Mode { FromEdmx, ToEdmx, ModelGen, CodeGen, ViewGen, Validate, RetrofitModel, Help }

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowUsage();
                return;
            }

            Mode mode = GetMode(args[0]);

            switch (mode)
            {
                case Mode.FromEdmx:
                    FromEdmx(args);
                    break;
                case Mode.ToEdmx:
                    ToEdmx(args);
                    break;
                case Mode.ModelGen:
                    ModelGen(args);
                    break;
                case Mode.CodeGen:
                    CodeGen(args);
                    break;
                case Mode.ViewGen:
                    ViewGen(args);
                    break;
                case Mode.Validate:
                    Validate(args);
                    break;
                case Mode.RetrofitModel: //
                    RetrofitModel(args);
                    break;
                default:
                    ShowUsage();
                    return;
            }
        }

        private static Mode GetMode(string arg)
        {
            if ("/FromEdmx".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                return Mode.FromEdmx;
            }
            else if ("/ToEdmx".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                return Mode.ToEdmx;
            }
            else if ("/ModelGen".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                return Mode.ModelGen;
            }
            else if ("/ViewGen".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                return Mode.ViewGen;
            }
            else if ("/CodeGen".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                return Mode.CodeGen;
            }
            else if ("/Validate".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                return Mode.Validate;
            }
            else if ("/RetrofitModel".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                return Mode.RetrofitModel;
            }
            else
            {
                return Mode.Help;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:  EdmGen2 [arguments]");
            Console.WriteLine("            /FromEdmx <edmx file>");
            Console.WriteLine("            /ToEdmx <csdl file> <ssdl file> <msl file> [<des file>]");
            Console.WriteLine("            /ModelGen <connection string> <provider name> <model name>");
            Console.WriteLine("            /RetrofitModel <connection string> <provider name> <model name> <percent threshold>?");
            Console.WriteLine("            /ViewGen cs|vb <edmx file>");
            Console.WriteLine("            /CodeGen cs|vb <edmx file>");
            Console.WriteLine("            /Validate <edmx file>");
            Console.WriteLine("RetrofitModel option takes table names in the form [schema_name].[table_name] from the file tables.txt, one per line, if it exists.");
            //Console.WriteLine("\nPress any key to quit.");
            //Console.ReadKey();
        }

        private static void PrintModelGenUsage()
        {
            System.Console.WriteLine("Usage:  ModelGenerator <connection string> <provider name> <model name> [<version>] [includeFKs]");
            System.Console.WriteLine("             where <version> is 1.0 (for EF v1) or 2.0 (for EF v2) or 3.0 (for EF v3)");
            System.Console.WriteLine("             and where includeFKs is only valid on for EF versions later than EF v1");
            //Console.WriteLine("\nPress any key to quit.");
            //Console.ReadKey();
        }

        #region the functions that actually do the interesting things

        private static void FromEdmx(string[] args)
        {
            if (args.Length != 2)
            {
                ShowUsage();
                return;
            }

            FileInfo edmxFile;
            if (ParseEdmxFileArguments(args[1], out edmxFile))
            {
                String csdl = String.Empty;
                String ssdl = String.Empty;
                String msl = String.Empty;
                String designerOut = String.Empty;

                EdmGen2Library.EdmGen2Library.FromEdmx(edmxFile.FullName, out  csdl, out  ssdl, out  msl, out designerOut);

                // select the csdl element, and write it out

                string csdlFileName = EdmGen2Library.EdmGen2Library.GetFileNameWithNewExtension(edmxFile, ".csdl");
                File.WriteAllText(csdlFileName, csdl.ToString());

                // select the ssdl element and write it out

                string ssdlFileName = EdmGen2Library.EdmGen2Library.GetFileNameWithNewExtension(edmxFile, ".ssdl");
                File.WriteAllText(ssdlFileName, ssdl.ToString());

                // select the msl element and write it out
                string mslFileName = EdmGen2Library.EdmGen2Library.GetFileNameWithNewExtension(edmxFile, ".msl");
                File.WriteAllText(mslFileName, msl.ToString());

                //Designer section also
                string desFileName = EdmGen2Library.EdmGen2Library.GetFileNameWithNewExtension(edmxFile, ".des");
                File.WriteAllText(desFileName, designerOut.ToString());
            }
        }

        private static void ToEdmx(string[] args)
        {
            //The Designer argument 5 is option
            if (args.Length < 4 || args.Length > 5)
            {
                ShowUsage();
                return;
            }

            FileInfo cFile, mFile, sFile, dFile;
            if (ParseCMSFileArguments(args, out cFile, out sFile, out mFile, out dFile))
            {
                String edmx;

                if (dFile != null)
                    edmx = EdmGen2Library.EdmGen2Library.ToEdmx(File.ReadAllText(cFile.FullName), File.ReadAllText(sFile.FullName), File.ReadAllText(mFile.FullName), File.ReadAllText(dFile.FullName));
                else
                    edmx = EdmGen2Library.EdmGen2Library.ToEdmx(File.ReadAllText(cFile.FullName), File.ReadAllText(sFile.FullName), File.ReadAllText(mFile.FullName), String.Empty);

                var fn = EdmGen2Library.EdmGen2Library.GetFileNameWithNewExtension(mFile, ".edmx");
                FileInfo outputFile = new FileInfo(fn);
                File.WriteAllText(outputFile.FullName, edmx);
            }
        }

        private static void ModelGen(string[] args)
        {
            if (args.Length < 4 || args.Length > 5)
            {
                PrintModelGenUsage();
                return;
            }
            string connectionString = args[1];
            string provider = args[2];
            string modelName = args[3];
            Version version = EntityFrameworkVersions.Version3;
            if (args.Length > 4)
            {
                if (args[4] == "1.0")
                {
                    version = EntityFrameworkVersions.Version1;
                }
                else if (args[4] == "2.0")
                {
                    version = EntityFrameworkVersions.Version2;
                }
                else if (args[4] == "3.0")
                {
                    version = EntityFrameworkVersions.Version3;
                }
            }

            bool includeForeignKeys = version >= EntityFrameworkVersions.Version2 ? true : false;
            if (args.Length > 5)
            {
                if (version >= EntityFrameworkVersions.Version2 && args[5] != "includeFKs")
                {
                    includeForeignKeys = true;
                }
                else
                {
                    PrintModelGenUsage();
                    return;
                }
            }

            List<Object> errors = null;
            String modelOut = String.Empty;
            if (EdmGen2Library.EdmGen2Library.ModelGen(connectionString, provider, modelName, version, includeForeignKeys, out modelOut, out errors))
            {
                //If errors, will return true
                var ret = WriteErrors(errors);
                if (ret)
                    return;
            }

            //File is only written if no errors
            FileInfo outputFile = new FileInfo(modelName + ".edmx");
            File.WriteAllText(outputFile.FullName, modelOut);
        }

        private static void CodeGen(string[] args)
        {
            if (args.Length != 3)
            {
                ShowUsage();
                return;
            }

            FileInfo edmxFile = null;
            LanguageOption languageOption;

            if (ParseLanguageOption(args[1], out languageOption))
            {
                if (ParseEdmxFileArguments(args[2], out edmxFile))
                {
                    String codeOut = String.Empty;
                    List<Object> errors = null;
                    if (EdmGen2Library.EdmGen2Library.CodeGen(edmxFile.FullName, languageOption, out codeOut, out errors))
                    {
                        //If errors, will return true
                        var ret = WriteErrors(errors);
                        if (ret)
                            return;
                    }

                    //write file if no errors
                    string outputFileName = Path.GetFileNameWithoutExtension(edmxFile.FullName) + EdmGen2Library.EdmGen2Library.GetFileExtensionForLanguageOption(languageOption);
                    File.WriteAllText(outputFileName, codeOut);
                }
            }
        }

        private static void ViewGen(string[] args)
        {
            if (args.Length != 3)
            {
                ShowUsage();
                return;
            }

            FileInfo edmxFile = null;
            LanguageOption langOpt;

            if (ParseLanguageOption(args[1], out langOpt))
            {
                if (ParseEdmxFileArguments(args[2], out edmxFile))
                {
                    String viewsOut = String.Empty;
                    List<Object> errors = null;
                    if (EdmGen2Library.EdmGen2Library.ValidateAndGenerateViews(edmxFile.FullName, langOpt, true, out viewsOut, out errors))
                    {
                        //If errors, will return true
                        var ret = WriteErrors(errors);
                        if (ret)
                            return;
                    }

                    // write out to a file if no errors
                    string outputFile = EdmGen2Library.EdmGen2Library.GetFileNameWithNewExtension(edmxFile, ".GeneratedViews" + EdmGen2Library.EdmGen2Library.GetFileExtensionForLanguageOption(langOpt));
                    File.WriteAllText(outputFile, viewsOut);
                }
            }
        }

        /// <summary>
        /// This uses ConcerptualEdmGen.dll for which there is no source.
        /// </summary>
        /// <param name="args"></param>
        private static void RetrofitModel(string[] args)
        {
            if (args.Length < 4 || args.Length > 5)
            {
                ShowUsage();
                return;
            }

            Generator cedm;
            if (args.Length == 5)
            {
                cedm = new ConceptualEdmGen.Generator(args[1], args[3], args[2], Convert.ToDouble(args[4]));
            }
            else
            {
                cedm = new ConceptualEdmGen.Generator(args[1], args[3], args[2]);
            }
            if (File.Exists("tables.txt"))
            {
                if (cedm.SetTables("tables.txt"))
                {
                    return;
                }
            }
            cedm.Execute();
        }

        private static void Validate(string[] args)
        {
            if (args.Length != 2)
            {
                ShowUsage();
                return;
            }

            FileInfo edmxFile = null;
            if (ParseEdmxFileArguments(args[1], out edmxFile))
            {
                String viewOut = String.Empty;
                List<Object> errors = null;
                if (EdmGen2Library.EdmGen2Library.ValidateAndGenerateViews(edmxFile.FullName, LanguageOption.GenerateCSharpCode, false, out viewOut, out errors))
                {
                    //If errors, will return true
                    var ret = WriteErrors(errors);
                }
            }
        }

        #endregion

        private static bool ParseEdmxFileArguments(string arg, out FileInfo fileInfo)
        {
            string edmxFile = arg;
            fileInfo = new FileInfo(edmxFile);
            if (!fileInfo.Exists)
            {
                System.Console.WriteLine("input file " + edmxFile + " does not exist");
                return false;
            }
            return true;
        }

        private static bool ParseCMSFileArguments(string[] args, out FileInfo cFile, out FileInfo sFile, out FileInfo mFile, out FileInfo dFile)
        {
            cFile = sFile = mFile = dFile = null;

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].EndsWith(".csdl", StringComparison.OrdinalIgnoreCase))
                {
                    cFile = new FileInfo(args[i]);
                }

                if (args[i].EndsWith(".ssdl", StringComparison.OrdinalIgnoreCase))
                {
                    sFile = new FileInfo(args[i]);
                }

                if (args[i].EndsWith(".msl", StringComparison.OrdinalIgnoreCase))
                {
                    mFile = new FileInfo(args[i]);
                }

                if (args[i].EndsWith(".des", StringComparison.OrdinalIgnoreCase))
                {
                    dFile = new FileInfo(args[i]);
                }
            }

            if (cFile == null)
            {
                Console.WriteLine("Error:  csdl file not specified");
            }
            if (sFile == null)
            {
                Console.WriteLine("Error:  ssdl file not specified");
            }
            if (mFile == null)
            {
                Console.WriteLine("Error:  msl file not specified");
            }

            if (args.Length == 5 && dFile == null)
            {
                Console.WriteLine("Error:  des file not specified");
            }

            if (!cFile.Exists)
            {
                Console.WriteLine("Error:  file " + cFile.FullName + " does not exist");
            }

            if (!sFile.Exists)
            {
                Console.WriteLine("Error:  file " + sFile.FullName + " does not exist");
            }

            if (!mFile.Exists)
            {
                Console.WriteLine("Error:  file " + mFile.FullName + " does not exist");
            }

            if (args.Length == 5 && !dFile.Exists)
            {
                Console.WriteLine("Error:  file " + dFile.FullName + " does not exist");
            }

            if (args.Length == 5)
            {
                if (cFile == null || sFile == null || mFile == null || dFile == null || !cFile.Exists || !sFile.Exists || !mFile.Exists || !dFile.Exists)
                {
                    return false;
                }
            }

            if (args.Length == 4)
            {
                if (cFile == null || sFile == null || mFile == null || !cFile.Exists || !sFile.Exists || !mFile.Exists)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ParseLanguageOption(string arg, out LanguageOption langOption)
        {
            langOption = LanguageOption.GenerateCSharpCode;
            if ("vb".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                langOption = LanguageOption.GenerateVBCode;
                return true;
            }
            else if ("cs".Equals(arg, StringComparison.OrdinalIgnoreCase))
            {
                langOption = LanguageOption.GenerateCSharpCode;
                return true;
            }
            else
            {
                ShowUsage();
                return false;
            }
        }

        private static bool WriteErrors(IEnumerable<Object> errors)
        {
            bool hasErrors = false;
            String message = String.Empty;
            if (errors != null)
            {
                foreach (Object e in errors)
                {
                    //At least one error will set true
                    hasErrors = EdmGen2Library.EdmGen2Library.GetError(e, out message);
                    Console.WriteLine(message);
                }
            }
            return hasErrors;
        }
    }
}
