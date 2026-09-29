using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ethos_viewer.Properties;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace ethos_viewer
{
    public partial class FlowerPower : Form
    {
        //
        // TODO -- clickonce
        //  http://kazinadudvari.wordpress.com/2009/06/01/how-to-deploy-clickonce-applications-to-windows-azure/
        //

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HT_CAPTION = 0x0002;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();


        public FlowerPower()
        {
            InitializeComponent();

            this.DragEnter += new DragEventHandler(Flower_DragEnter);
            this.DragDrop += new DragEventHandler(Flower_DragDrop);
            pic.DragEnter += new DragEventHandler(pic_DragEnter);
            pic.DragDrop += new DragEventHandler(pic_DragDrop);

            this.Height = this.pic.Height;
            this.Width = this.pic.Width;
            this.pic.Dock = DockStyle.Fill;

            pic.MouseDown += new MouseEventHandler(pic_MouseDown);
            pic.MouseUp += new MouseEventHandler(pic_MouseUp);
            pic.MouseMove += new MouseEventHandler(pic_MouseMove);

            Bitmap bitmap = new Bitmap(this.pic.Image);
            Color color = bitmap.GetPixel(0, 0);
            this.TransparencyKey = color;
            //this.BackColor = color;
            bitmap.Dispose();
        }

        void pic_MouseDown(object sender, MouseEventArgs e)
        {
        }

        void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        void pic_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(Control.ModifierKeys == Keys.Control))
            {
                Program.frmViewer.Visible = !Program.frmViewer.Visible;
                Program.frmMain.Visible = Program.frmViewer.Visible;

                //if (Program.frmMain.Visible && Program.frmMain.WindowState == FormWindowState.Minimized)
                //    Program.frmMain.WindowState = FormWindowState.Normal;
            }
        }


        void pic_DragDrop(object sender, DragEventArgs e) { Program.frmMain.xDragDrop(e); }
        void pic_DragEnter(object sender, DragEventArgs e) { Program.frmMain.xDragEnter(e); }

        void Flower_DragDrop(object sender, DragEventArgs e) { Program.frmMain.xDragDrop(e); }
        void Flower_DragEnter(object sender, DragEventArgs e) { Program.frmMain.xDragEnter(e); }

        private void FlowerPower_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
                Program.frmMain.PasteClipboard();

            else if (e.KeyCode == Keys.Escape)
                Program.frmMain.Close();
        }

        private void pic_Click(object sender, EventArgs e)
        {
        }

        private void FlowerPower_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.flowerLocation = this.Location;

            //if (this.WindowState == FormWindowState.Normal)
            //    Settings.Default.flowerSize = this.Size;
            //else
            //    Settings.Default.flowerSize = this.RestoreBounds.Size;

            Settings.Default.Save();
        }

        private void FlowerPower_Load(object sender, EventArgs e)
        {
            if (Settings.Default.flowerLocation != null && Settings.Default.flowerSize != null)
            {
                if (Program.IsVisibleOnAnyScreen(Settings.Default.flowerLocation, Settings.Default.flowerSize))
                {
                    this.Location = Settings.Default.flowerLocation;
                    //this.Size = Settings.Default.flowerSize;
                }
            }
        }

        private void FlowerPower_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
