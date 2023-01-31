using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VCXProjInterface;
using VCXProjInterface.LibraryAdder;

namespace Pkg__
{
    public class OnProjectDir
    {
        public OnProjectDir()
        {
        }
        public static string projectPath;
        public string Install(string libraryName, string projectName, Solution sol)
        {
            try
            {
                var proj = Project.Deserialize(projectName);
                var libs = Program.libraries.Where((lib) => lib.Name.ToLower() == libraryName.ToLower());
                if (libs.Count() == 0)
                {
                    return "No library named '" + libraryName + "'";
                }
                var install_lib = libs.First();

                if (proj.InstalledLibraries.Libraries.Where((lib) => lib.EqualsTo(install_lib)).Count() > 0)
                {
                    return "Library is already installed!";
                }

                Console.WriteLine("Installing [" + install_lib.Name+ "] to [" + projectName + "]...");

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
                var proj = Project.Deserialize(projectPath);

                var libs = Program.libraries.Where((lib) => lib.Name.ToLower() == libraryName.ToLower());
                if (libs.Count() == 0)
                {
                    return "No library named '" + libraryName + "'";
                }
                var remove_lib = libs.First();

                if (proj.InstalledLibraries.Libraries.Where((lib) => lib.EqualsTo(remove_lib)).Count() <= 0)
                {
                    return "Library is not installed!";
                }

                Console.WriteLine("Removing ["+remove_lib.Name+"] from [" + projectName + "]...");
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

            string proj = projectPath;
            string op = CONSOLE.args[0]; // first argument is always operation
            op = op[0].ToString().ToUpper() + op.Substring(1).ToLower();
            Operation operation = (op == null) ? Operation.Invalid
                : (Operation)(Enum.Parse(typeof(Operation), op));

            if (proj == null)
            {
                Console.WriteLine("No project selected. (use " + CONSOLE.ARG_NAME_MARKER + "p <project name>)");
            }
            string message = null;
            switch (operation)
            {
                case Operation.Invalid: message = "Invalid operation."; break;

                case Operation.Install: message = Install(CONSOLE.GetLibName(), proj, null); break;

                case Operation.Remove: message = Remove(CONSOLE.GetLibName(), proj); break;

                default: message = "Unknown operation."; break;
            }
            int return_value = (message.Contains("successfully")) ? 0 : -1;

            Console.WriteLine(message);
            return return_value;

        }

        public string ProjectFile { get; }
    }
}
