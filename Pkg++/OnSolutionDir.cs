using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VCXProjInterface;

namespace Pkg__
{
    public class OnSolutionDir
    {
        public OnSolutionDir()
        {

        }
        public string SolutionFile { get; set; }
        public void GetSolutionFile()
        {
            SolutionFile = CONSOLE.GetArg("solution");

        }
        Solution solution;

        public void Install(string libraryName, string projectName)
        {

        }
        public int Main()
        {
            GetSolutionFile();
            if (File.Exists(SolutionFile)) solution = new Solution(SolutionFile);
            else
            {
                Console.WriteLine("Solution file not found!");
                return -1;
            }
            // Get param "--install" or "--remove" to decide along with the library
            // Use Operation.Invalid if none;

            // Check if --project-name exists, then select that project
            // else, Print solution projects and allow choice. (with option for all i guess)



            Console.WriteLine("Solution: " + SolutionFile);
            return 0;
        }
    }
}
