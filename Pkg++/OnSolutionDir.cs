using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VCXProjInterface;
using VCXProjInterface.LibraryAdder;

namespace Pkg__
{
    public class OnSolutionDir
    {
        public OnSolutionDir()
        {

        }
        public string SolutionFile { get; set; }
        Solution solution;

        public string Install(string libraryName, string projectPath)
        {
            try
            {
                Project proj;
                if (new FileInfo(projectPath).Extension == "vcxproj")
                {
                    proj = Project.Deserialize(projectPath);
                    Console.WriteLine("Installing [" + libraryName + "] to [" + new FileInfo(proj.ProjectPath).Name + "]...");
                }
                else
                {
                    foreach (var projs in solution.Projects)
                        if (projs.ProjectName == projectPath)
                        {
                            proj = projs;
                            Console.WriteLine("Installing [" + libraryName + "] to [" + proj.ProjectName + "]...");
                            goto ValidProject;
                        }
                    return "Invalid project selected";
                }
                ValidProject:


                var install_lib = Program.libraries.Where((lib) => lib.Name.ToLower() == libraryName.ToLower()).First();

                if (proj.InstalledLibraries.Libraries.Where((lib) => lib.EqualsTo(install_lib)).Count() > 0)
                {
                    return "Library is already installed!";
                }

                var l = proj.InstalledLibraries.Libraries.ToList();
                l.Add(install_lib);
                proj.InstalledLibraries.Libraries = l.ToArray();
                proj.InstalledLibraries.Save(proj.LibDefFile);
                proj.ProjectPath.AddLibraryToFile(install_lib);
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#else
                return ex.Message;
#endif
            }
            return "Library installed successfully!";
            //proj_path.AddLibraryToFile(to_install, GetRelLibDir(ref curProj, to_install, depDir));
        }

        public string Remove(string libraryName, string projectName)
        {
            try
            {
                Project proj;
                if (new FileInfo(projectName).Extension == "vcxproj")
                {
                    proj = Project.Deserialize(projectName);
                    Console.WriteLine("Removing [" + libraryName + "] to [" + new FileInfo(proj.ProjectPath).Name + "]...");
                }
                else
                {
                    foreach (var projs in solution.Projects)
                        if (projs.ProjectName == projectName)
                        {
                            proj = projs;
                            Console.WriteLine("Removing [" + libraryName + "] to [" + proj.ProjectName + "]...");
                            goto ValidProject;
                        }
                    return "Invalid project selected";
                }
            ValidProject:
                //Console.WriteLine("Removing library from [" + projectName + "]...");
                //var proj = solution.Projects.Where((p) =>
                //{
                //    return p.ProjectName.ToLower() == projectName.ToLower();
                //}).First();

                var remove_lib = Program.libraries.Where((lib) => lib.Name.ToLower() == libraryName.ToLower()).First();

                if (proj.InstalledLibraries.Libraries.Where((lib) => lib.EqualsTo(remove_lib)).Count() <= 0)
                {
                    return "Library is not installed!";
                }
                var l = proj.InstalledLibraries.Libraries.ToList();
                for (int i = 0; i < proj.InstalledLibraries.Libraries.Length; i++)
                {
                    if (proj.InstalledLibraries[i].EqualsTo(remove_lib))
                    {
                        l.RemoveAt(i);
                        break;
                    }
                }
                proj.InstalledLibraries.Libraries = l.ToArray();
                proj.InstalledLibraries.Save(proj.LibDefFile);

                proj.ProjectPath.RemoveLibraryFromFile(remove_lib);
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#else
                return ex.Message; 
#endif

            }
            return "Library removed successfully!";
        }

        public int Main()
        {
            if (File.Exists(SolutionFile)) solution = new Solution(SolutionFile);
            else
            {
                Console.WriteLine("Solution file not found!");
                return -1;
            }
            // Get param "--install" or "--remove" to decide along with the library

            string proj = CONSOLE.GetArg("p");
            string op = CONSOLE.args[0]; // first argument is always operation
            op = op[0].ToString().ToUpper() + op.Substring(1).ToLower();
            Operation operation = (op == null) ? Operation.Invalid
                : (Operation)(Enum.Parse(typeof(Operation), op));

            if (proj == null)
            {
                Console.WriteLine("No project selected. (use " + CONSOLE.ARG_NAME_MARKER + "p <project name>)");
                return 0;
            }
            string message = null;
            switch (operation)
            {
                case Operation.Invalid: message = "Invalid operation."; break;

                case Operation.Install: message = Install(CONSOLE.GetLibName(), proj); break;

                case Operation.Remove: message = Remove(CONSOLE.GetLibName(), proj); break;

                default: message = "Unknown operation."; break;
            }
            int return_value = (message.Contains("successfully")) ? 0 : -1;

            Console.WriteLine(message);
            return return_value;



        }


    }
}
