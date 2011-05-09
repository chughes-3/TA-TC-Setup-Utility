namespace TaxAide_TrueCrypt_Utility
{
    partial class migrationFileActionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(migrationFileActionForm));
            this.delInitialExplain = new System.Windows.Forms.Label();
            this.CancelBut = new System.Windows.Forms.Button();
            this.radBtGrpBox = new System.Windows.Forms.GroupBox();
            this.CancelRadBut = new System.Windows.Forms.RadioButton();
            this.delOldTCFiles = new System.Windows.Forms.RadioButton();
            this.delNewPStartTAutil = new System.Windows.Forms.RadioButton();
            this.OKButton = new System.Windows.Forms.Button();
            this.radBtGrpBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // delInitialExplain
            // 
            this.delInitialExplain.AutoSize = true;
            this.delInitialExplain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delInitialExplain.Location = new System.Drawing.Point(21, 13);
            this.delInitialExplain.Name = "delInitialExplain";
            this.delInitialExplain.Size = new System.Drawing.Size(361, 60);
            this.delInitialExplain.TabIndex = 0;
            this.delInitialExplain.Text = resources.GetString("delInitialExplain.Text");
            // 
            // CancelBut
            // 
            this.CancelBut.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBut.Location = new System.Drawing.Point(58, 253);
            this.CancelBut.Name = "CancelBut";
            this.CancelBut.Size = new System.Drawing.Size(100, 37);
            this.CancelBut.TabIndex = 1;
            this.CancelBut.Text = "Cancel";
            this.CancelBut.UseVisualStyleBackColor = true;
            this.CancelBut.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // radBtGrpBox
            // 
            this.radBtGrpBox.Controls.Add(this.CancelRadBut);
            this.radBtGrpBox.Controls.Add(this.delOldTCFiles);
            this.radBtGrpBox.Controls.Add(this.delNewPStartTAutil);
            this.radBtGrpBox.Location = new System.Drawing.Point(16, 87);
            this.radBtGrpBox.Name = "radBtGrpBox";
            this.radBtGrpBox.Size = new System.Drawing.Size(376, 146);
            this.radBtGrpBox.TabIndex = 2;
            this.radBtGrpBox.TabStop = false;
            this.radBtGrpBox.Text = "Choose an Alternative";
            // 
            // CancelRadBut
            // 
            this.CancelRadBut.AutoSize = true;
            this.CancelRadBut.Checked = true;
            this.CancelRadBut.Location = new System.Drawing.Point(6, 121);
            this.CancelRadBut.Name = "CancelRadBut";
            this.CancelRadBut.Size = new System.Drawing.Size(148, 17);
            this.CancelRadBut.TabIndex = 0;
            this.CancelRadBut.TabStop = true;
            this.CancelRadBut.Text = "Cancel out of this program";
            this.CancelRadBut.UseVisualStyleBackColor = true;
            // 
            // delOldTCFiles
            // 
            this.delOldTCFiles.AutoSize = true;
            this.delOldTCFiles.Location = new System.Drawing.Point(7, 62);
            this.delOldTCFiles.Name = "delOldTCFiles";
            this.delOldTCFiles.Size = new System.Drawing.Size(315, 43);
            this.delOldTCFiles.TabIndex = 0;
            this.delOldTCFiles.Text = "Delete the old TrueCrypt files, Assumes data migration is done\r\ncorrectly, and ol" +
                "d files are not needed. Warning - they will be\r\npermanently deleted. Then contin" +
                "ue the utility.";
            this.delOldTCFiles.UseVisualStyleBackColor = true;
            // 
            // delNewPStartTAutil
            // 
            this.delNewPStartTAutil.AutoSize = true;
            this.delNewPStartTAutil.Location = new System.Drawing.Point(7, 19);
            this.delNewPStartTAutil.Name = "delNewPStartTAutil";
            this.delNewPStartTAutil.Size = new System.Drawing.Size(346, 30);
            this.delNewPStartTAutil.TabIndex = 0;
            this.delNewPStartTAutil.TabStop = true;
            this.delNewPStartTAutil.Text = "Delete the current TPDATA file. Continue the Tax-Aide Utility \r\nto create a new T" +
                "PDATA file and complete the migration that failed. \r\n";
            this.delNewPStartTAutil.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(238, 253);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(100, 37);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // migrationFileActionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 308);
            this.Controls.Add(this.radBtGrpBox);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelBut);
            this.Controls.Add(this.delInitialExplain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "migrationFileActionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AARP Tax-Aide TrueCrypt Utility";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.migrationFileActionForm_FormClosed);
            this.radBtGrpBox.ResumeLayout(false);
            this.radBtGrpBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CancelBut;
        private System.Windows.Forms.RadioButton CancelRadBut;
        private System.Windows.Forms.Button OKButton;
        internal System.Windows.Forms.RadioButton delNewPStartTAutil;
        internal System.Windows.Forms.RadioButton delOldTCFiles;
        internal System.Windows.Forms.Label delInitialExplain;
        internal System.Windows.Forms.GroupBox radBtGrpBox;
    }
}

