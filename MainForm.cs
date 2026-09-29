using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Configuration;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Threading;
using ethos_viewer.Properties;
using System.Runtime.InteropServices;
using Manina.Windows.Forms;

// http://www.codeproject.com/Articles/43265/ImageListView

namespace ethos_viewer
{
    public partial class MainForm : Form
    {
        public List<ethos> culture;
        public DisplayForm display;
        internal static string conStr = global::ethos_viewer.Properties.Settings.Default.ethosConnectionString;
        internal static string caption = "§ v1.2";
        internal Random rnd = new Random();
        
        FlowerPower flower = new FlowerPower();
        PicForm picForm = new PicForm();
        const string picsDumpDir = "./EthosPicDump";
        const string nsfwDumpDir = "./NSFW";

        public MainForm()
        {
            InitializeComponent();

            this.display = new DisplayForm(this);
            Program.frmViewer = this.display;

            LoadAll(true);
            LoadAllDisk();

            if (!Directory.Exists(picsDumpDir))
                Directory.CreateDirectory(picsDumpDir);
            if (!Directory.Exists(nsfwDumpDir))
                Directory.CreateDirectory(nsfwDumpDir);

            this.Text = caption;
            this.picForm.Visible = false;

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            
            lvw.DragDrop += new DragEventHandler(lvw_DragDrop);
            lvw.DragEnter += new DragEventHandler(lvw_DragEnter);
            
            display.Show();
            flower.Show();

            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                MainForm.caption += " {" + db.Connection.DataSource + "}";
            }

            timer1.Interval = rnd.Next(60, 90) * 1000;
            timer1_Tick(null, null);
        }

        void LoadAllDisk() {
            ilvw.Items.Clear();
            var dt1 = LoadFromDisk(picsDumpDir);
            var dt2 = LoadFromDisk(nsfwDumpDir);
            tabPage2.Text = "disk (" + dt1.ToString("dd MMM yyyy") + ")";
        }

