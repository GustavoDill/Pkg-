using CSharpExtendedCommands.UI.CSWinAnimator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Pkg__
{

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Menu = new MainMenu(new MenuItem[]
            {
                new("File", new MenuItem[]
                {
                    new("Open solution", ((o,e)=> OpenSolution())),
                    new("Close", (o, e) => Close())
                })
            });


            slnIcon = Manager.Resources["SolutionIcon"].Get<Image>();
            list.ItemSize = new Size(list.Width, new SolutionItem().Height);
            if (!File.Exists("recent_solutions.txt"))
                File.WriteAllLines("recent_solutions.txt", new string[0]);

            var s = File.ReadAllLines("recent_solutions.txt").ToList();
            s.RemoveAll((str) => string.IsNullOrEmpty(str));
            RecentSolutions = s.ToArray();
            foreach (var sln in RecentSolutions)
            {
                AddSln(sln);
            }
            slnView = new SlnView();
            slnView.Size = Size;
            slnView.StartPosition = FormStartPosition.Manual;
            slnView.FormClosing += SlnView_Closed;
        }

        private void SlnView_Closed(object sender, FormClosingEventArgs e)
        {
            slnView.Hide();
            e.Cancel = true;
            Location = slnView.Location;
            Show();
        }

        void AddSln(string sln)
        {
            var s = new SolutionItem() { Icon = slnIcon, Text = sln, Tag = list.Items.Length, 
                BackColor = Color.Cyan,
                SecondBackColor = Color.RoyalBlue};
            s.DoubleClick += Sln_DoubleClick;
            s.ItemClick += S_ItemClick;
            list.AddItem(s);
        }
        SolutionItem selItem;
        private void S_ItemClick(object sender, EventArgs e)
        {
            if (selItem?.Text == ((SolutionItem)sender).Text)
            { selItem.SetState(SolutionItem.ItemState.SelectedHover); return; }
            if (selItem != null)
                selItem.SetState(SolutionItem.ItemState.Normal);
            selItem = (SolutionItem)sender;
        }

        private SolutionItem GetSln(int index)
        {
            return (SolutionItem)list.Items[index];
        }
        private void Sln_DoubleClick(object sender, EventArgs e)
        {
            OpenSolution(((SolutionItem)sender).Text);
        }

        public string[] RecentSolutions { get; set; }
        public void WriteRecentSln(string sln)
        {
            var slns = RecentSolutions.ToList();
            slns.Add(sln);
            RecentSolutions = slns.ToArray();
            File.WriteAllLines("recent_solutions.txt", RecentSolutions);
            AddSln(sln);
        }
        Image slnIcon;

        SlnView slnView;
        private void OpenSolution(string fileName)
        {
            // Open solution
            //NavigateTo(typeof(SolutionView), fileName);
            slnView.Location = Location;
            slnView.Show();
            slnView.SetSln(fileName);

            Hide();
        }
        private void smoothButton1_Click(object sender, EventArgs e)
        {
            OpenSolution();
        }
        void OpenSolution()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "Solution file (*.sln)|*.sln"
            };
            if (ofd.ShowDialog() == DialogResult.Cancel) return;

            WriteRecentSln(ofd.FileName);
            OpenSolution(ofd.FileName);
        }
        FolderCopyForm c;
        private void smoothButton2_Click_2(object sender, EventArgs e)
        {
            c = new FolderCopyForm();
            
            c.CopyFolder(
                "C:\\libs\\SDL2-2.26.1",
                "C:\\libs\\COPY SDL2-2.26.1"
                );
            c.ShowDialog();
        }
    }

}
