namespace D3Overseer
{
    partial class frmOverseer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOverseer));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblGoldRate = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblGold = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblXP = new System.Windows.Forms.Label();
            this.lblXPRate = new System.Windows.Forms.Label();
            this.lblTFaction = new System.Windows.Forms.Label();
            this.lblTDist = new System.Windows.Forms.Label();
            this.lblTHealth = new System.Windows.Forms.Label();
            this.lblTiming = new System.Windows.Forms.Label();
            this.lblAttached = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblRes2 = new System.Windows.Forms.Label();
            this.lblRes1 = new System.Windows.Forms.Label();
            this.progExp = new System.Windows.Forms.ProgressBar();
            this.lblHealth = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdSettings = new System.Windows.Forms.Button();
            this.cmdShutdown = new System.Windows.Forms.Button();
            this.cmdStartStop = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabMap = new System.Windows.Forms.TabPage();
            this.panelMap = new System.Windows.Forms.Panel();
            this.picMap = new System.Windows.Forms.PictureBox();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.picBackpack = new System.Windows.Forms.PictureBox();
            this.progRes2 = new D3Overseer.ProgressBarEx();
            this.progRes1 = new D3Overseer.ProgressBarEx();
            this.progHealth = new D3Overseer.ProgressBarEx();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabMap.SuspendLayout();
            this.panelMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.tabInventory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBackpack)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLevel);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.progRes2);
            this.groupBox1.Controls.Add(this.progRes1);
            this.groupBox1.Controls.Add(this.progHealth);
            this.groupBox1.Controls.Add(this.lblGoldRate);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.lblGold);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.lblXP);
            this.groupBox1.Controls.Add(this.lblXPRate);
            this.groupBox1.Controls.Add(this.lblTFaction);
            this.groupBox1.Controls.Add(this.lblTDist);
            this.groupBox1.Controls.Add(this.lblTHealth);
            this.groupBox1.Controls.Add(this.lblTiming);
            this.groupBox1.Controls.Add(this.lblAttached);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblRes2);
            this.groupBox1.Controls.Add(this.lblRes1);
            this.groupBox1.Controls.Add(this.progExp);
            this.groupBox1.Controls.Add(this.lblHealth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 275);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblLevel.Location = new System.Drawing.Point(73, 230);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(13, 13);
            this.lblLevel.TabIndex = 36;
            this.lblLevel.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(31, 230);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 35;
            this.label9.Text = "Level:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblGoldRate
            // 
            this.lblGoldRate.AutoSize = true;
            this.lblGoldRate.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblGoldRate.Location = new System.Drawing.Point(218, 60);
            this.lblGoldRate.Name = "lblGoldRate";
            this.lblGoldRate.Size = new System.Drawing.Size(22, 13);
            this.lblGoldRate.TabIndex = 31;
            this.lblGoldRate.Text = "0.0";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(152, 60);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(60, 13);
            this.label18.TabIndex = 30;
            this.label18.Text = "Gold/Hour:";
            this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblGold
            // 
            this.lblGold.AutoSize = true;
            this.lblGold.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblGold.Location = new System.Drawing.Point(218, 33);
            this.lblGold.Name = "lblGold";
            this.lblGold.Size = new System.Drawing.Size(13, 13);
            this.lblGold.TabIndex = 29;
            this.lblGold.Text = "0";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(180, 33);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 13);
            this.label16.TabIndex = 28;
            this.label16.Text = "Gold:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblXP
            // 
            this.lblXP.AutoSize = true;
            this.lblXP.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblXP.Location = new System.Drawing.Point(308, 230);
            this.lblXP.Name = "lblXP";
            this.lblXP.Size = new System.Drawing.Size(30, 13);
            this.lblXP.TabIndex = 27;
            this.lblXP.Text = "0 / 0";
            // 
            // lblXPRate
            // 
            this.lblXPRate.AutoSize = true;
            this.lblXPRate.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblXPRate.Location = new System.Drawing.Point(205, 230);
            this.lblXPRate.Name = "lblXPRate";
            this.lblXPRate.Size = new System.Drawing.Size(22, 13);
            this.lblXPRate.TabIndex = 26;
            this.lblXPRate.Text = "0.0";
            // 
            // lblTFaction
            // 
            this.lblTFaction.AutoSize = true;
            this.lblTFaction.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblTFaction.Location = new System.Drawing.Point(308, 167);
            this.lblTFaction.Name = "lblTFaction";
            this.lblTFaction.Size = new System.Drawing.Size(53, 13);
            this.lblTFaction.TabIndex = 25;
            this.lblTFaction.Text = "Unknown";
            // 
            // lblTDist
            // 
            this.lblTDist.AutoSize = true;
            this.lblTDist.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblTDist.Location = new System.Drawing.Point(308, 139);
            this.lblTDist.Name = "lblTDist";
            this.lblTDist.Size = new System.Drawing.Size(22, 13);
            this.lblTDist.TabIndex = 24;
            this.lblTDist.Text = "0.0";
            // 
            // lblTHealth
            // 
            this.lblTHealth.AutoSize = true;
            this.lblTHealth.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblTHealth.Location = new System.Drawing.Point(308, 112);
            this.lblTHealth.Name = "lblTHealth";
            this.lblTHealth.Size = new System.Drawing.Size(13, 13);
            this.lblTHealth.TabIndex = 23;
            this.lblTHealth.Text = "0";
            // 
            // lblTiming
            // 
            this.lblTiming.AutoSize = true;
            this.lblTiming.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblTiming.Location = new System.Drawing.Point(90, 60);
            this.lblTiming.Name = "lblTiming";
            this.lblTiming.Size = new System.Drawing.Size(13, 13);
            this.lblTiming.TabIndex = 22;
            this.lblTiming.Text = "0";
            // 
            // lblAttached
            // 
            this.lblAttached.AutoSize = true;
            this.lblAttached.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblAttached.Location = new System.Drawing.Point(90, 33);
            this.lblAttached.Name = "lblAttached";
            this.lblAttached.Size = new System.Drawing.Size(21, 13);
            this.lblAttached.TabIndex = 21;
            this.lblAttached.Text = "No";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(147, 230);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "XP/Hour:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(278, 230);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "XP:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(223, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Target Faction:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(216, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Target Distance:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(227, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Target Health:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Frame Time:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblRes2
            // 
            this.lblRes2.Location = new System.Drawing.Point(153, 204);
            this.lblRes2.Name = "lblRes2";
            this.lblRes2.Size = new System.Drawing.Size(63, 22);
            this.lblRes2.TabIndex = 13;
            this.lblRes2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblRes1
            // 
            this.lblRes1.Location = new System.Drawing.Point(86, 204);
            this.lblRes1.Name = "lblRes1";
            this.lblRes1.Size = new System.Drawing.Size(63, 22);
            this.lblRes1.TabIndex = 11;
            this.lblRes1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progExp
            // 
            this.progExp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progExp.Location = new System.Drawing.Point(6, 246);
            this.progExp.Name = "progExp";
            this.progExp.Size = new System.Drawing.Size(365, 23);
            this.progExp.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progExp.TabIndex = 9;
            // 
            // lblHealth
            // 
            this.lblHealth.Location = new System.Drawing.Point(20, 204);
            this.lblHealth.Name = "lblHealth";
            this.lblHealth.Size = new System.Drawing.Size(63, 22);
            this.lblHealth.TabIndex = 6;
            this.lblHealth.Text = "Health";
            this.lblHealth.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Attached:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdSettings);
            this.groupBox2.Controls.Add(this.cmdShutdown);
            this.groupBox2.Controls.Add(this.cmdStartStop);
            this.groupBox2.Location = new System.Drawing.Point(12, 292);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(377, 97);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Action";
            // 
            // cmdSettings
            // 
            this.cmdSettings.Image = global::D3Overseer.Properties.Resources.settings;
            this.cmdSettings.Location = new System.Drawing.Point(261, 24);
            this.cmdSettings.Name = "cmdSettings";
            this.cmdSettings.Size = new System.Drawing.Size(64, 56);
            this.cmdSettings.TabIndex = 2;
            this.cmdSettings.Text = "Settings";
            this.cmdSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdSettings.UseVisualStyleBackColor = true;
            // 
            // cmdShutdown
            // 
            this.cmdShutdown.Image = global::D3Overseer.Properties.Resources.power;
            this.cmdShutdown.Location = new System.Drawing.Point(154, 24);
            this.cmdShutdown.Name = "cmdShutdown";
            this.cmdShutdown.Size = new System.Drawing.Size(64, 56);
            this.cmdShutdown.TabIndex = 1;
            this.cmdShutdown.Text = "Shutdown";
            this.cmdShutdown.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdShutdown.UseVisualStyleBackColor = true;
            // 
            // cmdStartStop
            // 
            this.cmdStartStop.Image = global::D3Overseer.Properties.Resources.play;
            this.cmdStartStop.Location = new System.Drawing.Point(46, 24);
            this.cmdStartStop.Name = "cmdStartStop";
            this.cmdStartStop.Size = new System.Drawing.Size(64, 56);
            this.cmdStartStop.TabIndex = 0;
            this.cmdStartStop.Text = "Start";
            this.cmdStartStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdStartStop.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Location = new System.Drawing.Point(12, 402);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.Size = new System.Drawing.Size(377, 183);
            this.txtLog.TabIndex = 2;
            this.txtLog.Text = "";
            // 
            // lblVersion
            // 
            this.lblVersion.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblVersion.Location = new System.Drawing.Point(9, 601);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(380, 16);
            this.lblVersion.TabIndex = 28;
            this.lblVersion.Text = "Overseer 0.0.0 (Build) - Date";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabMap);
            this.tabs.Controls.Add(this.tabInventory);
            this.tabs.Location = new System.Drawing.Point(395, 19);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(430, 598);
            this.tabs.TabIndex = 31;
            // 
            // tabMap
            // 
            this.tabMap.Controls.Add(this.panelMap);
            this.tabMap.Controls.Add(this.picPreview);
            this.tabMap.Location = new System.Drawing.Point(4, 22);
            this.tabMap.Name = "tabMap";
            this.tabMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabMap.Size = new System.Drawing.Size(422, 572);
            this.tabMap.TabIndex = 0;
            this.tabMap.Text = "Map";
            this.tabMap.UseVisualStyleBackColor = true;
            // 
            // panelMap
            // 
            this.panelMap.AutoScroll = true;
            this.panelMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMap.Controls.Add(this.picMap);
            this.panelMap.Location = new System.Drawing.Point(9, 12);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(405, 316);
            this.panelMap.TabIndex = 32;
            // 
            // picMap
            // 
            this.picMap.BackColor = System.Drawing.Color.White;
            this.picMap.Location = new System.Drawing.Point(0, 0);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(512, 512);
            this.picMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picMap.TabIndex = 30;
            this.picMap.TabStop = false;
            this.picMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseUp);
            // 
            // picPreview
            // 
            this.picPreview.BackColor = System.Drawing.Color.White;
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPreview.Location = new System.Drawing.Point(9, 335);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(405, 227);
            this.picPreview.TabIndex = 31;
            this.picPreview.TabStop = false;
            this.picPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.picPreview_Paint);
            this.picPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseDown);
            this.picPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseMove);
            // 
            // tabInventory
            // 
            this.tabInventory.Controls.Add(this.picBackpack);
            this.tabInventory.Location = new System.Drawing.Point(4, 22);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Padding = new System.Windows.Forms.Padding(3);
            this.tabInventory.Size = new System.Drawing.Size(422, 572);
            this.tabInventory.TabIndex = 1;
            this.tabInventory.Text = "Inventory";
            this.tabInventory.UseVisualStyleBackColor = true;
            // 
            // picBackpack
            // 
            this.picBackpack.Location = new System.Drawing.Point(9, 12);
            this.picBackpack.Name = "picBackpack";
            this.picBackpack.Size = new System.Drawing.Size(64, 64);
            this.picBackpack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBackpack.TabIndex = 0;
            this.picBackpack.TabStop = false;
            // 
            // progRes2
            // 
            this.progRes2.Location = new System.Drawing.Point(167, 90);
            this.progRes2.Name = "progRes2";
            this.progRes2.Size = new System.Drawing.Size(36, 111);
            this.progRes2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progRes2.TabIndex = 34;
            // 
            // progRes1
            // 
            this.progRes1.Location = new System.Drawing.Point(100, 90);
            this.progRes1.Name = "progRes1";
            this.progRes1.Size = new System.Drawing.Size(36, 111);
            this.progRes1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progRes1.TabIndex = 33;
            // 
            // progHealth
            // 
            this.progHealth.Location = new System.Drawing.Point(34, 90);
            this.progHealth.Name = "progHealth";
            this.progHealth.Size = new System.Drawing.Size(36, 111);
            this.progHealth.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progHealth.TabIndex = 32;
            // 
            // frmOverseer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 631);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmOverseer";
            this.Text = "Overseer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOverseer_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabMap.ResumeLayout(false);
            this.panelMap.ResumeLayout(false);
            this.panelMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.tabInventory.ResumeLayout(false);
            this.tabInventory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBackpack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblGoldRate;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblGold;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblXP;
        private System.Windows.Forms.Label lblXPRate;
        private System.Windows.Forms.Label lblTFaction;
        private System.Windows.Forms.Label lblTDist;
        private System.Windows.Forms.Label lblTHealth;
        private System.Windows.Forms.Label lblTiming;
        private System.Windows.Forms.Label lblAttached;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblRes2;
        private System.Windows.Forms.Label lblRes1;
        private System.Windows.Forms.ProgressBar progExp;
        private System.Windows.Forms.Label lblHealth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button cmdStartStop;
        private System.Windows.Forms.Button cmdSettings;
        private System.Windows.Forms.Button cmdShutdown;
        private ProgressBarEx progRes2;
        private ProgressBarEx progRes1;
        private ProgressBarEx progHealth;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabMap;
        private System.Windows.Forms.Panel panelMap;
        private System.Windows.Forms.PictureBox picMap;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.TabPage tabInventory;
        private System.Windows.Forms.PictureBox picBackpack;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.Label label9;
    }
}

