using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCXProjInterface;

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
        public static string ARG_NAME_MARKER = "--";
        public static string GetArg(string argName, bool ignoreCase = true)
        {
            for (int i = 0; i < args.Length; i++)
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
    }
    internal class Program
    {
        static int Main(string[] args)
        {
            CONSOLE.args = args;
            if (IsSolution(args))
            {
#if DEBUG
#else
                return 
#endif
                new OnSolutionDir().Main();
            }
            else if (IsProject(args))
            {
#if DEBUG
#else
                return
#endif
                new OnProjectDir().Main();
            }
            else
            {
                Help();
#if DEBUG
#else
                return 0;
#endif
            }
#if DEBUG
            Console.ReadKey();
            return 0;
#endif
        }
        static bool IsSolution(string[] args)
        {
            return CONSOLE.GetArg("solution") != null;
        }

        static bool IsProject(string[] args)
        {
            return CONSOLE.GetArg("project") != null;
        }
        static void LoadProject(string path)
        {

        }


        Project project;
        static void Help()
        {
            Console.WriteLine("Usage: <pkg++> <args>");
        }
    }
}
