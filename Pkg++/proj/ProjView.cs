using CSharpExtendedCommands.DataTypeExtensions.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCXProjInterface;
using VCXProjInterface.LibraryAdder;

namespace Pkg__
{
    public partial class ProjView : Form
    {
        public ProjView()
        {
            InitializeComponent();
            label1.CenterOnContainerX();
            SetAvaliableLibraries(LibraryCollection.Load("libs.xml"));
            Menu = new MainMenu();
            Menu.MenuItems.Add("Files", new MenuItem[]
            {
                new MenuItem("Apply changes", new EventHandler(OnApplyChanges))
            });
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            CheckedItems = new List<bool>();
        }
        public List<bool> CheckedItems;
        public void OnApplyChanges(object sender, EventArgs e)
        {
            var remove = GetToBeRemoved();
            var install = GetToBeInstalled();

            LibraryCollection newCol = new LibraryCollection();
            List<Library> newLibs = new List<Library>();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (CheckedItems[row.Index])
                    newLibs.Add(avaliableLibraries[row.Index]);
            }


            var projDir = new FileInfo(currentProj.ProjectPath).DirectoryName;
            var depDir = Path.Combine(projDir, "deps");
            var p_path = currentProj.ProjectPath + "_mod.vcxproj";
            foreach (var to_remove in remove)
            {
                if (HasLib(to_remove) == false) continue;
                p_path.RemoveLibraryFromFile(to_remove, GetRelLibDir(ref currentProj, to_remove, depDir));
                if (Directory.Exists(GetLibDir(to_remove, depDir)))
                {
                    Directory.Delete(GetLibDir(to_remove, depDir), true);
                }
            }
            FolderCopyForm copier;
            foreach (var to_install in install)
            {
                if (HasLib(to_install)) continue;
                p_path.AddLibraryToFile(to_install, GetRelLibDir(ref currentProj, to_install, depDir));
#if true
                copier = new FolderCopyForm();
                copier.Text = "Installing " + to_install.Name + " - Version " + to_install.Version;
                copier.CopyFolder(to_install.LibraryPath, Path.Combine(
                    depDir,
                    to_install.Name + "-" + to_install.Version));
                copier.ShowDialog();
#endif
            }

            currentProj.InstalledLibraries.Libraries = newLibs.ToArray();
#if true
            currentProj.InstalledLibraries.Save(currentProj.LibDefFile);
#endif
            MessageBox.Show("Changes applied successfully!", "Apply Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        bool HasLib(Library lib)
        {
            for (int i = 0; i < currentProj.InstalledLibraries.Libraries.Length; i++)
            {
                if (currentProj.InstalledLibraries[i].EqualsTo(lib)) return true;
            }
            return false;
        }
        private string GetRelLibDir(ref Project proj, Library lib, string depDir)
        {
            var info = new FileInfo(proj.ProjectPath);
            var parentDir = info.Directory;
            var libDir = GetLibDir(lib, depDir);
            return libDir.Substring(parentDir.FullName.Length+1);
        }
        private string GetLibDir(Library lib, string depDir)
        {
            return Path.Combine(depDir, lib.Name + "-" + lib.Version);
        }
        public Library[] GetToBeRemoved()
        {
            List<Library> libraries = new List<Library>();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (!((bool)row.Cells[row.Cells.Count - 1].EditedFormattedValue))
                {
                    libraries.Add(avaliableLibraries.Libraries[row.Index]);
                }
            }
            return libraries.ToArray();
        }
        public Library[] GetToBeInstalled()
        {
            List<Library> libraries = new List<Library>();
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (((bool)row.Cells[row.Cells.Count - 1].EditedFormattedValue) == true)
                {
                    libraries.Add(avaliableLibraries.Libraries[row.Index]);
                }
            }
            return libraries.ToArray();
        }

        public void SetAvaliableLibraries(LibraryCollection libs)
        {
            foreach (var lib in libs.Libraries)
            {
                grid.Rows.Add(lib.Name, lib.Version, lib.LibraryPath, false);
            }
            avaliableLibraries = libs;
        }
        LibraryCollection avaliableLibraries;
        bool LibraryInstalled(string name, string version, string libraryPath)
        {
            foreach (var lib in currentProj.InstalledLibraries.Libraries)
            {
                if (lib.Name == name &&
                    lib.Version == version &&
                    lib.LibraryPath == libraryPath)
                    return true;
            }
            return false;
        }

        bool LibraryInstalled(DataGridViewRow lib)
        {
            return LibraryInstalled(lib.Cells[0].Value.ToString(), lib.Cells[1].Value.ToString(), lib.Cells[2].Value.ToString());
        }
        Project currentProj;
        internal void SetProject(VCXProjInterface.Project proj)
        {
            currentProj = proj;
            foreach (DataGridViewRow row in grid.Rows)
            {
                var installed = LibraryInstalled(row);
                CheckedItems.Add(installed);
                row.Cells[row.Cells.Count - 1].Value = installed;
                    
            }
            label1.Text = proj.ProjectName;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == grid.ColumnCount - 1)
            {
                CheckedItems[e.RowIndex] = !CheckedItems[e.RowIndex];
            }
        }
    }
}
