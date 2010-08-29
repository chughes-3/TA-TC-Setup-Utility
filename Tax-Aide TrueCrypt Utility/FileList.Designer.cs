namespace TaxAide_TrueCrypt_Utility
{
    partial class FileList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileList));
            this.taskChoice1 = new System.Windows.Forms.RadioButton();
            this.taskChoice2 = new System.Windows.Forms.RadioButton();
            this.taskChoice3 = new System.Windows.Forms.RadioButton();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.mainLabel = new System.Windows.Forms.Label();
            this.newFileSizeMB = new System.Windows.Forms.TextBox();
            this.gBytes = new System.Windows.Forms.RadioButton();
            this.mBytes = new System.Windows.Forms.RadioButton();
            this.labelGrpBox = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelVolSize = new System.Windows.Forms.Label();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.usbSelectionComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskChoice1
            // 
            this.taskChoice1.AutoSize = true;
            this.taskChoice1.Location = new System.Drawing.Point(22, 59);
            this.taskChoice1.Name = "taskChoice1";
            this.taskChoice1.Size = new System.Drawing.Size(84, 17);
            this.taskChoice1.TabIndex = 1;
            this.taskChoice1.TabStop = true;
            this.taskChoice1.Text = "taskChoice1";
            this.taskChoice1.UseVisualStyleBackColor = true;
            this.taskChoice1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // taskChoice2
            // 
            this.taskChoice2.AutoSize = true;
            this.taskChoice2.Location = new System.Drawing.Point(22, 82);
            this.taskChoice2.Name = "taskChoice2";
            this.taskChoice2.Size = new System.Drawing.Size(84, 17);
            this.taskChoice2.TabIndex = 2;
            this.taskChoice2.TabStop = true;
            this.taskChoice2.Text = "taskChoice2";
            this.taskChoice2.UseVisualStyleBackColor = true;
            // 
            // taskChoice3
            // 
            this.taskChoice3.AutoSize = true;
            this.taskChoice3.Location = new System.Drawing.Point(22, 105);
            this.taskChoice3.Name = "taskChoice3";
            this.taskChoice3.Size = new System.Drawing.Size(84, 17);
            this.taskChoice3.TabIndex = 3;
            this.taskChoice3.TabStop = true;
            this.taskChoice3.Text = "taskChoice3";
            this.taskChoice3.UseVisualStyleBackColor = true;
            this.taskChoice3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.Location = new System.Drawing.Point(260, 236);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 5;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(53, 236);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 4;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // mainLabel
            // 
            this.mainLabel.AutoSize = true;
            this.mainLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainLabel.Location = new System.Drawing.Point(6, 16);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(326, 13);
            this.mainLabel.TabIndex = 12;
            this.mainLabel.Text = "Select a task, and if necessary a Drive and Volume Size";
            // 
            // newFileSizeMB
            // 
            this.newFileSizeMB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newFileSizeMB.ForeColor = System.Drawing.SystemColors.GrayText;
            this.newFileSizeMB.Location = new System.Drawing.Point(131, 73);
            this.newFileSizeMB.Name = "newFileSizeMB";
            this.newFileSizeMB.Size = new System.Drawing.Size(54, 20);
            this.newFileSizeMB.TabIndex = 6;
            this.newFileSizeMB.Text = "950 ";
            this.newFileSizeMB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.newFileSizeMB.Enter += new System.EventHandler(this.Edit_Entry);
            // 
            // gBytes
            // 
            this.gBytes.AutoSize = true;
            this.gBytes.Location = new System.Drawing.Point(264, 74);
            this.gBytes.Name = "gBytes";
            this.gBytes.Size = new System.Drawing.Size(59, 17);
            this.gBytes.TabIndex = 8;
            this.gBytes.Text = "GBytes";
            this.gBytes.UseVisualStyleBackColor = true;
            // 
            // mBytes
            // 
            this.mBytes.AutoSize = true;
            this.mBytes.Checked = true;
            this.mBytes.Location = new System.Drawing.Point(197, 74);
            this.mBytes.Name = "mBytes";
            this.mBytes.Size = new System.Drawing.Size(60, 17);
            this.mBytes.TabIndex = 7;
            this.mBytes.TabStop = true;
            this.mBytes.Text = "MBytes";
            this.mBytes.UseVisualStyleBackColor = true;
            // 
            // labelGrpBox
            // 
            this.labelGrpBox.AutoSize = true;
            this.labelGrpBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGrpBox.Location = new System.Drawing.Point(113, 14);
            this.labelGrpBox.Name = "labelGrpBox";
            this.labelGrpBox.Size = new System.Drawing.Size(145, 16);
            this.labelGrpBox.TabIndex = 9;
            this.labelGrpBox.Text = "TrueCrypt Volume Size";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.labelVolSize);
            this.groupBox2.Controls.Add(this.sizeLabel);
            this.groupBox2.Controls.Add(this.gBytes);
            this.groupBox2.Controls.Add(this.labelGrpBox);
            this.groupBox2.Controls.Add(this.mBytes);
            this.groupBox2.Controls.Add(this.newFileSizeMB);
            this.groupBox2.Location = new System.Drawing.Point(12, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(382, 104);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            // 
            // labelVolSize
            // 
            this.labelVolSize.AutoSize = true;
            this.labelVolSize.Location = new System.Drawing.Point(38, 77);
            this.labelVolSize.Name = "labelVolSize";
            this.labelVolSize.Size = new System.Drawing.Size(90, 13);
            this.labelVolSize.TabIndex = 11;
            this.labelVolSize.Text = "New Volume Size";
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(7, 45);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(359, 13);
            this.sizeLabel.TabIndex = 10;
            this.sizeLabel.Text = "C: has xxxMB of Available Space with an existing yyyMB TrueCrypt Volume";
            this.sizeLabel.Visible = false;
            // 
            // usbSelectionComboBox
            // 
            this.usbSelectionComboBox.AllowDrop = true;
            this.usbSelectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.usbSelectionComboBox.FormattingEnabled = true;
            this.usbSelectionComboBox.Items.AddRange(new object[] {
            "Choose the USB Drive"});
            this.usbSelectionComboBox.Location = new System.Drawing.Point(209, 35);
            this.usbSelectionComboBox.Name = "usbSelectionComboBox";
            this.usbSelectionComboBox.Size = new System.Drawing.Size(195, 21);
            this.usbSelectionComboBox.TabIndex = 13;
            this.usbSelectionComboBox.Visible = false;
            this.usbSelectionComboBox.SelectedIndexChanged += new System.EventHandler(this.usbSelectionComboBox_SelectedIndexChanged);
            // 
            // FileList
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(406, 274);
            this.Controls.Add(this.usbSelectionComboBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mainLabel);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.taskChoice3);
            this.Controls.Add(this.taskChoice2);
            this.Controls.Add(this.taskChoice1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FileList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AARP Tax-Aide TrueCrypt Utilities";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton taskChoice1;
        private System.Windows.Forms.RadioButton taskChoice2;
        private System.Windows.Forms.RadioButton taskChoice3;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.TextBox newFileSizeMB;
        private System.Windows.Forms.RadioButton gBytes;
        private System.Windows.Forms.RadioButton mBytes;
        private System.Windows.Forms.Label labelGrpBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.Label labelVolSize;
        private System.Windows.Forms.ComboBox usbSelectionComboBox;
    }
}

