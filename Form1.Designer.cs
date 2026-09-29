namespace ethos_viewer
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lvw = new System.Windows.Forms.ListView();
            this.chquote = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chattrib = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chpic = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaddedOn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaddedBy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chnsfw = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chrank = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chtype = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.churl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chhash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkNSFW = new System.Windows.Forms.CheckBox();
            this.txtAttrib = new System.Windows.Forms.TextBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.txtQuote = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cmdLoadAll = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvw
            // 
            this.lvw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chquote,
            this.chattrib,
            this.chpic,
            this.chaddedOn,
            this.chaddedBy,
            this.chnsfw,
            this.chrank,
            this.chtype,
            this.churl,
            this.chhash});
            this.lvw.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvw.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvw.FullRowSelect = true;
            this.lvw.HideSelection = false;
            this.lvw.Location = new System.Drawing.Point(0, 0);
            this.lvw.MultiSelect = false;
            this.lvw.Name = "lvw";
            this.lvw.Size = new System.Drawing.Size(966, 230);
            this.lvw.TabIndex = 0;
            this.lvw.UseCompatibleStateImageBehavior = false;
            this.lvw.View = System.Windows.Forms.View.Details;
            this.lvw.SelectedIndexChanged += new System.EventHandler(this.lvw_SelectedIndexChanged);
            this.lvw.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvw_KeyUp);
            // 
            // chquote
            // 
            this.chquote.Text = "quote";
            // 
            // chattrib
            // 
            this.chattrib.DisplayIndex = 6;
            this.chattrib.Text = "attrib";
            // 
            // chpic
            // 
            this.chpic.DisplayIndex = 1;
            this.chpic.Text = "pic";
            // 
            // chaddedOn
            // 
            this.chaddedOn.DisplayIndex = 2;
            this.chaddedOn.Text = "addedOn";
            // 
            // chaddedBy
            // 
            this.chaddedBy.DisplayIndex = 3;
            this.chaddedBy.Text = "addedBy";
            this.chaddedBy.Width = 76;
            // 
            // chnsfw
            // 
            this.chnsfw.DisplayIndex = 4;
            this.chnsfw.Text = "NSFW";
            // 
            // chrank
            // 
            this.chrank.DisplayIndex = 5;
            this.chrank.Text = "rank";
            // 
            // chtype
            // 
            this.chtype.Text = "format";
            // 
            // churl
            // 
            this.churl.Text = "src";
            // 
            // chhash
            // 
            this.chhash.Text = "hash";
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 230);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(966, 6);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdLoadAll);
            this.panel1.Controls.Add(this.chkNSFW);
            this.panel1.Controls.Add(this.txtAttrib);
            this.panel1.Controls.Add(this.cmdSave);
            this.panel1.Controls.Add(this.cmdDelete);
            this.panel1.Controls.Add(this.txtQuote);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 236);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(966, 457);
            this.panel1.TabIndex = 5;
            // 
            // chkNSFW
            // 
            this.chkNSFW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkNSFW.AutoSize = true;
            this.chkNSFW.Font = new System.Drawing.Font("Courier New", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNSFW.ForeColor = System.Drawing.Color.DarkRed;
            this.chkNSFW.Location = new System.Drawing.Point(12, 384);
            this.chkNSFW.Name = "chkNSFW";
            this.chkNSFW.Size = new System.Drawing.Size(73, 26);
            this.chkNSFW.TabIndex = 7;
            this.chkNSFW.Text = "NSFW";
            this.chkNSFW.UseVisualStyleBackColor = true;
            // 
            // txtAttrib
            // 
            this.txtAttrib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAttrib.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAttrib.Location = new System.Drawing.Point(91, 383);
            this.txtAttrib.Multiline = true;
            this.txtAttrib.Name = "txtAttrib";
            this.txtAttrib.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAttrib.Size = new System.Drawing.Size(872, 32);
            this.txtAttrib.TabIndex = 6;
            this.txtAttrib.TextChanged += new System.EventHandler(this.txtAttrib_TextChanged);
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSave.Location = new System.Drawing.Point(906, 427);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(57, 27);
            this.cmdSave.TabIndex = 5;
            this.cmdSave.Text = "save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdDelete.Location = new System.Drawing.Point(3, 427);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(57, 27);
            this.cmdDelete.TabIndex = 4;
            this.cmdDelete.Text = "delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click_1);
            // 
            // txtQuote
            // 
            this.txtQuote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuote.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQuote.Location = new System.Drawing.Point(3, 3);
            this.txtQuote.Multiline = true;
            this.txtQuote.Name = "txtQuote";
            this.txtQuote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtQuote.Size = new System.Drawing.Size(960, 374);
            this.txtQuote.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cmdLoadAll
            // 
            this.cmdLoadAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLoadAll.Location = new System.Drawing.Point(407, 427);
            this.cmdLoadAll.Name = "cmdLoadAll";
            this.cmdLoadAll.Size = new System.Drawing.Size(158, 27);
            this.cmdLoadAll.TabIndex = 8;
            this.cmdLoadAll.Text = "load all && check for dupes";
            this.cmdLoadAll.UseVisualStyleBackColor = true;
            this.cmdLoadAll.Click += new System.EventHandler(this.cmdLoadAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 693);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.lvw);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "title";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvw;
        private System.Windows.Forms.ColumnHeader chquote;
        private System.Windows.Forms.ColumnHeader chpic;
        private System.Windows.Forms.ColumnHeader chaddedOn;
        private System.Windows.Forms.ColumnHeader chaddedBy;
        private System.Windows.Forms.ColumnHeader chrank;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.TextBox txtQuote;
        private System.Windows.Forms.ColumnHeader chattrib;
        private System.Windows.Forms.TextBox txtAttrib;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ColumnHeader chtype;
        private System.Windows.Forms.ColumnHeader churl;
        private System.Windows.Forms.ColumnHeader chhash;
        private System.Windows.Forms.ColumnHeader chnsfw;
        private System.Windows.Forms.CheckBox chkNSFW;
        private System.Windows.Forms.Button cmdLoadAll;

    }
}

