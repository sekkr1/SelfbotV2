namespace SelfbotV2
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
            this.mynotifyicon = new System.Windows.Forms.NotifyIcon(this.components);
            this.prefixTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.help = new System.Windows.Forms.Label();
            this.connected = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.logBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // mynotifyicon
            // 
            this.mynotifyicon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.mynotifyicon.BalloonTipText = "now in tray";
            this.mynotifyicon.BalloonTipTitle = "Discord selfbot";
            this.mynotifyicon.Icon = ((System.Drawing.Icon)(resources.GetObject("mynotifyicon.Icon")));
            this.mynotifyicon.Text = "Discord selfbot";
            this.mynotifyicon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Mynotifyicon_MouseClick);
            // 
            // prefixTB
            // 
            this.prefixTB.Enabled = false;
            this.prefixTB.Location = new System.Drawing.Point(141, 13);
            this.prefixTB.MaxLength = 10;
            this.prefixTB.Name = "prefixTB";
            this.prefixTB.Size = new System.Drawing.Size(99, 20);
            this.prefixTB.TabIndex = 0;
            this.prefixTB.Text = "?";
            this.prefixTB.TextChanged += new System.EventHandler(this.PrefixChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Command prefix";
            // 
            // help
            // 
            this.help.AutoSize = true;
            this.help.Location = new System.Drawing.Point(47, 36);
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(114, 13);
            this.help.TabIndex = 2;
            this.help.Text = "?help     for commands";
            // 
            // connected
            // 
            this.connected.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connected.Location = new System.Drawing.Point(1, 0);
            this.connected.Name = "connected";
            this.connected.Size = new System.Drawing.Size(286, 49);
            this.connected.TabIndex = 3;
            this.connected.Text = "Connecting.";
            this.connected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // logBox
            // 
            this.logBox.FormattingEnabled = true;
            this.logBox.Location = new System.Drawing.Point(1, 52);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(286, 134);
            this.logBox.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 184);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.connected);
            this.Controls.Add(this.help);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.prefixTB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Discord selfbot ©Ogre";
            this.Load += new System.EventHandler(this.Form1_LoadAsync);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon mynotifyicon;
        private System.Windows.Forms.TextBox prefixTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label help;
        private System.Windows.Forms.Label connected;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox logBox;
    }
}

