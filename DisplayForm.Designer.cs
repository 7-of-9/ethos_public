namespace ethos_viewer
{
    partial class DisplayForm
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
            this.lbl = new System.Windows.Forms.Label();
            this.lblAttrib = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pic2 = new System.Windows.Forms.PictureBox();
            this.cmdPause = new System.Windows.Forms.Button();
            this.cmdNSFW = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.cmdPrev = new System.Windows.Forms.Button();
            this.cmdNext = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl
            // 
            this.lbl.BackColor = System.Drawing.Color.Transparent;
            this.lbl.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl.ForeColor = System.Drawing.Color.White;
            this.lbl.Location = new System.Drawing.Point(25, 57);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(183, 118);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "label1 asd asd asd  asd asd asd asd asd asd asdas dasd a12e123123123123";
            this.lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl.DoubleClick += new System.EventHandler(this.lbl_DoubleClick);
            // 
            // lblAttrib
            // 
            this.lblAttrib.AutoSize = true;
            this.lblAttrib.BackColor = System.Drawing.Color.Transparent;
            this.lblAttrib.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAttrib.ForeColor = System.Drawing.Color.DarkGray;
            this.lblAttrib.Location = new System.Drawing.Point(246, 7);
            this.lblAttrib.Name = "lblAttrib";
            this.lblAttrib.Size = new System.Drawing.Size(42, 15);
            this.lblAttrib.TabIndex = 2;
            this.lblAttrib.Text = "label1";
            this.lblAttrib.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblAttrib.DoubleClick += new System.EventHandler(this.lblAttrib_DoubleClick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pic2
            // 
            this.pic2.BackColor = System.Drawing.Color.DimGray;
            this.pic2.Location = new System.Drawing.Point(66, 15);
            this.pic2.Name = "pic2";
            this.pic2.Size = new System.Drawing.Size(120, 57);
            this.pic2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic2.TabIndex = 4;
            this.pic2.TabStop = false;
            this.pic2.Click += new System.EventHandler(this.pic2_Click);
            this.pic2.DoubleClick += new System.EventHandler(this.pic2_DoubleClick);
            this.pic2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic2_MouseDown);
            this.pic2.MouseEnter += new System.EventHandler(this.pic2_MouseEnter);
            this.pic2.MouseLeave += new System.EventHandler(this.pic2_MouseLeave);
            this.pic2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic2_MouseMove);
            // 
            // cmdPause
            // 
            this.cmdPause.BackColor = System.Drawing.Color.DimGray;
            this.cmdPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdPause.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdPause.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdPause.Location = new System.Drawing.Point(95, 3);
            this.cmdPause.Name = "cmdPause";
            this.cmdPause.Size = new System.Drawing.Size(53, 23);
            this.cmdPause.TabIndex = 1;
            this.cmdPause.Text = "paused: 0";
            this.cmdPause.UseVisualStyleBackColor = false;
            this.cmdPause.Click += new System.EventHandler(this.cmdPause_Click);
            // 
            // cmdNSFW
            // 
            this.cmdNSFW.BackColor = System.Drawing.Color.DimGray;
            this.cmdNSFW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdNSFW.Font = new System.Drawing.Font("Courier New", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdNSFW.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdNSFW.Location = new System.Drawing.Point(7, 3);
            this.cmdNSFW.Name = "cmdNSFW";
            this.cmdNSFW.Size = new System.Drawing.Size(84, 23);
            this.cmdNSFW.TabIndex = 0;
            this.cmdNSFW.Text = "NSFW: 0";
            this.cmdNSFW.UseVisualStyleBackColor = false;
            this.cmdNSFW.Click += new System.EventHandler(this.cmdNSFW_Click);
            this.cmdNSFW.MouseEnter += new System.EventHandler(this.cmdNSFW_MouseEnter);
            // 
            // timer2
            // 
            this.timer2.Interval = 1500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // cmdPrev
            // 
            this.cmdPrev.BackColor = System.Drawing.Color.DimGray;
            this.cmdPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdPrev.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdPrev.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdPrev.Location = new System.Drawing.Point(152, 3);
            this.cmdPrev.Name = "cmdPrev";
            this.cmdPrev.Size = new System.Drawing.Size(48, 23);
            this.cmdPrev.TabIndex = 5;
            this.cmdPrev.Text = "<<";
            this.cmdPrev.UseVisualStyleBackColor = false;
            this.cmdPrev.Click += new System.EventHandler(this.cmdPrev_Click);
            // 
            // cmdNext
            // 
            this.cmdNext.BackColor = System.Drawing.Color.DimGray;
            this.cmdNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdNext.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdNext.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdNext.Location = new System.Drawing.Point(204, 3);
            this.cmdNext.Name = "cmdNext";
            this.cmdNext.Size = new System.Drawing.Size(48, 23);
            this.cmdNext.TabIndex = 6;
            this.cmdNext.Text = ">>";
            this.cmdNext.UseVisualStyleBackColor = false;
            this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.cmdNext);
            this.panel1.Controls.Add(this.cmdNSFW);
            this.panel1.Controls.Add(this.cmdPrev);
            this.panel1.Controls.Add(this.cmdPause);
            this.panel1.Controls.Add(this.pic2);
            this.panel1.Controls.Add(this.lblAttrib);
            this.panel1.Controls.Add(this.lbl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(720, 506);
            this.panel1.TabIndex = 9;
            this.panel1.DoubleClick += new System.EventHandler(this.panel1_DoubleClick);
            this.panel1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDoubleClick);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
            this.panel1.MouseLeave += new System.EventHandler(this.panel1_MouseLeave);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // DisplayForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(726, 512);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "DisplayForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Display";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Display_FormClosing);
            this.Load += new System.EventHandler(this.Display_Load);
            this.ResizeEnd += new System.EventHandler(this.Display_ResizeEnd);
            this.DoubleClick += new System.EventHandler(this.Display_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Display_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Display_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Display_MouseDown);
            this.MouseEnter += new System.EventHandler(this.Display_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.Display_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Display_MouseMove);
            this.Resize += new System.EventHandler(this.Display_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Label lblAttrib;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pic2;
        private System.Windows.Forms.Button cmdPause;
        private System.Windows.Forms.Button cmdNSFW;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button cmdPrev;
        private System.Windows.Forms.Button cmdNext;
        private System.Windows.Forms.Panel panel1;
    }
}