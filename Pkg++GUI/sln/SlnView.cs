using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCXProjInterface;

namespace Pkg__
{
    public partial class SlnView : Form
    {
        public SlnView()
        {
         
            InitializeComponent();
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Menu = new MainMenu(new MenuItem[]
            {
                new("Solution", new MenuItem[]
                {
                    new("Close", (o, e) => Close())
                })
            });
            projView = new ProjView();
            projView.StartPosition = FormStartPosition.Manual;
            projView.FormClosing += ProjView_FormClosing;

            grid.MultiSelect = false;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            projView.Size = Size;
        }
        private void ProjView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Show();
            projView.Hide();
        }

        Solution sln;
        public void SetSln(string fileName)
        {
            grid.Rows.Clear();
            sln = new VCXProjInterface.Solution(fileName);
            foreach (var proj in sln.Projects)
                grid.Rows.Add(proj.ProjectName, proj.ProjectGUID, proj.ProjectPath);

            Text = "[Solution View] -> " + sln.SolutionName;
        }

        private void SlnView_Load(object sender, EventArgs e)
        {

        }
        void ShowProject(Project proj)
        {
            projView.Location = Location;
            projView.Show();
            projView.SetProject(proj);
            Hide();
        }
        ProjView projView;
        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = grid.SelectedRows[0].Index;
            var proj = sln.Projects[row];
            ShowProject(proj);
        }
    }
}