        DateTime LoadFromDisk(string path) {
            var earliest = DateTime.MaxValue;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var files = Directory.GetFiles(path);
            foreach (var file in files) {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension == ".gif") {
                    var item = new ImageListViewItem(file);
                    item.Tag = Convert.ToInt32(Path.GetFileNameWithoutExtension(fi.Name));
                    ilvw.Items.Add(item);
                    if (fi.CreationTime < earliest)
                        earliest = fi.CreationTime;
                }
            }
            return earliest;
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            display.Close();
            flower.Close();
        }

        void lvw_DragEnter(object sender, DragEventArgs e) { xDragEnter(e); }
        void Form1_DragEnter(object sender, DragEventArgs e) { xDragEnter(e); }
        public void xDragEnter(DragEventArgs e)
        {
            string[] ss = e.Data.GetFormats();
            foreach (string s in ss)
                Debug.WriteLine(s);
            
            if (e.Data.GetDataPresent("FileDrop", true) ||
                e.Data.GetDataPresent("System.String", true) ||
                e.Data.GetDataPresent("msSourceUrl", true) ||
                e.Data.GetDataPresent("text/html", true))
            {
                e.Effect = e.AllowedEffect;//DragDropEffects.Copy;
            }
        }

        void Form1_DragDrop(object sender, DragEventArgs e) { xDragDrop(e); }
        void lvw_DragDrop(object sender, DragEventArgs e) { xDragDrop(e); }
        public void xDragDrop(DragEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string[] ss = e.Data.GetFormats();
                foreach (string s in ss)
                    Debug.WriteLine(s);

                //if (e.Data.GetDataPresent("UniformResourceLocator", true))
                //{
                //    object o = e.Data.GetData("UniformResourceLocator");
                //    MemoryStream ms = (MemoryStream)o;
                //    byte[] buffer = new byte[ms.Length];
                //    ms.Read(buffer, 0, (int)ms.Length);
                //    string uri;
                //    if (buffer[1] == (byte)0)  // Detecting unicode
                //    {
                //        uri = System.Text.Encoding.Unicode.GetString(buffer);
                //    }
                //    else
                //    {
                //        uri = System.Text.Encoding.ASCII.GetString(buffer);
                //    }
                //}

                if (e.Data.GetDataPresent("text/html", true))
                {
                    var obj = e.Data.GetData("text/html");
                    string html = string.Empty;
                    if (obj is string)
                    {
                        html = (string)obj;
                    }
                    else if (obj is MemoryStream)
                    {
                        MemoryStream ms = (MemoryStream)obj;
                        byte[] buffer = new byte[ms.Length];
                        ms.Read(buffer, 0, (int)ms.Length);
                        if (buffer[1] == (byte)0)  // Detecting unicode
                        {
                            html = System.Text.Encoding.Unicode.GetString(buffer);
                        }
                        else
                        {
                            html = System.Text.Encoding.ASCII.GetString(buffer);
                        }
                    }
                    // Using a regex to parse HTML, but JUST FOR THIS EXAMPLE :-)
                    var match = new Regex(@"<img[^/]src=""([^""]*)""").Match(html);
                    if (match.Success)
                    {
                        Uri uri = new Uri(match.Groups[1].Value);
                        Image img = GetImageFromUri(uri);
                        if (img != null)
                        {
                            PopulateCulture(AddEthos(new ethos { quote = null, pic = img, url = html }));
                            return;
                        }
                    }
                }

                // drag image from browser (IE 8)
                if (e.Data.GetDataPresent("FileDrop", true))
                {
                    string filename = ((string[])e.Data.GetData("FileDrop"))[0];
                    //MessageBox.Show("FileDrop: " + filename);
                    Image img = Image.FromFile(filename);
                    if (img != null)
                    {
                        PopulateCulture(AddEthos(new ethos { quote = null, pic = img, url = filename }));
                        return;
                    }
                }

                // drag of text
                if (e.Data.GetDataPresent("System.String", true))
                {
                    string text = e.Data.GetData("System.String", true).ToString();
                    if (text.StartsWith("data:image/jpg;base64"))
                    {
                        ; //TODO
                    }
                    else
                    {
                        if (text.ToLower().StartsWith("http") &&
                           (text.ToLower().EndsWith(".gif") ||
                            text.ToLower().EndsWith(".png") ||
                            text.ToLower().EndsWith(".jpg") ||
                            text.ToLower().EndsWith(".jpeg")))
                        {
                            Image img = GetImageFromUri(new Uri(text));
                            if (img != null)
                            {
                                PopulateCulture(AddEthos(new ethos { quote = null, pic = img, url = text }));
                                return;
                            }
                        }

                        if (!text.StartsWith("http://www.pinterest.com/pin/"))
                        {
                            PopulateCulture(AddEthos(new ethos { quote = text, pic = null }));
                        }
                        return;
                    }
                }

                //if (e.Data.GetDataPresent(DataFormats.Bitmap))
                //{
                //    Bitmap bmp = (Bitmap)e.Data.GetData(DataFormats.Bitmap, true);
                //    MessageBox.Show("DataFormats.Bitmap: " + bmp.ToString());
                //}
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        void LoadAll(bool loadFast, bool saveAll = false)
        {
            culture = new List<ethos>();
            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                db.CommandTimeout = 180;
                if (loadFast)
                {
                    var ethosWithoutPics = from x in db.ethos
                                           orderby x.special descending
                                           select new
                                           {
                                               id = x.id,
                                               addedBy = x.addedBy,
                                               nsfw = x.nsfw ?? false,
                                               addedOn = x.addedOn,
                                               attrib = x.attrib,
                                               fastGotPic = (x.pic != null ? true : false), // perf: don't bring back images yet, just a flag
                                               quote = x.quote,
                                               rank = x.rank,
                                               url = x.url,
                                               special = x.special,
                                               hash = x.hash,
                                           };
                    Debug.WriteLine(ethosWithoutPics.ToString());
                    foreach (var et in ethosWithoutPics)
                    {
                        ethos e = new ethos();
                        e.id = et.id;
                        e.addedBy = et.addedBy;
                        e.addedOn = et.addedOn;
                        e.attrib = et.attrib;
                        e.fastGotPic = et.fastGotPic;
                        e.quote = et.quote;
                        e.rank = et.rank;
                        e.url = et.url;
                        e.nsfw = et.nsfw;
                        e.special = et.special;
                        e.picHash = et.hash;
                        culture.Add(e);
                    }
                }
                else
                {
                    int ndx = 0;
                    int count = db.ethos.Count();
                    db.CommandTimeout = 180; 
                    
                    //List<etho> loadedEthos = db.ethos.ToList();
                    //foreach(etho et in loadedEthos)
                    foreach (etho et in db.ethos)
                    {
                        this.Text = MainForm.caption + " LoadAll " + (((float)ndx++ / (float)count) * 100).ToString(",.") + "%";
                        Application.DoEvents();
                        ethos e = FromDbo(et);
                        if (e.picHash != null && et.hash == null)
                        {
                            et.hash = e.picHash;
                            db.SubmitChanges();
                        }
                        culture.Add(e);

                        if (saveAll)
                        {
                            if (e.pic != null)
                            {
                                string fileName;
                                if (e.nsfw)
                                    fileName = nsfwDumpDir + "/" + e.id + ".gif";
                                else
                                    fileName = picsDumpDir + "/" + e.id + ".gif";
                                if (!File.Exists(fileName))
                                    e.pic.Save(fileName, ImageFormat.Gif);
                            }
                        }

                        // release image memory even after slow "full" load;
                        // impossible to retain all in memory (too many), but note that image hash has been created
                        if (e.pic != null)
                        {
                            e.fastGotPic = true;
                            e.pic.Dispose();
                            e.pic = null;
                        }
                    }

                    MessageBox.Show("loaded and got image hashes for " + db.ethos.Count() + " ethos entries.");
                }
                 
                PopulateCulture(-1);
            }
        }

        public int AddEthos(ethos e)
        {
            // dedpue by URL
            if (!string.IsNullOrEmpty(e.url) && culture.Where(p => p.url == e.url).Count() > 0)
            {
                var alreadyPresent = culture.Where(p => p.url == e.url).FirstOrDefault();
                if (alreadyPresent != null)
                {
                    MessageBox.Show("URL already present; skipping add!");
                    return alreadyPresent.id;
                }
            }

            // dedupe by hash
            if (e.pic != null)// && e.picHash == null)
            {
                byte[] bytes = ImageToByteArray(e.pic);
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    e.picHash = Convert.ToBase64String(sha1.ComputeHash(bytes));
                }
                var alreadyPresent = culture.Where(p => p.picHash == e.picHash).FirstOrDefault();
                if (alreadyPresent != null)
                {
                    MessageBox.Show("hash already present; skipping add!");
                    return alreadyPresent.id;
                }
            }

            culture.Add(e);
            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                etho et = ToDbo(e);
                e.picHash = et.hash;
                db.ethos.InsertOnSubmit(et);
                db.SubmitChanges();
                e.id = et.id;
                return e.id;
            }
            //this.WindowState = FormWindowState.Normal;
            //this.Show();
        }

        etho ToDbo(ethos e)
        {
            etho et = new etho();
            et.addedBy = e.addedBy;
            et.addedOn = e.addedOn;
            et.quote = e.quote;
            et.rank = e.rank;
            et.attrib = e.attrib;
            et.url = e.url;
            if (e.pic != null)
            {
                byte[] bytes = ImageToByteArray(e.pic);
                et.pic = bytes;

                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    et.hash = Convert.ToBase64String(sha1.ComputeHash(bytes));
                }
            }
            return et;
        }

        ethos FromDbo(etho et)
        {
            int retry = 0;
again:
            try
            {
                ethos e = new ethos();
                e.addedBy = et.addedBy;
                e.addedOn = et.addedOn;
                if (et.pic != null)
                {
                    byte[] bytes = et.pic.ToArray();
                    e.pic = ByteArrayToImage(bytes);
                    using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                    {
                        e.picHash = Convert.ToBase64String(sha1.ComputeHash(bytes));
                    }
                }
                e.quote = et.quote;
                e.rank = et.rank;
                e.attrib = et.attrib;
                e.id = et.id;
                e.url = et.url;
                e.nsfw = et.nsfw ?? false;
                return e;
            }
            catch (Exception ex)
            {
                if (++retry == 3)
                    throw new ApplicationException("rety exceeded: " + ex.Message, ex);
                Thread.Sleep(10 * 1000);
                goto again;
            }
        }

        internal static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            //using (
            MemoryStream ms = new MemoryStream(); //)
            {
                FrameDimension fd = new FrameDimension(imageIn.FrameDimensionsList[0]);
                int frames = imageIn.GetFrameCount(fd);

                if (frames > 1)
                    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                else
                    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        internal static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            //using (
            MemoryStream ms = new MemoryStream(byteArrayIn); //)
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }

        public Image GetImageFromUri(Uri uri)
        {
            string fileName = System.IO.Path.GetTempFileName();
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(uri, fileName);
            }
            Image img;

            // MUST KEEP STREAM OPEN IN ORDER FOR THE IMAGE TO BE USABLE LATER
            FileStream fs = File.OpenRead(fileName);
            //using (FileStream fs = File.OpenRead(fileName))
            //{
                img = Image.FromStream(fs);
            //}
            //File.Delete(fileName);

            return img;
        }

        public void PopulateCulture(int addOrEdit_id)
        {
            List<ListViewItem> lvis = new List<ListViewItem>();
            bool dupes = false;

            if (addOrEdit_id != -1)
            {
                // add/edit single item
                foreach (ListViewItem lvi in lvw.Items)
                {
                    if ((lvi.Tag as ethos).id == addOrEdit_id)
                    {
                        lvw.Items.Remove(lvi);
                        break;
                    }
                }
                ethos e = culture.Where(p => p.id == addOrEdit_id).First();
                ListViewItem newLvi = lviFromEthos(e);
                lvw.Items.Add(newLvi);
                newLvi.Selected = true;
                newLvi.EnsureVisible();
            }
            else 
            {
                // all
                foreach (ethos e in culture)
                {
                    ListViewItem lvi = lviFromEthos(e);
                    if (e.id == addOrEdit_id)
                        lvi.Selected = true;
                    if (e.picHash != null)
                    {
                        int hashCount = culture.Count(p => p.picHash == e.picHash);
                        if (hashCount > 1)
                        {
                            lvi.ForeColor = Color.Red;
                            lvi.Font = new Font(lvi.Font, FontStyle.Bold);
                            dupes = true;
                        }
                    }
                    //if (e.nsfw)
                    //{
                    //    lvi.ForeColor = Color.Red;
                    //}

                    lvis.Add(lvi);
                }
                if (dupes)
                    MessageBox.Show("duplicate images! highlighted in red...");

                lvw.BeginUpdate();
                lvw.Items.Clear();
                lvw.Items.AddRange(lvis.ToArray());
                lvw.EndUpdate();

                if (lvw.SelectedItems.Count > 0)
                    lvw.EnsureVisible(lvw.SelectedItems[0].Index);

                for (int i = 1; i < this.lvw.Columns.Count - 1; i++)
                    this.lvw.Columns[i].Width = -2;
                this.lvw.Columns[0].Width = 200;
            }
        }

        private static ListViewItem lviFromEthos(ethos e)
        {
            ListViewItem lvi = new ListViewItem(
                new string[] { e.quote, e.attrib, e.pic != null || e.fastGotPic ? "*" : "", 
                                       e.addedOn.ToString("dd MMM yyyy HH:mm"), 
                                       e.addedBy, 
                                       e.nsfw ? "NSFW" : "",
                                       (e.rank * 100).ToString("#,00"),
                                       e.picType,
                                       e.url, 
                                       e.picHash });
            lvi.Tag = e;
            return lvi;
        }

        private void ilvw_SelectionChanged(object sender, EventArgs e)
        {
            this.display.Cursor = Cursors.WaitCursor;
            if (ilvw.SelectedItems.Count == 0) return;
            var id = Convert.ToInt32(ilvw.SelectedItems[0].Tag);
            for (int i=0; i < lvw.Items.Count; i++) {
                var lvi = lvw.Items[i];
                ethos et = lvi.Tag as ethos;
                //Debug.WriteLine($"id={id} et.id={et.id}");
                if (et.id == id) {
                    lvw.Items[i].Selected = true;
                    break;
                }
            }
            this.TogglePause();

            //using (ethosDataContext db = new ethosDataContext(conStr))
            //{
            //    etho ee = db.ethos.FirstOrDefault(p => p.id == id);
            //    if (ee != null) {
            //        var et = FromDbo(ee);
            //        display.ShowEthos(et);
            //        txtQuote.Text = et.quote;
            //        txtAttrib.Text = et.attrib;
            //        chkNSFW.Checked = et.nsfw;
            //        SetUi();
            //    }
            //}
            this.display.Cursor = Cursors.Default;
        }

        internal void lvw_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.display.Cursor = Cursors.WaitCursor;
            if (lvw.SelectedItems.Count == 0) return;
            ethos et = lvw.SelectedItems[0].Tag as ethos;
            if (sender != null && e != null)
                if (!this.timer1.Enabled)
                    TogglePause();
            display.ShowEthos(et);
            txtQuote.Text = et.quote;
            txtAttrib.Text = et.attrib;
            chkNSFW.Checked = et.nsfw;
            SetUi();
            this.display.Cursor = Cursors.Default;
        }

        internal void cmdSave_Click(object sender, EventArgs e)
        {
            if (lvw.SelectedItems.Count == 0) return;
            ethos et = lvw.SelectedItems[0].Tag as ethos;
            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                etho edb = db.ethos.FirstOrDefault(p => p.id == et.id);
                edb.quote = et.quote = txtQuote.Text;
                edb.attrib = et.attrib = txtAttrib.Text;
                edb.nsfw = et.nsfw = chkNSFW.Checked;
                db.SubmitChanges();
                //lvw_SelectedIndexChanged(null, null);
                PopulateCulture(edb.id);
            }
        }

        void SetUi()
        {
            bool selected = lvw.SelectedItems.Count > 0;
            cmdSave.Enabled = chkNSFW.Enabled = cmdDelete.Enabled = txtQuote.Enabled = selected;
            if (!selected)
                txtQuote.Text = "";
            else
                if (!indexesDisplayed.Contains(lvw.SelectedItems[0].Index))
                    indexesDisplayed.Add(lvw.SelectedItems[0].Index);
            SetCaption();
        }

        private void txtQuote_TextChanged(object sender, EventArgs e)
        {
            cmdSave.Enabled = true;
        }

        internal void cmdDelete_Click_1(object sender, EventArgs e)
        {
            if (lvw.SelectedItems.Count == 0) return;
            ethos et = lvw.SelectedItems[0].Tag as ethos;
            if (MessageBox.Show("sure or not?", "delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                using (ethosDataContext db = new ethosDataContext(conStr))
                {
                    db.ethos.DeleteOnSubmit(db.ethos.FirstOrDefault(p => p.id == et.id));
                    db.SubmitChanges();
                }
                culture.Remove(et);
                lvw.Items.Remove(lvw.SelectedItems[0]);
                SetUi();
            }
        }

        int tickCount = 1;
        private bool showingNSFW = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = rnd.Next(60, 90) * 1000;

            bool showSpecial = false;
            //if ((++tickCount) % 2 == 0 && sender != null)
            //{
            //    timer1.Interval = 10 * 1000;
            //    showSpecial = true;
            //}

            showingNSFW = DateTime.Now.Hour >= 20 || DateTime.Now.Hour < 8
                       || DateTime.Now.DayOfWeek == DayOfWeek.Saturday 
                       || DateTime.Now.DayOfWeek == DayOfWeek.Sunday;
again:
//<<<<<<< HEAD
//            int ndx;
//            if (sender is int)
//                if ((int)sender < indexesDisplayed.Count)
//                    ndx = indexesDisplayed[(int)sender];
//                else
//                    ndx = indexesDisplayed[indexesDisplayed.Count - 1] + 1;
//            else
//                ndx = rnd.Next(lvw.Items.Count);

//=======
            int ndx = showSpecial ? 0
                                  : sender is int ? indexesDisplayed[(int)sender] : rnd.Next(lvw.Items.Count);
//>>>>>>> origin/master
            ethos et = lvw.Items[ndx].Tag as ethos;
            if (et.nsfw && !showingNSFW)
                goto again;

            SetCaption();

            lvw.Items[ndx].Selected = true;
            lvw.Items[ndx].EnsureVisible();
            lvw.Refresh();

            // hack; flower loses its topmost after some time, don't know why
            this.flower.TopMost = false;
            this.flower.TopMost = true;
        }

        private void SetCaption()
        {
            this.Text = MainForm.caption;

            if (this.display != null && this.display._e != null)
                this.Text += " " + this.display._e.ToString();

            if (!this.timer1.Enabled)
                this.Text += " (paused)";
            else
                this.Text += (showingNSFW ? " >>> SHOWING NSFW <<<" : " (sfw)");
            
            this.display.Text = this.Text;
        }

        private void lvw_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) {
                cmdDelete_Click_1(null, null);
            }
            timer1.Enabled = false; timer1.Enabled = true; // reset timer
        }

        private void txtAttrib_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdLoadAll_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            LoadAll(false, saveAll:true);
            this.Cursor = Cursors.Default;
        }

        private void chkNSFW_CheckedChanged(object sender, EventArgs e)
        {

        }

        bool firstActivation = true;
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (lvw.SelectedItems.Count == 0) return;
            int ndx = lvw.SelectedItems[0].Index;
            lvw.Items[ndx].Selected = true;
            lvw.Items[ndx].EnsureVisible();

            if (firstActivation)
            {
                //firstActivation = false;
                //this.Height = Screen.GetWorkingArea(this.Location).Height;
                //this.lvw.Height = (this.Height / 8) * 4;
            }
        }

        internal void TogglePause()
        {
            this.timer1.Enabled = !this.timer1.Enabled;
            this.lvw_SelectedIndexChanged(null, null);
            SetCaption();
        }

        List<int> indexesDisplayed = new List<int>();
        int? ndxOverride = null;
        internal void Previous()
        {
            this.display.Cursor = Cursors.WaitCursor;

            //ndxOverride = Math.Max(0, (ndxOverride ?? indexesDisplayed.Count - 1) - 1);
            //timer1_Tick(ndxOverride, null);

            timer1.Enabled = false; timer1.Enabled = true; // reset timer
            timer1.Interval = timer1.Interval + 1;
            if (this.lvw.SelectedItems.Count == 0 || this.lvw.SelectedIndices[0] == 0) return;
            this.lvw.Items[this.lvw.SelectedIndices[0] - 1].Selected = true;
            this.lvw.SelectedItems[0].EnsureVisible();
            this.display.Activate();

            this.display.Cursor = Cursors.Default;
        }

        internal void Next()
        {
            this.display.Cursor = Cursors.WaitCursor;

            //ndxOverride = Math.Max(0, (ndxOverride ?? indexesDisplayed.Count - 1) + 1);
            //timer1_Tick(ndxOverride, null);

            timer1.Enabled = false; timer1.Enabled = true; // reset timer
            timer1.Interval = timer1.Interval + 1;
            if (this.lvw.SelectedItems.Count == 0 || this.lvw.SelectedIndices[0] == this.lvw.Items.Count - 1) return;
            this.lvw.Items[this.lvw.SelectedIndices[0] + 1].Selected = true;
            this.lvw.SelectedItems[0].EnsureVisible();
            this.display.Activate();

            this.display.Cursor = Cursors.Default;
        }

        internal void PasteClipboard()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string clipText = Clipboard.GetText();
                if (!string.IsNullOrEmpty(clipText))
                {
                    Uri uri = null; try { uri = new Uri(clipText); }
                    catch { return; }
                    Image img = GetImageFromUri(new Uri(clipText));
                    if (img != null)
                        PopulateCulture(AddEthos(new ethos { quote = null, pic = img, url = clipText }));
                }
                else
                {
                    IDataObject data = Clipboard.GetDataObject();
                    if (data.GetDataPresent(DataFormats.Bitmap))
                    {
                        Image img = (Image)data.GetData(DataFormats.Bitmap, true);
                        if (img != null)
                        {
                            // sometimes get all black bmp's (when running in parallels & sharing clipboard from mac)
                            Bitmap bmp = new Bitmap(img);
                            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
                            IntPtr ptr = bmpData.Scan0;
                            int bytes = bmpData.Stride * bmp.Height;
                            byte[] rgbValues = new byte[bytes];
                            Marshal.Copy(ptr, rgbValues, 0, bytes);
                            bool allBlack = true;
                            for (int index = 0; index < rgbValues.Length; index++) // Scanning for non-zero bytes
                                if (rgbValues[index] != 0 && rgbValues[index] != 255 )
                                {
                                    allBlack = false;
                                    break;
                                }
                            bmp.UnlockBits(bmpData);

                            if (!allBlack)
                                PopulateCulture(AddEthos(new ethos { quote = null, pic = img, url = null }));
                            else
                                MessageBox.Show("all black!");
                        }
                    }
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
                PasteClipboard();

            else if (e.KeyCode == Keys.W)
                e.Handled = true;

            else if (e.KeyCode == Keys.Escape)
                Program.frmMain.Close();

            else if (e.KeyCode == Keys.D)
                cmdDelete_Click_1(null, null);
        }

        private void CopyCurrentImageToClipboard() {
            if (lvw.SelectedItems.Count == 0) {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            ethos et = lvw.SelectedItems[0].Tag as ethos;
            if (et == null)
                return;

            // Try to get the image from the ethos object
            Image img = et.pic;

            // If the image is not loaded (because of fast load), try to load it from disk
            if (img == null && et.fastGotPic && et.id > 0) {
                string fileName = et.nsfw
                    ? Path.Combine(nsfwDumpDir, et.id + ".gif")
                    : Path.Combine(picsDumpDir, et.id + ".gif");
                if (File.Exists(fileName)) {
                    try {
                        img = Image.FromFile(fileName);
                    }
                    catch {
                        img = null;
                    }
                }
            }

            if (img != null) {
                try {
                    Clipboard.SetImage(img);
                }
                catch (ExternalException) {
                    MessageBox.Show("Could not copy image to clipboard. Another process may be locking the clipboard.", "Clipboard Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally {
                    // If we loaded the image from disk, dispose it
                    if (et.pic == null && img != null)
                        img.Dispose();
                }
            }
            else {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                this.SetWallpaper();
                e.Handled = true;
            }
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w' || e.KeyChar == 'W')
                e.Handled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.WindowState = Settings.Default.mainWindowState;
            if (Settings.Default.mainLocation != null && Settings.Default.mainSize != null)
            {
                if (Program.IsVisibleOnAnyScreen(Settings.Default.mainLocation, Settings.Default.mainSize))
                {
                    this.Location = Settings.Default.mainLocation;
                    this.Size = Settings.Default.mainSize;

                    this.Height = Screen.GetWorkingArea(this.Location).Height;
                    this.Top = 0;
                    this.Left = Screen.GetWorkingArea(this.Location).Width - this.Width;
                    this.lvw.Height = (this.Height / 8) * 6;
                }
            }

            if (Program.ss)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.flower.Visible = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.mainLocation = this.Location;
            if (this.WindowState == FormWindowState.Normal)
                Settings.Default.mainSize = this.Size;
            else
                Settings.Default.mainSize = this.RestoreBounds.Size;
            Settings.Default.mainWindowState = this.WindowState;
            Settings.Default.Save();
        }

        internal void SetWallpaper()
        {
            string tmpFile2 = display.tmpFile + ".wallpaper";
            File.Copy(display.tmpFile, tmpFile2);

            Uri uri = new Uri(tmpFile2);
            Wallpaper.Set(uri, Wallpaper.Style.Centered);
        }

        private void lvw_MouseUp(object sender, MouseEventArgs e)
        {
            this.TogglePause();
        }
    }
}
