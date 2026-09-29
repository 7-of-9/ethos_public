using System;
using System.Timers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using ethos_viewer.Properties;
using System.Drawing.Imaging;
using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;

namespace ethos_viewer
{
    public partial class DisplayForm : Form
    {
        internal string tmpFile;
        bool fullScreen = false;
        internal ethos _e;
        MainForm _mainForm;

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int WM_NCMOUSELEAVE = 0x02A2;
        const int WM_NCMOUSEMOVE = 0x00A0;
        const int WM_MOUSEWHEEL = 0x020A;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_MOUSELEAVE = 0x02A3;
        const int HT_CAPTION = 0x2;
        private bool mouseInWindow = false;


        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        const int WM_NCHITTEST = 0x84;
        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTBOTTOMRIGHT = 17;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 16;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTCLIENT = 1;
        const int HTCAPTION = 2;

        [DllImport("user32.dll")]
        static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENT
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr hwndTrack;
            public uint dwHoverTime;
        }

        public const uint TME_LEAVE = 0x00000002;
        public const uint TME_HOVER = 0x00000001;
        public const uint TME_NONCLIENT = 0x00000010;
        public const uint HOVER_DEFAULT = 0xFFFFFFFF;

        public DisplayForm(MainForm mainForm)
        {
            InitializeComponent();
            this._mainForm = mainForm;
            this.FormClosed += new FormClosedEventHandler(Display_FormClosed);

            timer1.Enabled = false;

            this.DragEnter += new DragEventHandler(Viewer_DragEnter);
            this.DragDrop += new DragEventHandler(Viewer_DragDrop);
            pic2.DragEnter += new DragEventHandler(Viewer_DragEnter);
            pic2.DragDrop += new DragEventHandler(Viewer_DragDrop);

            this.Padding = new Padding(5);

            //this.rtbAltStats.BackColor = Color.Black;
            //this.rtbAltStats.SelectionChanged += rtbAltStats_SelectionChanged;

            // fake transparency
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
        }

        // manual resizing
        private bool handlingWheel = false;
        //private Rectangle clientRectScreen;
        //private Rectangle expandedClientRect;
        private bool isMouseOutsideWindow = false;
        private System.Timers.Timer mouseLeaveTimer;
        
        private void OnMouseLeaveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Handle the event when the timer elapses
            Debug.WriteLine("Mouse left the window");
            isMouseOutsideWindow = true;
            mouseLeaveTimer?.Stop();
            mouseLeaveTimer?.Dispose();
            mouseLeaveTimer = null;

            mouseInWindow = false;

