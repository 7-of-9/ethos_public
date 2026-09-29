namespace ethos_viewer
{
    partial class PicForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PicForm));
            this.ilvw = new Manina.Windows.Forms.ImageListView();
            this.SuspendLayout();
            // 
            // ilvw
            // 
            this.ilvw.DefaultImage = ((System.Drawing.Image)(resources.GetObject("ilvw.DefaultImage")));
            this.ilvw.ErrorImage = ((System.Drawing.Image)(resources.GetObject("ilvw.ErrorImage")));
            this.ilvw.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ilvw.Location = new System.Drawing.Point(12, 12);
            this.ilvw.Name = "ilvw";
            this.ilvw.Size = new System.Drawing.Size(441, 248);
            this.ilvw.TabIndex = 0;
            this.ilvw.Text = "";
            // 
            // frmMain2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 340);
            this.Controls.Add(this.ilvw);
            this.Name = "frmMain2";
            this.Text = "frmMain2";
            this.ResumeLayout(false);

        }

        #endregion

        private Manina.Windows.Forms.ImageListView ilvw;
    }
}