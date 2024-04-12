namespace TrunkScannerCPS
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnLoadCodeplug = new System.Windows.Forms.Button();
            this.txtChannelName = new System.Windows.Forms.TextBox();
            this.txtTgid = new System.Windows.Forms.TextBox();
            this.btnAddChannel = new System.Windows.Forms.Button();
            this.btnDeleteChannel = new System.Windows.Forms.Button();
            this.btnSaveChannel = new System.Windows.Forms.Button();
            this.btnAddZone = new System.Windows.Forms.Button();
            this.btnDeleteZone = new System.Windows.Forms.Button();
            this.btnSaveCodeplug = new System.Windows.Forms.Button();
            this.txtZoneName = new System.Windows.Forms.TextBox();
            this.btnSaveZoneName = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lastProgramSrcBox = new System.Windows.Forms.TextBox();
            this.codeplugVersionBox = new System.Windows.Forms.TextBox();
            this.modelBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.serialNumberBox = new System.Windows.Forms.TextBox();
            this.btnRenameScanList = new System.Windows.Forms.Button();
            this.btnAddChannelToScanList = new System.Windows.Forms.Button();
            this.btnRemoveChannelFromScanList = new System.Windows.Forms.Button();
            this.txtScanListName = new System.Windows.Forms.TextBox();
            this.cmbScanList = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbTtsEnabled = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbChannels = new System.Windows.Forms.ComboBox();
            this.btnAddScanList = new System.Windows.Forms.Button();
            this.btnDeleteScanList = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(181, 426);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // btnLoadCodeplug
            // 
            this.btnLoadCodeplug.Location = new System.Drawing.Point(654, 9);
            this.btnLoadCodeplug.Name = "btnLoadCodeplug";
            this.btnLoadCodeplug.Size = new System.Drawing.Size(134, 23);
            this.btnLoadCodeplug.TabIndex = 1;
            this.btnLoadCodeplug.Text = "Load Codeplug";
            this.btnLoadCodeplug.UseVisualStyleBackColor = true;
            this.btnLoadCodeplug.Click += new System.EventHandler(this.btnLoadCodeplug_Click);
            // 
            // txtChannelName
            // 
            this.txtChannelName.Location = new System.Drawing.Point(63, 18);
            this.txtChannelName.Name = "txtChannelName";
            this.txtChannelName.Size = new System.Drawing.Size(117, 20);
            this.txtChannelName.TabIndex = 2;
            // 
            // txtTgid
            // 
            this.txtTgid.Location = new System.Drawing.Point(63, 40);
            this.txtTgid.Name = "txtTgid";
            this.txtTgid.Size = new System.Drawing.Size(117, 20);
            this.txtTgid.TabIndex = 3;
            // 
            // btnAddChannel
            // 
            this.btnAddChannel.Location = new System.Drawing.Point(28, 98);
            this.btnAddChannel.Name = "btnAddChannel";
            this.btnAddChannel.Size = new System.Drawing.Size(152, 23);
            this.btnAddChannel.TabIndex = 4;
            this.btnAddChannel.Text = "Add Channel";
            this.btnAddChannel.UseVisualStyleBackColor = true;
            this.btnAddChannel.Click += new System.EventHandler(this.btnAddChannel_Click);
            // 
            // btnDeleteChannel
            // 
            this.btnDeleteChannel.Location = new System.Drawing.Point(28, 127);
            this.btnDeleteChannel.Name = "btnDeleteChannel";
            this.btnDeleteChannel.Size = new System.Drawing.Size(152, 23);
            this.btnDeleteChannel.TabIndex = 5;
            this.btnDeleteChannel.Text = "Delete Channel";
            this.btnDeleteChannel.UseVisualStyleBackColor = true;
            this.btnDeleteChannel.Click += new System.EventHandler(this.btnDeleteChannel_Click);
            // 
            // btnSaveChannel
            // 
            this.btnSaveChannel.Location = new System.Drawing.Point(28, 69);
            this.btnSaveChannel.Name = "btnSaveChannel";
            this.btnSaveChannel.Size = new System.Drawing.Size(152, 23);
            this.btnSaveChannel.TabIndex = 6;
            this.btnSaveChannel.Text = "Save Channel";
            this.btnSaveChannel.UseVisualStyleBackColor = true;
            this.btnSaveChannel.Click += new System.EventHandler(this.btnSaveChannel_Click);
            // 
            // btnAddZone
            // 
            this.btnAddZone.Location = new System.Drawing.Point(6, 98);
            this.btnAddZone.Name = "btnAddZone";
            this.btnAddZone.Size = new System.Drawing.Size(63, 23);
            this.btnAddZone.TabIndex = 7;
            this.btnAddZone.Text = "Add Zone";
            this.btnAddZone.UseVisualStyleBackColor = true;
            this.btnAddZone.Click += new System.EventHandler(this.btnAddZone_Click);
            // 
            // btnDeleteZone
            // 
            this.btnDeleteZone.Location = new System.Drawing.Point(75, 98);
            this.btnDeleteZone.Name = "btnDeleteZone";
            this.btnDeleteZone.Size = new System.Drawing.Size(101, 23);
            this.btnDeleteZone.TabIndex = 8;
            this.btnDeleteZone.Text = "Delete Zone";
            this.btnDeleteZone.UseVisualStyleBackColor = true;
            this.btnDeleteZone.Click += new System.EventHandler(this.btnDeleteZone_Click);
            // 
            // btnSaveCodeplug
            // 
            this.btnSaveCodeplug.Location = new System.Drawing.Point(654, 40);
            this.btnSaveCodeplug.Name = "btnSaveCodeplug";
            this.btnSaveCodeplug.Size = new System.Drawing.Size(134, 23);
            this.btnSaveCodeplug.TabIndex = 9;
            this.btnSaveCodeplug.Text = "Save Codeplug";
            this.btnSaveCodeplug.UseVisualStyleBackColor = true;
            this.btnSaveCodeplug.Click += new System.EventHandler(this.btnSaveCodeplug_Click);
            // 
            // txtZoneName
            // 
            this.txtZoneName.Location = new System.Drawing.Point(57, 19);
            this.txtZoneName.Name = "txtZoneName";
            this.txtZoneName.Size = new System.Drawing.Size(117, 20);
            this.txtZoneName.TabIndex = 10;
            // 
            // btnSaveZoneName
            // 
            this.btnSaveZoneName.Location = new System.Drawing.Point(9, 69);
            this.btnSaveZoneName.Name = "btnSaveZoneName";
            this.btnSaveZoneName.Size = new System.Drawing.Size(167, 23);
            this.btnSaveZoneName.TabIndex = 11;
            this.btnSaveZoneName.Text = "Save Zone";
            this.btnSaveZoneName.UseVisualStyleBackColor = true;
            this.btnSaveZoneName.Click += new System.EventHandler(this.btnSaveZoneName_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "TGID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lastProgramSrcBox);
            this.groupBox1.Controls.Add(this.codeplugVersionBox);
            this.groupBox1.Controls.Add(this.modelBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.serialNumberBox);
            this.groupBox1.Location = new System.Drawing.Point(597, 175);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 263);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Last Program:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(40, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Version:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(46, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Model:";
            // 
            // lastProgramSrcBox
            // 
            this.lastProgramSrcBox.Location = new System.Drawing.Point(91, 97);
            this.lastProgramSrcBox.Name = "lastProgramSrcBox";
            this.lastProgramSrcBox.Size = new System.Drawing.Size(100, 20);
            this.lastProgramSrcBox.TabIndex = 4;
            // 
            // codeplugVersionBox
            // 
            this.codeplugVersionBox.Location = new System.Drawing.Point(91, 71);
            this.codeplugVersionBox.Name = "codeplugVersionBox";
            this.codeplugVersionBox.Size = new System.Drawing.Size(100, 20);
            this.codeplugVersionBox.TabIndex = 3;
            // 
            // modelBox
            // 
            this.modelBox.Location = new System.Drawing.Point(91, 45);
            this.modelBox.Name = "modelBox";
            this.modelBox.Size = new System.Drawing.Size(100, 20);
            this.modelBox.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(49, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Serial:";
            // 
            // serialNumberBox
            // 
            this.serialNumberBox.Location = new System.Drawing.Point(91, 19);
            this.serialNumberBox.Name = "serialNumberBox";
            this.serialNumberBox.Size = new System.Drawing.Size(100, 20);
            this.serialNumberBox.TabIndex = 0;
            // 
            // btnRenameScanList
            // 
            this.btnRenameScanList.Location = new System.Drawing.Point(24, 122);
            this.btnRenameScanList.Name = "btnRenameScanList";
            this.btnRenameScanList.Size = new System.Drawing.Size(117, 23);
            this.btnRenameScanList.TabIndex = 16;
            this.btnRenameScanList.Text = "Rename List";
            this.btnRenameScanList.UseVisualStyleBackColor = true;
            this.btnRenameScanList.Click += new System.EventHandler(this.btnRenameScanList_Click);
            // 
            // btnAddChannelToScanList
            // 
            this.btnAddChannelToScanList.Location = new System.Drawing.Point(6, 42);
            this.btnAddChannelToScanList.Name = "btnAddChannelToScanList";
            this.btnAddChannelToScanList.Size = new System.Drawing.Size(75, 23);
            this.btnAddChannelToScanList.TabIndex = 17;
            this.btnAddChannelToScanList.Text = "Add to List";
            this.btnAddChannelToScanList.UseVisualStyleBackColor = true;
            this.btnAddChannelToScanList.Click += new System.EventHandler(this.btnAddChannelToScanList_Click);
            // 
            // btnRemoveChannelFromScanList
            // 
            this.btnRemoveChannelFromScanList.Location = new System.Drawing.Point(87, 42);
            this.btnRemoveChannelFromScanList.Name = "btnRemoveChannelFromScanList";
            this.btnRemoveChannelFromScanList.Size = new System.Drawing.Size(99, 23);
            this.btnRemoveChannelFromScanList.TabIndex = 18;
            this.btnRemoveChannelFromScanList.Text = "Delete from List";
            this.btnRemoveChannelFromScanList.UseVisualStyleBackColor = true;
            this.btnRemoveChannelFromScanList.Click += new System.EventHandler(this.btnRemoveChannelFromScanList_Click);
            // 
            // txtScanListName
            // 
            this.txtScanListName.Location = new System.Drawing.Point(66, 98);
            this.txtScanListName.Name = "txtScanListName";
            this.txtScanListName.Size = new System.Drawing.Size(117, 20);
            this.txtScanListName.TabIndex = 19;
            // 
            // cmbScanList
            // 
            this.cmbScanList.FormattingEnabled = true;
            this.cmbScanList.Location = new System.Drawing.Point(57, 45);
            this.cmbScanList.Name = "cmbScanList";
            this.cmbScanList.Size = new System.Drawing.Size(117, 21);
            this.cmbScanList.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Scan List:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "List Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.cmbTtsEnabled);
            this.groupBox2.Location = new System.Drawing.Point(391, 175);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 263);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // cmbTtsEnabled
            // 
            this.cmbTtsEnabled.FormattingEnabled = true;
            this.cmbTtsEnabled.Location = new System.Drawing.Point(79, 22);
            this.cmbTtsEnabled.Name = "cmbTtsEnabled";
            this.cmbTtsEnabled.Size = new System.Drawing.Size(121, 21);
            this.cmbTtsEnabled.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "TTS Enabled:";
            // 
            // cmbChannels
            // 
            this.cmbChannels.FormattingEnabled = true;
            this.cmbChannels.Location = new System.Drawing.Point(6, 15);
            this.cmbChannels.Name = "cmbChannels";
            this.cmbChannels.Size = new System.Drawing.Size(156, 21);
            this.cmbChannels.TabIndex = 24;
            // 
            // btnAddScanList
            // 
            this.btnAddScanList.Location = new System.Drawing.Point(6, 71);
            this.btnAddScanList.Name = "btnAddScanList";
            this.btnAddScanList.Size = new System.Drawing.Size(75, 23);
            this.btnAddScanList.TabIndex = 25;
            this.btnAddScanList.Text = "Add List";
            this.btnAddScanList.UseVisualStyleBackColor = true;
            this.btnAddScanList.Click += new System.EventHandler(this.btnAddScanList_Click);
            // 
            // btnDeleteScanList
            // 
            this.btnDeleteScanList.Location = new System.Drawing.Point(87, 71);
            this.btnDeleteScanList.Name = "btnDeleteScanList";
            this.btnDeleteScanList.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteScanList.TabIndex = 26;
            this.btnDeleteScanList.Text = "Delete List";
            this.btnDeleteScanList.UseVisualStyleBackColor = true;
            this.btnDeleteScanList.Click += new System.EventHandler(this.btnDeleteScanList_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnAddScanList);
            this.groupBox3.Controls.Add(this.cmbChannels);
            this.groupBox3.Controls.Add(this.btnDeleteScanList);
            this.groupBox3.Controls.Add(this.btnRenameScanList);
            this.groupBox3.Controls.Add(this.txtScanListName);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.btnAddChannelToScanList);
            this.groupBox3.Controls.Add(this.btnRemoveChannelFromScanList);
            this.groupBox3.Location = new System.Drawing.Point(199, 293);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(186, 145);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Scan";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtChannelName);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txtTgid);
            this.groupBox4.Controls.Add(this.btnSaveChannel);
            this.groupBox4.Controls.Add(this.btnDeleteChannel);
            this.groupBox4.Controls.Add(this.btnAddChannel);
            this.groupBox4.Location = new System.Drawing.Point(405, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(195, 157);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Channel";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtZoneName);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.btnSaveZoneName);
            this.groupBox5.Controls.Add(this.btnAddZone);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.btnDeleteZone);
            this.groupBox5.Controls.Add(this.cmbScanList);
            this.groupBox5.Location = new System.Drawing.Point(199, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(200, 140);
            this.groupBox5.TabIndex = 28;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Zone";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSaveCodeplug);
            this.Controls.Add(this.btnLoadCodeplug);
            this.Controls.Add(this.treeView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btnLoadCodeplug;
        private System.Windows.Forms.TextBox txtChannelName;
        private System.Windows.Forms.TextBox txtTgid;
        private System.Windows.Forms.Button btnAddChannel;
        private System.Windows.Forms.Button btnDeleteChannel;
        private System.Windows.Forms.Button btnSaveChannel;
        private System.Windows.Forms.Button btnAddZone;
        private System.Windows.Forms.Button btnDeleteZone;
        private System.Windows.Forms.Button btnSaveCodeplug;
        private System.Windows.Forms.TextBox txtZoneName;
        private System.Windows.Forms.Button btnSaveZoneName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox serialNumberBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox lastProgramSrcBox;
        private System.Windows.Forms.TextBox codeplugVersionBox;
        private System.Windows.Forms.TextBox modelBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRenameScanList;
        private System.Windows.Forms.Button btnAddChannelToScanList;
        private System.Windows.Forms.Button btnRemoveChannelFromScanList;
        private System.Windows.Forms.TextBox txtScanListName;
        private System.Windows.Forms.ComboBox cmbScanList;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbTtsEnabled;
        private System.Windows.Forms.ComboBox cmbChannels;
        private System.Windows.Forms.Button btnAddScanList;
        private System.Windows.Forms.Button btnDeleteScanList;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}

