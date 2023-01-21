using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCXProjInterface.LibraryAdder;
using VCXProjInterface;
using System.Security.Policy;

namespace Pkg__
{
    public static  class lib_manager
    {
        public static void InstallLib(Library to_install, ref Project curProj, string proj_path, string depDir, ref FolderCopyForm copier)
        {
            proj_path.AddLibraryToFile(to_install, GetRelLibDir(ref curProj, to_install, depDir));

            if (Settings.CopyLibraries)
            {
                copier = new FolderCopyForm();
                copier.Text = "[Installing Library] " + to_install.Name + " - Version " + to_install.Version;
                copier.CopyFolder(to_install.LibraryPath, Path.Combine(
                    depDir,
                    to_install.Name + "-" + to_install.Version));
                copier.ShowDialog();
            }
        }
        public static void RemoveLib(string project_path, Library to_remove, ref Project currentProject, string depDir)
        {
            project_path.RemoveLibraryFromFile(to_remove, GetRelLibDir(ref currentProject, to_remove, depDir));
            if (Directory.Exists(GetLibDir(to_remove, depDir)))
            {
                Directory.Delete(GetLibDir(to_remove, depDir), true);
            }
        }

        public static string GetRelLibDir(ref Project proj, Library lib, string depDir)
        {
            var info = new FileInfo(proj.ProjectPath);
            var parentDir = info.Directory;
            var libDir = GetLibDir(lib, depDir);
            return libDir.Substring(parentDir.FullName.Length + 1);
        }
        public static string GetLibDir(Library lib, string depDir)
        {
            return Path.Combine(depDir, lib.Name + "-" + lib.Version);
        }
    }
}
