using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ethos_viewer
{
    public class ethos
    {
        public int id;
        public string quote;
        public string attrib;
        public bool fastGotPic;
        public Image pic;
        public string picHash;
        public string addedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        public bool nsfw;
        public bool? special;
        public DateTime addedOn = DateTime.Now;
        public double rank = 0.5;
        public string url;

        public string picType { get {
            if (pic == null) return "-";
            if (ImageFormat.Jpeg.Equals(pic.RawFormat)) return "JPEG";
            if (ImageFormat.Png.Equals(pic.RawFormat)) return "PNG";
            if (ImageFormat.Gif.Equals(pic.RawFormat)) return "GIF";
            return "?"; } }

        public override string ToString()
        {
            return "#" + this.id.ToString();
        }
    }
}