            this.Invoke((MethodInvoker)delegate{
                HideFrame();
                ShowHideControls(false);
            });
        }

        void StartMouseLeaveTimer()
        {
            mouseLeaveTimer?.Stop();  // Stop the timer if it's already running
            mouseLeaveTimer = new System.Timers.Timer(1000);
            mouseLeaveTimer.Elapsed += OnMouseLeaveTimerElapsed;
            mouseLeaveTimer.AutoReset = false;
            mouseLeaveTimer.Start();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
            {
                // resizing
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft(cursor)) m.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight(cursor)) m.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft(cursor)) m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight(cursor)) m.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top(cursor)) m.Result = (IntPtr)HTTOP;
                else if (Left(cursor)) m.Result = (IntPtr)HTLEFT;
                else if (Right(cursor)) m.Result = (IntPtr)HTRIGHT;
                else if (Bottom(cursor)) m.Result = (IntPtr)HTBOTTOM;

                else m.Result = (IntPtr)HTCAPTION;

                // expanded client rect tracking
                //clientRectScreen = RectangleToScreen(ClientRectangle); // Store the client rectangle in screen coordinates
            }

            if (m.Msg == WM_NCMOUSEMOVE)
            {
                //if (!mouseInWindow)
                {
                    Debug.WriteLine("WM_NCMOUSEMOVE");
                    mouseLeaveTimer?.Stop();

                    mouseInWindow = true;
                    ShowFrame();
                    ShowHideControls(true);
                }
            }

            //if (m.Msg == WM_MOUSEMOVE)
            //{
            //    Debug.WriteLine("WM_MOUSEMOVE");
            //    // Check if the mouse position is outside the expanded client rectangle
            //    Point mousePos = PointToScreen(new Point(m.LParam.ToInt32() & 0xFFFF, m.LParam.ToInt32() >> 16));
            //    bool isMouseOutsideExpandedRect = !expandedClientRect.Contains(mousePos);
            //    // Only update the flag if the mouse position has changed
            //    if (isMouseOutsideExpandedRect != isMouseOutsideRect)
            //    {
            //        isMouseOutsideRect = isMouseOutsideExpandedRect;
            //        // Handle the event only when the mouse is outside the expanded client rectangle
            //        if (isMouseOutsideRect)
            //        {
            //            Debug.WriteLine("WM_NCMOUSELEAVE");
            //            mouseInWindow = false;
            //            HideFrame();
            //            ShowHideControls(false);
            //        }
            //    }
            //}

            if (m.Msg == WM_NCMOUSELEAVE)
            {
                Debug.WriteLine("WM_NCMOUSELEAVE");
                StartMouseLeaveTimer();
                //if (isMouseOutsideRect)
                //{
                //    Debug.WriteLine("WM_NCMOUSELEAVE 2");
                //    mouseInWindow = false;
                //    HideFrame();
                //    ShowHideControls(false);
                //}
            }

            //if (m.Msg == WM_MOUSELEAVE)
            //{
            //    Debug.WriteLine("WM_MOUSELEAVE");
            //    StartMouseLeaveTimer();
            //}
         
            if (m.Msg == WM_MOUSEWHEEL)
            {
                HideFrame();
                ShowHideControls(false);

                if (handlingWheel) return;

                int wheelDelta = (short)((m.WParam.ToInt64() >> 16) & 0xffff);

                // Determine the scale factor based on the wheel rotation.
                const float SPEED = 0.2f;
                float scaleFactor = wheelDelta > 0 ? 1.0f + SPEED : 1.0f - SPEED;
                //Debug.WriteLine("WM_MOUSEWHEEL, wheelDelta=" + wheelDelta);

                // Calculate the target size while keeping the aspect ratio.
                int targetWidth = (int)(this.Width * scaleFactor);
                int targetHeight = (int)(this.Height * scaleFactor);

                // Determine the amount to change the size by at each step.
                const int STEPS = 20;
                int stepWidth = (targetWidth - this.Width) / STEPS;
                int stepHeight = (targetHeight - this.Height) / STEPS;

                // Gradually change the form size.
                // TODO: GPT4 for
                handlingWheel = true;
                new Thread(() => {
                    for (int i = 0; i < STEPS; i++) {
                        // Calculate the progress as a value between 0 and 1
                        float progress = (float)i / STEPS;

                        // Use a quadratic easing function for the progress
                        double easedProgress = (progress < 0.5) ? 2 * progress * progress : 1 - Math.Pow(-2 * progress + 2, 2) / 2;

                        // Calculate the current width and height based on the eased progress
                        int currentWidth = this.Width + (int)((targetWidth - this.Width) * easedProgress);
                        int currentHeight = this.Height + (int)((targetHeight - this.Height) * easedProgress);

                        // Update the form size
                        this.Invoke((MethodInvoker)delegate {
                            this.Width = currentWidth;
                            this.Height = currentHeight;

                            // Allow the form to repaint itself at each step.
                            //this.Refresh();
                        });

                        // Wait a small amount of time before the next step.
                        Thread.Sleep(2);
                    }

                    // Set the form size to the target size, in case it didn't quite get there due to rounding errors.
                    //this.Width = targetWidth;
                    //this.Height = targetHeight;
                    handlingWheel = false;
                })
                { IsBackground = true }.Start();

                //int wheelDelta = (short)((m.WParam.ToInt64() >> 16) & 0xffff);
                //Point mouseLocation = this.PointToClient(new Point(m.LParam.ToInt32() & 0xFFFF, m.LParam.ToInt32() >> 16));

                //// Transform mouse location to screen coordinates
                //mouseLocation = this.PointToScreen(mouseLocation);

                //// Determine the scale factor based on the wheel rotation.
                //float scaleFactor = wheelDelta > 0 ? 1.1f : 0.9f;

                //// Calculate the new size while keeping the aspect ratio.
                //int newWidth = (int)(this.Width * scaleFactor);
                //int newHeight = (int)(this.Height * scaleFactor);

                //// Scale the form and keep it centered on the mouse location.
                //this.Bounds = new Rectangle(
                //    mouseLocation.X - (newWidth / 2),
                //    mouseLocation.Y - (newHeight / 2),
                //    newWidth,
                //    newHeight);
            }
        }
        public bool TopLeft(Point p) { return p.X <= this.Padding.Left && p.Y <= this.Padding.Top; }
        public bool TopRight(Point p) { return p.X >= this.ClientSize.Width - this.Padding.Right && p.Y <= this.Padding.Top; }
        public bool BottomLeft(Point p) { return p.X <= this.Padding.Left && p.Y >= this.ClientSize.Height - this.Padding.Bottom; }
        public bool BottomRight(Point p) { return p.X >= this.ClientSize.Width - this.Padding.Right && p.Y >= this.ClientSize.Height - this.Padding.Bottom; }
        public bool Top(Point p) { return p.Y <= this.Padding.Top; }
        public bool Left(Point p) { return p.X <= this.Padding.Left; }
        public bool Right(Point p) { return p.X >= this.ClientSize.Width - this.Padding.Right; }
        public bool Bottom(Point p) { return p.Y >= this.ClientSize.Height - this.Padding.Bottom; }

        void rtbAltStats_SelectionChanged(object sender, EventArgs e)
        {
            //rtbAltStats.Select(0, 0);
        }

        void Viewer_DragEnter(object sender, DragEventArgs e) { Program.frmMain.xDragEnter(e); }
        void Viewer_DragDrop(object sender, DragEventArgs e) { Program.frmMain.xDragDrop(e); }

        protected override void OnLoad(EventArgs e)
        {
            if (Settings.Default.viewerLocation != null && Settings.Default.viewerSize != null)
            {
                if (Program.IsVisibleOnAnyScreen(Settings.Default.viewerLocation, Settings.Default.viewerSize))
                {
                    this.Location = Settings.Default.viewerLocation;
                    this.Size = Settings.Default.viewerSize;
                }
            }

            if (Program.ss)
            {
                foreach (var screen in Screen.AllScreens)
                {
                    if (!screen.Primary)
                    {
                        this.Location = new Point(screen.Bounds.X, screen.Bounds.Y);
                    }
                }
                Cursor.Hide();
                ToggleFullScreen();
            }

            this.ShowHideControls(false);
        }

        private void Display_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.viewerLocation = this.Location;

            if (this.WindowState == FormWindowState.Normal)
                Settings.Default.viewerSize = this.Size;
            else
                Settings.Default.viewerSize = this.RestoreBounds.Size;

            Settings.Default.Save();
        }

        void Display_FormClosed(object sender, FormClosedEventArgs e)
        {
            CleanTmpFile();
        }

        void CleanTmpFile()
        {
            if (this.pic2.Image != null)
            {
                this.pic2.Image.Dispose();
                this.pic2.Image = null;
            }   
            if (tmpFile != null)
            {
                File.Delete(tmpFile);  
                tmpFile = null;
            }
        }
        //private void AddText(string text)
        //{
        //    string[] str = text.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

        //    if (str.Length == 2)
        //    {
        //        this.rtbAltStats.DeselectAll();
        //        this.rtbAltStats.SelectionFont = new Font(this.rtbAltStats.SelectionFont, FontStyle.Bold);
        //        this.rtbAltStats.AppendText(Environment.NewLine + str[0] + ":");
        //        this.rtbAltStats.SelectionFont = new Font(this.rtbAltStats.SelectionFont, FontStyle.Regular);
        //        this.rtbAltStats.AppendText(str[1]);
        //    }
        //    else
        //        this.rtbAltStats.AppendText(text);
        //}

        internal void ShowEthos(ethos e)
        {
            this._e = e;

            {
                //this.rtbAltStats.Visible = false;
                if (e.fastGotPic && e.pic == null) // lazy load pics
                {
                    try
                    {
                        using (ethosDataContext db = new ethosDataContext(MainForm.conStr))
                        {
                            var picBinary = (from x in db.ethos where x.id == e.id select x.pic).FirstOrDefault();
                            byte[] bytes = picBinary.ToArray();
                            e.pic = MainForm.ByteArrayToImage(bytes);
                            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                            {
                                e.picHash = Convert.ToBase64String(sha1.ComputeHash(bytes));
                            }
                        }
                    }
                    catch (Exception ex) // handle network crapness
                    {
                        ;
                    }
                }

                if (e.pic != null)
                {
                    this.lbl.Visible = false;
                    this.lblAttrib.Visible = false;

                    // clone image to file, then load from it (to allow animated gif to work)
                    CleanTmpFile();
                    tmpFile = System.IO.Path.GetTempFileName();
                    e.pic.Save(tmpFile);

                    this.pic2.Enabled = true;
                    this.pic2.Visible = true;
                    this.pic2.Image = Image.FromFile(tmpFile);

                    ScalePosPic();

                    timer1.Enabled = false;
                }
                else
                {
                    this.pic2.Visible = false;
                    this.pic2.Enabled = false;

                    this.lbl.Text = e.quote;
                    this.lbl.Visible = true;
                    this.lbl.Width = this.Width / 2;
                    this.lbl.Height = this.Height / 2;
                    this.lbl.AutoSize = true;

                    this.lblAttrib.Text = e.attrib;
                    this.lblAttrib.Visible = false;
                    this.lblAttrib.AutoSize = true;

                    mIncrement = 0;
                    timer1.Enabled = true;
                }
            }

            this.cmdNSFW.Text = e.nsfw ? "sfw" : "NSFW!";
            this.cmdPause.Text = this._mainForm.timer1.Enabled ? "||" : ">";
        }

        //private void SetSpecialTextSize()
        //{
        //    float f = Math.Min((float)Program.frmViewer.Width / 1024.0f, 1.0f);
        //    float z = 4.0f * f;
        //    if (z >= 1)
        //        this.rtbAltStats.ZoomFactor = z;
        //}

        void ScalePosPic()
        {
            if (this.pic2.Image != null)
            {
                float aspect = (float)this.pic2.Image.Width / (float)this.pic2.Image.Height;
                //if (pic2.Image.Width > pic2.Image.Height)
                {
                    pic2.Width = this.ClientSize.Width;
                    pic2.Height = (int)((float)pic2.Width / aspect);
                    if (pic2.Height > this.ClientSize.Height)
                    {
                        pic2.Height = this.ClientSize.Height;
                        pic2.Width = (int)((float)pic2.Height * aspect);
                    }
                    pic2.Left = this.ClientSize.Width / 2 - pic2.Width / 2;
                    pic2.Top = this.ClientSize.Height / 2 - pic2.Height / 2;
                }
            }
        }

        float mIncrement;
        private void adjustFont()
        {
            float size = 10 * (1 + mIncrement);// / 7f);
            this.lbl.Font = new Font(this.lbl.Font.FontFamily, size);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            mIncrement += 0.1F;
            adjustFont();

            //lbl.Refresh();
            //Application.DoEvents();

            lbl.Left = this.ClientSize.Width / 2 - lbl.Width / 2;
            lbl.Top = this.ClientSize.Height / 2 - lbl.Height / 2 - this.ClientSize.Height / 15;

            if (lbl.Width >= this.ClientSize.Width * 0.8 || lbl.Height >= this.ClientSize.Height * 0.8)
            {
                timer1.Enabled = false;
                lblAttrib.Font = new Font(this.lblAttrib.Font.FontFamily, lbl.Font.Size, FontStyle.Italic | FontStyle.Bold);
                lblAttrib.Visible = true;
                lblAttrib.Left = this.ClientSize.Width / 2 - lblAttrib.Width / 2;
                lblAttrib.Top = lbl.Top + lbl.Height + lblAttrib.Height / 2;
            }
        }

        private void Display_ResizeEnd(object sender, EventArgs e)
        {
            //this.lbl.MaximumSize = new Size((this.ClientSize.Width / 10) * 7, (this.ClientSize.Height / 10) * 9);
            if (lbl.Visible && !timer1.Enabled)
            {
                mIncrement = 0;
                timer1.Enabled = true;
                lblAttrib.Visible = false;
            }

            if (pic2.Visible)
                ScalePosPic();

            //SetSpecialTextSize();
        }

       

        private void Display_KeyDown(object sender, KeyEventArgs e) { HandleKeyDown(e); }
        private void Display_KeyUp(object sender, KeyEventArgs e) { HandleKeyUp(e); }

        void HandleKeyDown(KeyEventArgs e)
        {
            Debug.WriteLine("HandleKeyDown");
            if (Program.ss)
                Program.frmMain.Close();

            if (e.KeyCode == Keys.Enter && e.Alt || (e.KeyCode == Keys.Escape && fullScreen))
                ToggleFullScreen();
        }
        void HandleKeyUp(KeyEventArgs e)
        {
            Debug.WriteLine("HandleKeyUp");

            if (Program.ss)
                Program.frmMain.Close();

            if (e.KeyCode == Keys.Left)
                this._mainForm.Previous();
            else if (e.KeyCode == Keys.Right)
                this._mainForm.Next();

            else if (e.KeyCode == Keys.R) {
                Program.frmMain.rnd = new Random();
                MessageBox.Show("created new random.");
            }

            else if (e.KeyCode == Keys.Escape && !fullScreen)
                Program.frmMain.Close();

            else if (e.Control && e.KeyCode == Keys.V) {
                this.Cursor = Cursors.WaitCursor;
                _mainForm.PasteClipboard();
                this.Cursor = Cursors.Default;
            }

            else if (e.Control && e.KeyCode == Keys.C)
                CopyCurrentImageToClipboard();

            else if (e.KeyCode == Keys.W)
                _mainForm.SetWallpaper();

            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Delete)
                _mainForm.cmdDelete_Click_1(null, null);

            else if (e.KeyCode == Keys.S)
                SaveCurrent();

            //else if (e.KeyCode == Keys.S)
            //    ShowScreenSaver(this);
        }

        private void CopyCurrentImageToClipboard() {
            if (pic2.Image == null) {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            try {
                Clipboard.SetImage(pic2.Image);
            }
            catch (ExternalException) {
                MessageBox.Show("Could not copy image to clipboard. Another process may be locking the clipboard.", "Clipboard Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCurrent()
        {
            if (_e.pic != null)
            {
                if (!Directory.Exists("./EthosPicSingles"))
                    Directory.CreateDirectory("./EthosPicSingles");
                string fileName = "./EthosPicSingles/" + _e.pic.GetHashCode().ToString() + ".gif";
                if (!File.Exists(fileName))
                {
                    _e.pic.Save(fileName, ImageFormat.Gif);
                    MessageBox.Show("saved to " + fileName);
                }
            }
        }

        //private void ShowScreenSaver(Control displayControl)
        //{
        //    using (RegistryKey desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop"))
        //    {
        //        if (desktopKey != null)
        //        {
        //            string screenSaverExe = @"C:\Windows\System32\scrnsave.scr"; //desktopKey.GetValue("SCRNSAVE.EXE") as string;
        //            if (!string.IsNullOrEmpty(screenSaverExe))
        //            {
        //                Process p = Process.Start(screenSaverExe, "/P " + displayControl.Handle);
        //                p.WaitForInputIdle();
        //                IntPtr hwnd = p.MainWindowHandle;
        //                if (hwnd != IntPtr.Zero)
        //                {
        //                    SetParent(hwnd, displayControl.Handle);
        //                    Rectangle r = displayControl.ClientRectangle;
        //                    MoveWindow(hwnd, r.Left, r.Top, r.Width, r.Height, true);
        //                }
        //            }
        //        }
        //    }
        //}

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hwndChild, IntPtr hwndParent);

        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint);

        void ToggleFullScreen()
        {
            if (!fullScreen)
            {
                fullScreen = true;
                //this.MaximizeBox = false;
                //this.MinimizeBox = false;
                //this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.BackColor = Color.Black;
                this.TransparencyKey = Color.Transparent; // no transparency
                ShowHideControls(false);
            }
            else
            {
                fullScreen = false;
                //this.MaximizeBox = true;
                //this.MinimizeBox = true;
                //this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                this.BackColor = Color.DimGray;
                this.TransparencyKey = Color.Black;
            }
        }

        private void lbl_DoubleClick(object sender, EventArgs e) { HandleDblClick(); }
        private void pic2_DoubleClick(object sender, EventArgs e) { HandleDblClick(); }
        private void lblAttrib_DoubleClick(object sender, EventArgs e) { HandleDblClick(); }
        private void Display_DoubleClick(object sender, EventArgs e) { HandleDblClick(); }
        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e) { /*HandleDblClick();*/ }
        private void panel1_DoubleClick(object sender, EventArgs e) { HandleDblClick(); }
        private void HandleDblClick() {
            //Debug.WriteLine("HandleDblClick");
            if (Program.ss)
                Program.frmMain.Close();
            ToggleFullScreen();
        }

        private void Display_Resize(object sender, EventArgs e) { Display_ResizeEnd(null, null); }

        private void cmdNSFW_MouseEnter(object sender, EventArgs e) { } //ShowHideControls(true); }
        
        private void Display_MouseEnter(object sender, EventArgs e) {
            Debug.WriteLine("Display_MouseEnter");
            ShowHideControls(true);
        }
        private void Display_MouseLeave(object sender, EventArgs e) {
            Debug.WriteLine("Display_MouseLeave");
            //ShowHideControls(false);
        }
        private void Display_MouseMove(object sender, MouseEventArgs e) {
            Debug.WriteLine("Display_MouseMove");
            ShowHideControls(true);

        }
        private void panel1_MouseEnter(object sender, EventArgs e) { 
            Debug.WriteLine("panel1_MouseEnter");
            ShowHideControls(true);
            ShowFrame();
        }
        private void panel1_MouseLeave(object sender, EventArgs e) {
            Debug.WriteLine("panel1_MouseLeave");
            HideFrame();
        }
        private void HideFrame() { this.BackColor = Color.Black; }
        private void ShowFrame() { 
            if (!this.fullScreen)
                this.BackColor = Color.DimGray;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e) { ShowHideControls(true); }

        private void Display_MouseDown(object sender, MouseEventArgs e) { HandleMove_Timer(e); }
        private void pic2_MouseDown(object sender, MouseEventArgs e) { HandleMove_Timer(e); }
        private void panel1_MouseDown(object sender, MouseEventArgs e) { HandleMove_Timer(e); }
        private void HandleMove_Timer(MouseEventArgs e) {
            if (e.Clicks < 2) // If it's not a double click
            {
                // Delay the start of the drag operation by 200 milliseconds
                // This gives the DoubleClick event a chance to fire
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 10; // SystemInformation.DoubleClickTime;
                timer.Tick += (s, e2) => {
                    timer.Stop();
                    timer.Dispose();
                    HandleMove_Action(e);
                };
                timer.Start();
            }
        }
        private void HandleMove_Action(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pic2_MouseEnter(object sender, EventArgs e) {
            Debug.WriteLine("pic2_MouseEnter");
            ShowFrame();
            ShowHideControls(true);
        }
        private void pic2_MouseLeave(object sender, EventArgs e) {
            Debug.WriteLine("pic2_MouseLeave");
            //StartMouseLeaveTimer();
            //HideFrame();
            //ShowHideControls(false); }
        }
        private Point previousMouseLocation;
        private void pic2_MouseMove(object sender, MouseEventArgs e) {
            Debug.WriteLine("pic2_MouseMove");

            if (e.Location != previousMouseLocation) {
                previousMouseLocation = e.Location;
                ShowHideControls(true);
            }

            // somehow makes WM_NCMOUSELEAVE fire more consistently (when moving mouse fast out of the window into another)
            TRACKMOUSEEVENT tme = new TRACKMOUSEEVENT();
            tme.cbSize = (uint)Marshal.SizeOf(tme);
            tme.dwFlags = TME_LEAVE | TME_HOVER | TME_NONCLIENT;
            tme.hwndTrack = this.Handle;
            tme.dwHoverTime = HOVER_DEFAULT;
            TrackMouseEvent(ref tme);
        }

        //private void rtbAltStats_MouseEnter(object sender, EventArgs e) { ShowHideControls(true); }
        //private void rtbAltStats_MouseLeave(object sender, EventArgs e) { } //ShowHideControls(false);
        //private void rtbAltStats_MouseMove(object sender, MouseEventArgs e) {
        //    ShowHideControls(true);
        //}

        private void pic2_Click(object sender, EventArgs e)
        {
            if (Program.ss)
                Program.frmMain.Close();
            ShowHideControls(true);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("timer2_Tick");
            ShowHideControls(false);
            this.timer2.Enabled = false;
        }

        private void ShowHideControls(bool b)
        {
            //StackTrace stackTrace = new StackTrace();
            //string callStack = stackTrace.ToString();
            //Debug.WriteLine("ShowHideControls: " + b + "\n" + callStack);

            if (this.fullScreen)
                b = false;

            if (b)
            {
                Point pc = Cursor.Position;
                Point p = pc;
                p = this.PointToClient(pc);
                //if (p.Y > this.cmdNext.Height + cmdNext.Top)
                //    return;
            }

            this.cmdNSFW.Visible = this.cmdNSFW.Enabled =
            this.cmdPrev.Visible = this.cmdPrev.Enabled = 
            this.cmdNext.Visible = this.cmdNext.Enabled = 
            this.cmdPause.Visible = this.cmdPause.Enabled 
                = b;

            if (b && !this.timer2.Enabled)
            {
                //Debug.WriteLine("ShowHideControls - start timer");
                this.timer2.Enabled = true;
            }
        }

        private void cmdNSFW_Click(object sender, EventArgs e)
        {
            this._mainForm.chkNSFW.Checked = !this._mainForm.chkNSFW.Checked;
            this._mainForm.cmdSave_Click(null, null);
            //this._mainForm.lvw_SelectedIndexChanged(null, null);
        }

        private void cmdFrames_Click(object sender, EventArgs e)
        {
        }

        private void cmdPause_Click(object sender, EventArgs e)
        {
            this._mainForm.TogglePause();
        }

        private void cmdPrev_Click(object sender, EventArgs e)
        {
            this._mainForm.Previous();
        }

        private void cmdNext_Click(object sender, EventArgs e)
        {
            this._mainForm.Next();
        }

        private void Display_Load(object sender, EventArgs e)
        {
        }

    }        
}