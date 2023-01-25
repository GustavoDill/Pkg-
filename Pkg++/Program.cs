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
            if (args[0] == "add")
            {
                var name = CONSOLE.GetArg("name");
                var path = CONSOLE.GetArg("path");
                var version = CONSOLE.GetArg("version");
                var collection = LibraryCollection.Load(LOAD_LIBS);
                var l = collection.Libraries.ToList();
                
                if (name == null ||
                    path == null ||
                    version == null)
                {
                    Console.WriteLine("Invalid arguments");
                    return -1;
                }
                int libraryIndex = -1;
                for (int i = 0; i < collection.Libraries.Length; i++)
                {
                    Library lib = collection.Libraries[i];
                    if ( lib.Name == name)
                    {
                        if (CONSOLE.GetArg("force") == null)
                        {
                            Console.WriteLine("Library already exists. Overwrite (y/n):");
                            char c = Console.ReadKey().KeyChar;
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            if (c != 'y' && c != 'Y')
                            {
                                Console.WriteLine("Aborted");
                                return -1;
                            }
                        }
                        Console.WriteLine("Updating library...");
                        libraryIndex = i;
                        //l[i] = null;
                        break;
                    }
                }
                if (libraryIndex == -1)
                {
                    libraryIndex = collection.Libraries.Length;
                    l.Add(null); // now libraryIndex is valid index and is last index.
                }


                var configs = new List<Library.LibConfig>();
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-config"))
                    {
                        // add -config x64 ["include", "include\msvc"] ["libPaths";"separated like this"]

                        var m = Regex.Match(args[i + 2], @"\[([\s\w\\;\.]+)\]");
                        var m2 = Regex.Match(args[i + 3], @"\[([\s\w\\;\.]+)\]");
                        if (m.Success ==false || m2.Success==false) { Console.WriteLine("Invalid arguments"); return -1; }

                        string[] includePaths = m.Groups[1].Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] libPaths = m2.Groups[1].Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        configs.Add(new()
                        {
                            Platform = args[i + 1],
                            LibPaths = libPaths,
                            IncludePaths = includePaths
                        });
                        i += 2;
                        
                    }
                }



                if (CONSOLE.GetArg("copy") != null)
                {

                    var newPath = Path.Combine(new FileInfo(LOAD_LIBS).DirectoryName, name + "-" + version);
                    if (Directory.Exists(newPath))
                    {
                        if (CONSOLE.GetArg("force") == null)
                        {
                            Console.WriteLine("Library directory already exists. Overwrite (y/n):");
                            char c = Console.ReadKey().KeyChar;
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            if (c != 'y' && c != 'Y')
                            {
                                Console.WriteLine("Aborted");
                                return 0;
                            }
                        }
                        Console.WriteLine("Deleting old library...");
                        Directory.Delete(newPath, true);


                    }
                    try { Console.WriteLine("Copying library..."); CopyDirectoryRecursive(new DirectoryInfo(path), new DirectoryInfo(newPath)); } catch { Console.WriteLine("Failed to copy library directory"); return -1; }
                    path = newPath;
                }
                var addLib = new Library()
                {
                    Name = name,
                    LibraryPath = path,
                    Version = version,
                };


                addLib.Configurations = configs.ToArray();
                l[libraryIndex] = addLib;
                collection.Libraries = l.ToArray();
                collection.Save(LOAD_LIBS);



                Console.WriteLine("Library added successfully!");
#if DEBUG
#if true
                Console.ReadKey();
#endif
#endif
                return 0;
            }
            else if (args[0] == "add" && CONSOLE.GetArg("gui") != null)
            {

                return 0;
            }


#if _DEBUG
            return 0;
#endif
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


        public static void Help()
        {
            Console.WriteLine("Usage: <pkg++> <args>");
        }

        static string RelPath(DirectoryInfo rootSrc, DirectoryInfo dest, string current)
        {
            // C:\source\folder\tocopy\
            // C:\libs\dest
            // C:\source\folder\tocopy\some\other\folder
            return Path.Combine(
                dest.FullName,
                current.Substring(rootSrc.FullName.Length + 1)
                );
        }
        static void CopyDirectoryRecursive(DirectoryInfo src, DirectoryInfo dest)
        {
            foreach (var d in src.GetDirectories("*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(RelPath(src, dest, d.FullName));
            }
            foreach (var f in src.GetFiles("*", SearchOption.AllDirectories))
            {
                f.CopyTo(RelPath(src, dest, f.FullName));
            }
        }
    }
}
