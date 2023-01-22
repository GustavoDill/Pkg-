using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using VCXProjInterface;
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

    internal class Program
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
            CONSOLE.args = args;
            libraries = LibraryCollection.Load(LOAD_LIBS).Libraries;
            if (IsSolution(args))
            {
#if DEBUG
#else
                return 
#endif

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
            return CONSOLE.GetArg("s") != null;
        }


        Project project;
        public static void Help()
        {
            Console.WriteLine("Usage: <pkg++> <args>");
        }
    }
}
