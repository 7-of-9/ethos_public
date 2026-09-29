using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ethos_viewer
{
    static class Program
    {
        public static MainForm frmMain;
        public static DisplayForm frmViewer;
        public static bool ss = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // screensaver mode?
            if (args != null && args.Length > 0)
                if (args[0].ToLower() == "/s")
                    ss = true;
                else if (args[0].ToLower() == "/p")
                    { MessageBox.Show("preview not supported."); return; }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmMain = new MainForm();
            Application.Run(frmMain);
        }

        internal static bool IsVisibleOnAnyScreen(Point location, Size size)
        {
            Rectangle rect = new Rectangle(location, size);
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
