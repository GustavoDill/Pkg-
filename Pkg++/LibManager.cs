using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VCXProjInterface;

namespace Pkg__
{
    internal class LibManager
    {
        public static int Delete()
        {
            var name = CONSOLE.GetArg("name");
            if (name == null)
            {
                Console.WriteLine("Please provide a library name to remove!");
                return 0;
            }
            var collection = LibraryCollection.Load(Program.LOAD_LIBS);
            var l = collection.Libraries.ToList();
            bool found = false;
            int removeIndex = -1;
            for (int i = 0; i < collection.Libraries.Length; i++)
            {
                if (collection[i].Name == name)
                {
                    removeIndex = i;
                    found = true;
                    break;
                }       
            }
            if (!found)
            {
                Console.WriteLine("Library '" + name + "' not found");
                return 0;
            }

            if (CONSOLE.HasArg("force"))
            {
                Console.WriteLine("Are you shure you want to delete '" + name + "'?");
                char c = Console.ReadKey().KeyChar;
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                if (c != 'y' && c != 'Y')
                    return 0;
            }

            Console.WriteLine("Deleting library...");
            try { Directory.Delete(l[removeIndex].LibraryPath, true); } catch (Exception ex){ Console.WriteLine("Failed to delete library directory:\n\t" + ex.Message);  return -1; }
            l.RemoveAt(removeIndex);
            collection.Libraries = l.ToArray();
            collection.Save(Program.LOAD_LIBS);
            Console.WriteLine("Successfully deleted '" + name + "'");
            return 0;
        }

        public static int Add()
        {
            var name = CONSOLE.GetArg("name");
            var path = CONSOLE.GetArg("path");
            var version = CONSOLE.GetArg("version");
            var collection = LibraryCollection.Load(Program.LOAD_LIBS);
            var l = collection.Libraries.ToList();

            if (name == null ||
                path == null ||
                version == null)
            {
                Console.WriteLine("Invalid arguments (67)");
                return -1;
            }
            int libraryIndex = -1;
            for (int i = 0; i < collection.Libraries.Length; i++)
            {
                Library lib = collection.Libraries[i];
                if (lib.Name == name)
                {
                    if (!CONSOLE.HasArg("force"))
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
            for (int i = 0; i < CONSOLE.args.Length; i++)
            {
                if (CONSOLE.args[i].StartsWith("-config"))
                {
                    // add -config x64 ["include", "include\msvc"] ["libPaths";"separated like this"]

                    var m = Regex.Match(CONSOLE.args[i + 2], @"\[([\s\w\\;\.]+)\]");
                    var m2 = Regex.Match(CONSOLE.args[i + 3], @"\[([\s\w\\;\.]+)\]");
                    if (m.Success == false || m2.Success == false) { Console.WriteLine("Invalid arguments (109)"); return -1; }

                    string[] includePaths = m.Groups[1].Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] libPaths = m2.Groups[1].Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    configs.Add(new()
                    {
                        Platform = CONSOLE.args[i + 1],
                        LibPaths = libPaths,
                        IncludePaths = includePaths
                    });
                    i += 2;

                }
            }



            if (CONSOLE.HasArg("copy"))
            {

                var newPath = Path.Combine(new FileInfo(Program.LOAD_LIBS).DirectoryName, name + "-" + version);
                if (Directory.Exists(newPath))
                {
                    if (!CONSOLE.HasArg("force"))
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
                try { Console.WriteLine("Copying library..."); CopyDirectoryRecursive(new DirectoryInfo(path), new DirectoryInfo(newPath)); } catch (Exception ex) { Console.WriteLine("Failed to copy library directory:\t" + ex.Message); return -1; }
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
            collection.Save(Program.LOAD_LIBS);



            Console.WriteLine("Library added successfully!");
#if _DEBUG
#if true
            Console.ReadKey();
#endif
#endif
            return 0;
        }


        public static string RelPath(DirectoryInfo rootSrc, DirectoryInfo dest, string current)
        {
            return Path.Combine(
                dest.FullName,
                current.Substring(rootSrc.FullName.Length + 1)
                );
        }
        public static void CopyDirectoryRecursive(DirectoryInfo src, DirectoryInfo dest)
        {
            foreach (var d in src.GetDirectories("*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(RelPath(src, dest, d.FullName));
            }

            if (dest.Exists == false) dest.Create();

            foreach (var f in src.GetFiles("*", SearchOption.AllDirectories))
            {
                f.CopyTo(RelPath(src, dest, f.FullName));
            }
        }
    }

}
