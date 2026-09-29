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

namespace ethos_viewer
{
    public partial class Form1 : Form
    {
        public List<ethos> culture;
        public Display display = new Display();
        internal static string conStr = global::ethos_viewer.Properties.Settings.Default.ethosConnectionString;
        
        Flower flower = new Flower();
        Random rnd = new Random();
        string caption = "ethos viewer v0.7";

        public Form1()
        {
            InitializeComponent();

            LoadAll(true);

            this.Text = caption;
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            
            lvw.DragDrop += new DragEventHandler(lvw_DragDrop);
            lvw.DragEnter += new DragEventHandler(lvw_DragEnter);
            
            display.Show();
            flower.Location = new Point(0, 0);
            flower.Show();

            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                this.caption += " {" + db.Connection.DataSource + "}";
            }

            timer1.Interval = rnd.Next(60, 90) * 1000;
            timer1_Tick(null, null);
        }
        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            display.Close();
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

                        PopulateCulture(AddEthos(new ethos { quote = text, pic = null }));
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

        void LoadAll(bool loadFast)
        {
            culture = new List<ethos>();
            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                if (loadFast)
                {
                    var ethosWithoutPics = from x in db.ethos
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
                                           };
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

                        culture.Add(e);
                    }
                }
                else
                {
                    int ndx = 0;
                    int count = db.ethos.Count();
                    db.CommandTimeout = 90; // 1 min cmd timeout for expensive fetchs of image data
                    
                    //List<etho> loadedEthos = db.ethos.ToList();
                    //foreach(etho et in loadedEthos)
                    foreach (etho et in db.ethos)
                    {
                        this.Text = this.caption + " LoadAll " + (((float)ndx++ / (float)count) * 100).ToString(",.") + "%";
                        Application.DoEvents();
                        ethos e = FromDbo(et);
                        culture.Add(e);

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

        int AddEthos(ethos e)
        {
            if (!string.IsNullOrEmpty(e.url) && culture.Where(p => p.url == e.url).Count() > 0)
            {
                var alreadyPresent = culture.Where(p => p.url == e.url).FirstOrDefault();
                if (alreadyPresent != null)
                {
                    MessageBox.Show("URL already present; skipping add!");
                    return alreadyPresent.id;
                }
            }

            culture.Add(e);
            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                etho et = ToDbo(e);
                db.ethos.InsertOnSubmit(et);
                db.SubmitChanges();
                e.id = et.id;
                return e.id;
            }
            this.WindowState = FormWindowState.Normal;
            this.Show();
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
                et.pic = ImageToByteArray(e.pic);
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

        private Image GetImageFromUri(Uri uri)
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

        void PopulateCulture(int addOrEdit_id)
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

        private void lvw_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvw.SelectedItems.Count == 0) return;
            ethos et = lvw.SelectedItems[0].Tag as ethos;
            display.ShowDiag(et);
            txtQuote.Text = et.quote;
            txtAttrib.Text = et.attrib;
            chkNSFW.Checked = et.nsfw;
            SetUi();
        }

        private void cmdSave_Click(object sender, EventArgs e)
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
            if (!selected) txtQuote.Text = "";
        }

        private void txtQuote_TextChanged(object sender, EventArgs e)
        {
            cmdSave.Enabled = true;
        }

        private void cmdDelete_Click_1(object sender, EventArgs e)
        {
            if (lvw.SelectedItems.Count == 0) return;
            ethos et = lvw.SelectedItems[0].Tag as ethos;
            using (ethosDataContext db = new ethosDataContext(conStr))
            {
                db.ethos.DeleteOnSubmit(db.ethos.FirstOrDefault(p => p.id == et.id));
                db.SubmitChanges();
            }
            culture.Remove(et);
            lvw.Items.Remove(lvw.SelectedItems[0]);
            SetUi();
        }

        int tickCount = 1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = rnd.Next(60, 90) * 1000;

            if ((++tickCount) % 10 == 0)
                rnd = new Random();

            bool showNsfw = DateTime.Now.Hour >= 19 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday;
again:
            int ndx = rnd.Next(lvw.Items.Count);
            ethos et = lvw.Items[ndx].Tag as ethos;
            if (et.nsfw && !showNsfw)
                goto again;

            this.Text = this.caption + (showNsfw ? " >>> NSFW is ON <<<" : "");

            lvw.Items[ndx].Selected = true;
            lvw.Items[ndx].EnsureVisible();
            lvw.Refresh();

            // hack; flower loses its topmost after some time, don't know why
            this.flower.TopMost = false;
            this.flower.TopMost = true;
        }

        private void lvw_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) {
                cmdDelete_Click_1(null, null);
            }
            timer1.Enabled = false;
            timer1.Enabled = true; // reset timer
        }

        private void txtAttrib_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdLoadAll_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            LoadAll(false);
            this.Cursor = Cursors.Default;
        }
    }
}
