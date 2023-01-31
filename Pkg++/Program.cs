using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VCXProjInterface;
using VS;
using static System.Net.WebRequestMethods;

namespace Pkg__
{
    public enum Operation
    {
        Invalid,
        Install,
        Remove
    }
    public static class CONSOLE
    {
        public static string[] args;
        public static string ARG_NAME_MARKER = "-";
        public static bool HasArg(string argName, bool ignoreCase = true)
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith(ARG_NAME_MARKER))
                {
                    if (string.Compare(args[i].Substring(ARG_NAME_MARKER.Length), argName, ignoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static string GetArg(string argName, bool ignoreCase = true)
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith(ARG_NAME_MARKER))
                {
                    if (string.Compare(args[i].Substring(ARG_NAME_MARKER.Length), argName, ignoreCase) == 0)
                    {
                        return args[i + 1];
                    }
                }
            }
            return null;
        }
        public static string GetLibName()
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i - 1].StartsWith(ARG_NAME_MARKER) == false &&
                    args[i].StartsWith(ARG_NAME_MARKER) == false)
                    return args[i];
            }
            return null;
        }

    }

    public class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        }
        public static Library[] libraries;
        public static string LOAD_LIBS = "C:\\libs\\libs.xml";
        static int Main(string[] args)
        {

            //return 0;
            if (args.Length == 0)
            {
                Help();
                return 0;
            }
            CONSOLE.args = args;
            if (args[0] == "list")
            {
                var cl = LibraryCollection.Load(LOAD_LIBS);
                foreach (var lib in cl.Libraries)
                {
                    Console.WriteLine("---- " + lib.Name + " ----");
                    Console.WriteLine("\tVersion: " + lib.Version);
                    Console.WriteLine("\tPath: " + lib.LibraryPath);
                    Console.WriteLine();
                    //Console.WriteLine($"{lib.Name,15}{lib.Version,10}{lib.LibraryPath,10}");
                }
#if _DEBUG
                Console.ReadKey();
#endif
                return 0;
            }
            if (args[0] == "delete")
            {
                return LibManager.Delete();
            }

            if (args[0] == "add")
            {
                return LibManager.Add();
            }
            else if (args[0] == "add" && CONSOLE.HasArg("gui"))
            {
                Console.WriteLine("Function not avaliable yet!");
                return 0;
            }
            libraries = LibraryCollection.Load(LOAD_LIBS).Libraries;
            if (IsSolution(args))
            {
                new OnSolutionDir()
                {
                    SolutionFile = CONSOLE.GetArg("s")
                }.Main();
            }
            else if (args.Contains("install") || args.Contains("remove") && args.Length >= 2)
            {
                var r = testProjDir();
                if (r == 0) return r;
                r = testSlnDir();
                if (r == 0) return r;
                else Help();
            }

            return 0;

        }

        static int testProjDir()
        {
            var files = new DirectoryInfo(Environment.CurrentDirectory).GetFiles("*.vcxproj");
            if (files.Length > 1)
            {
                Console.WriteLine("Too many project inside current directory!");
                return -1;
            }
            else if (files.Length == 1)
            {
                OnProjectDir.projectPath = files[0].FullName;
                return new OnProjectDir().Main();
            }
            else
            {
                Console.WriteLine("Invalid arguments");
            return -1;
            }
        }
        static int testSlnDir()
        {
            var files = new DirectoryInfo(Environment.CurrentDirectory).GetFiles("*.sln");
            if (files.Length > 1)
            {
                Console.WriteLine("Too many solution files inside current directory!");
                return -1;
            }
            else if (files.Length == 1)
            {
                return new OnSolutionDir()
                {
                    SolutionFile = files[0].FullName
                }.Main();
            }
            else
            {
                Console.WriteLine("No solution/projects selected (-s/-p)");
                return -1;
            }
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("VCXProjInterface"))
            {
                var res = Assembly.GetCallingAssembly().GetManifestResourceStream("Pkg__.VCXProjInterface.dll");
                BinaryReader reader = new BinaryReader(res);
                var assembly_data = reader.ReadBytes((int)reader.BaseStream.Length);
                reader.Close();
                return Assembly.Load(assembly_data);
            }
            return null;
        }

        static bool IsSolution(string[] args)
        {
            return CONSOLE.HasArg("s");
        }


        public static void Help()
        {
            Console.WriteLine("Usage:\npkg++ <add/delete install/remove list> [args]\n");
            MainArg("add", "Adds a library to the avaliable library list. Takes <name> <version> <path> [copy] [force]");
            MainArg("delete", "Deletes a library from the avaliable library list. Takes <name>");
            MainArg("install", "Installs a library in a C++ project. Takes <name>");
            MainArg("remove", "Removes a library from a C++ project. Takes <name>");
            MainArg("list", "Lists the avaliable libraries");
            Console.WriteLine("");
            Arg("name", "Targets the name of the library");
            Arg("version", "Defines the library version");
            Arg("path", "Defines the library path to copy from");
            Arg("copy", "Defines that it must copy the library from 'path'");
            Arg("force", "Remove prompts");
            
        }

        public static void MainArg(string name, string desc)
        {
            Console.WriteLine("\t{0}\t:\t{1}", name, desc);
            //Console.ReadKey();
        }

        public static void Arg(string name, string desc)
        {
            Console.WriteLine("\t{0}\t:\t{1}", name, desc);
            
        }

    }
}
