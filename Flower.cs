using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ethos_viewer
{
    public partial class Flower : Form
    {
        public Flower()
        {
            InitializeComponent();

            this.DragEnter += new DragEventHandler(Flower_DragEnter);
            this.DragDrop += new DragEventHandler(Flower_DragDrop);
            pic.DragEnter += new DragEventHandler(pic_DragEnter);
            pic.DragDrop += new DragEventHandler(pic_DragDrop);

            this.Width = pic.Width;
            this.Height = pic.Height;

            pic.MouseDown += new MouseEventHandler(pic_MouseDown);
            pic.MouseUp += new MouseEventHandler(pic_MouseUp);
            pic.MouseMove += new MouseEventHandler(pic_MouseMove);
        }

        Point mouseDown;
        void pic_MouseMove(object sender, MouseEventArgs e)
        {
            //Point delta = new Point(e.Location.X - mouseDown.X, e.Location.Y - mouseDown.Y);
            if (e.Button == MouseButtons.Left)
            {
                this.Location = this.PointToScreen(e.Location);
            }
            //Debug.WriteLine(e.Location.ToString());
        }
        void pic_MouseUp(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine(e.Location.ToString());
        }
        void pic_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = e.Location;
            //Debug.WriteLine(e.Location.ToString());
        }

        void pic_DragDrop(object sender, DragEventArgs e) { Program.formMain.xDragDrop(e); }
        void pic_DragEnter(object sender, DragEventArgs e) { Program.formMain.xDragEnter(e); }

        void Flower_DragDrop(object sender, DragEventArgs e) { Program.formMain.xDragDrop(e); }
        void Flower_DragEnter(object sender, DragEventArgs e) { Program.formMain.xDragEnter(e); }
    }
}
