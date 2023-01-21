using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pkg__
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!File.Exists("settings.cfg"))
            {
                Settings.CreateDefault("settings.cfg");
            }
            Settings.Load("settings.cfg");
            Manager.LoadResources();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var frm = new Main();
            Application.Run(frm);

        }
    }
}
