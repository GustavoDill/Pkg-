using CSharpExtendedCommands.DataTypeExtensions.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Pkg__
{
    public partial class FolderCopyForm : Form
    {
        public FolderCopyForm()
        {
            InitializeComponent();
            Paint += FolderCopyForm_Paint;
        }
        public int exitTimeout = 1500;
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }
        private void FolderCopyForm_Paint(object sender, PaintEventArgs e)
        {
            var m = TextRenderer.MeasureText(ProgressPercentage, Font);
            TextRenderer.DrawText(CreateGraphics(), ProgressPercentage, Font, new Point(
                bar.Left + bar.Width / 2 - m.Width / 2,
                bar.Top + m.Height / 2
                ), ForeColor, Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        FolderCopier copier;

        public void CopyFolder(string source, string dest)
        {
            copier = new FolderCopier(source, dest);
            SetProgressValue(0);
            SetProgressMax(copier.TotalFolders + copier.TotalFiles);
            CalculatePercentage();
            copier.ProgressChange += Copier_ProgressChange;
            copier.Finished += Copier_Finished;
            copier.StartCopy();
        }
        public event EventHandler CopyFinished;
        private void Copier_Finished(string file, int progress)
        {
            CopyFinished?.Invoke(this, EventArgs.Empty);
            SetProgressValue(bar.Maximum);
            SetFile("Complete!");
            Thread.Sleep(exitTimeout);
            if (InvokeRequired)
                Invoke((MethodInvoker)(() =>
                {
                    Close();
                }));
            else
            Close(); 
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
        }

        public string ProgressPercentage;
        private void Copier_ProgressChange(string file, int progress)
        {
            var method = ((MethodInvoker)(() => {
                bar.Value++;
                label1.Text = file;
                CalculatePercentage();
            }));
            try
            {
                if (InvokeRequired)
                    Invoke(method);
                else
                    method();
            }
            catch(ObjectDisposedException) { copier.Cancel();return; }
            
        }

        public void SetFile(string file)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => { label1.Text = file; }));
            else
                label1.Text = file;
        }
        public void SetProgressMax(int value)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => { bar.Maximum = value; }));
            else
                bar.Maximum = value;
        }
        public void SetProgressValue(int value)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => { bar.Value = value; }));
            else
                bar.Value = value;
        }
        public void SetPercentage(int percent)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => { label2.Text = $"{percent}%";Invalidate(); }));
            else
            {
                label2.Text = $"{percent}%";
                Invalidate();
            }
        }
        int Percent(int value, int max)
        {
            return value * 100 / max;
        }
        void CalculatePercentage()
        {
            SetPercentage((int)Percent(bar.Value, bar.Maximum));
        }

        private void FolderCopyForm_Load(object sender, EventArgs e)
        {

        }
    }
    public class NoBackGroundText : Control
    {
        public NoBackGroundText()
        {

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var m = TextRenderer.MeasureText(Text, Font);
            TextRenderer.DrawText(e.Graphics, Text, Font, new Point(Width + 3, Height / 2), ForeColor, Background ? Color.White : Color.Transparent, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Background)
                base.OnPaintBackground(pevent);
        }
        public bool Background = true;
    }
    public class FolderCopier
    {
        public FolderCopier(string source, string destination)
        {

            this.source = new DirectoryInfo(source);
            dirs = this.source.GetDirectories("*", SearchOption.AllDirectories);
            files = this.source.GetFiles("*", SearchOption.AllDirectories);
            TotalFolders = dirs.Length;
            TotalFiles = files.Length;
            dest = new DirectoryInfo(destination);
            copyThread = new Thread(Copy);
        }
        public void StartCopy()
        {
            //ProgressChange?.Invoke(dirs[0].FullName, 0);
            copyThread.Start();
        }
        DirectoryInfo dest;
        DirectoryInfo[] dirs;
        FileInfo[] files; 
        DirectoryInfo source;
        Thread copyThread;
        public delegate void ProgressHandler(string file, int progress);
        public event ProgressHandler ProgressChange;
        public event ProgressHandler Finished;
        public void Cancel()
        {
            copyThread.Abort();
        }
        string RelPath(string dir)
        {
            return Path.Combine(dest.FullName,
                    dir.Substring(source.FullName.Length + 1));
        }
        void Copy()
        {
            Thread.Sleep(200);
            for (int i = 0; i < dirs.Length; i++)
            {
                ProgressChange?.Invoke(dirs[i].FullName, i);
                Directory.CreateDirectory(RelPath(dirs[i].FullName));
            }
            for (int i = 0; i < files.Length; i++)
            {
                ProgressChange?.Invoke(files[i].FullName, i+dirs.Length);
                files[i].CopyTo(RelPath(files[i].FullName), true);
            }
            Finished?.Invoke("", files.Length + dirs.Length);
        }

        public int TotalFolders
        {
            get;set;
        }
        public int TotalFiles { get; set; }
    }
}
