using CSharpExtendedCommands.DataTypeExtensions.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            SetAvaliableLibraries(LibraryCollection.Load("libs.xml"));
            Menu = new MainMenu();
            Menu.MenuItems.Add("Project", new MenuItem[]
            {
                new("Apply changes", new EventHandler(OnApplyChanges)),
                new("Close", (o, e) => Close())
            });
            grid.CellContentClick += grid_CellContentClick;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            CheckedItems = new List<bool>();

            grid.DefaultCellStyle = new DataGridViewCellStyle()
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9.25f),
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = Color.Cyan,
                SelectionForeColor = Color.Black,
            };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.WhiteSmoke
            };
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
            var project_path = currentProj.ProjectPath /*+ "_mod.vcxproj"*/;
            foreach (var to_remove in remove)
            {
                if (HasLib(to_remove) == false) continue;
                lib_manager.RemoveLib(project_path, to_remove, ref currentProj, depDir);
            }
            FolderCopyForm copier = null;
            foreach (var to_install in install)
            {
                if (HasLib(to_install)) continue;
                lib_manager.InstallLib(to_install, ref currentProj, project_path, depDir, ref copier);
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
            Text = $"[Project View] -> {currentProj.ParentSolution.SolutionName} -> {currentProj.ProjectName}";
        }
        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var cell = grid[e.ColumnIndex, e.RowIndex];
            if (e.ColumnIndex == grid.ColumnCount - 1)
            {
                CheckedItems[e.RowIndex] = !CheckedItems[e.RowIndex];
            }
        }
    }
}
