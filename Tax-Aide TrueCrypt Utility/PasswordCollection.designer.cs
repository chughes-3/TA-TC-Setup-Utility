namespace TaxAide_TrueCrypt_Utility
{
    partial class PasswordCollectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordCollectForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.passHDnewP = new System.Windows.Forms.TextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBoxHDnewP = new System.Windows.Forms.CheckBox();
            this.panelHDNewP = new System.Windows.Forms.Panel();
            this.panelHDoldP = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxHDoldP = new System.Windows.Forms.CheckBox();
            this.passHDoldP = new System.Windows.Forms.TextBox();
            this.panelHDoldS = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxSsameP = new System.Windows.Forms.CheckBox();
            this.checkBoxHDoldS = new System.Windows.Forms.CheckBox();
            this.passHDoldS = new System.Windows.Forms.TextBox();
            this.panelHD = new System.Windows.Forms.Panel();
            this.panelTrav = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panelTravOldP = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxTravOldP = new System.Windows.Forms.CheckBox();
            this.passTravOldP = new System.Windows.Forms.TextBox();
            this.panelTravNewP = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxTravnewP = new System.Windows.Forms.CheckBox();
            this.passTravNewP = new System.Windows.Forms.TextBox();
            this.passTravNewPConfirm = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.passHDnewPConfirm = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panelHDNewP.SuspendLayout();
            this.panelHDoldP.SuspendLayout();
            this.panelHDoldS.SuspendLayout();
            this.panelHD.SuspendLayout();
            this.panelTrav.SuspendLayout();
            this.panelTravOldP.SuspendLayout();
            this.panelTravNewP.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "HARD DRIVE TrueCrypt Data File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(281, 26);
            this.label2.TabIndex = 20;
            this.label2.Text = "Type the Password to be used when creating the NEW P \r\nHard Drive TrueCrypt Data " +
                "File";
            // 
            // passHDnewP
            // 
            this.passHDnewP.Location = new System.Drawing.Point(6, 51);
            this.passHDnewP.Name = "passHDnewP";
            this.passHDnewP.Size = new System.Drawing.Size(153, 20);
            this.passHDnewP.TabIndex = 0;
            this.passHDnewP.UseSystemPasswordChar = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(239, 527);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(79, 34);
            this.OKButton.TabIndex = 40;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(44, 527);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 34);
            this.cancelButton.TabIndex = 41;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // checkBoxHDnewP
            // 
            this.checkBoxHDnewP.AutoSize = true;
            this.checkBoxHDnewP.Location = new System.Drawing.Point(181, 51);
            this.checkBoxHDnewP.Name = "checkBoxHDnewP";
            this.checkBoxHDnewP.Size = new System.Drawing.Size(102, 17);
            this.checkBoxHDnewP.TabIndex = 30;
            this.checkBoxHDnewP.Text = "Show Password";
            this.checkBoxHDnewP.UseVisualStyleBackColor = true;
            this.checkBoxHDnewP.CheckedChanged += new System.EventHandler(this.checkBoxPassCheckedChanged);
            // 
            // panelHDNewP
            // 
            this.panelHDNewP.Controls.Add(this.label9);
            this.panelHDNewP.Controls.Add(this.label2);
            this.panelHDNewP.Controls.Add(this.checkBoxHDnewP);
            this.panelHDNewP.Controls.Add(this.passHDnewP);
            this.panelHDNewP.Controls.Add(this.passHDnewPConfirm);
            this.panelHDNewP.Location = new System.Drawing.Point(15, 22);
            this.panelHDNewP.Name = "panelHDNewP";
            this.panelHDNewP.Size = new System.Drawing.Size(332, 103);
            this.panelHDNewP.TabIndex = 22;
            // 
            // panelHDoldP
            // 
            this.panelHDoldP.Controls.Add(this.label3);
            this.panelHDoldP.Controls.Add(this.checkBoxHDoldP);
            this.panelHDoldP.Controls.Add(this.passHDoldP);
            this.panelHDoldP.Location = new System.Drawing.Point(15, 126);
            this.panelHDoldP.Name = "panelHDoldP";
            this.panelHDoldP.Size = new System.Drawing.Size(332, 85);
            this.panelHDoldP.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(317, 39);
            this.label3.TabIndex = 25;
            this.label3.Text = "Type the Password for the OLD P Hard Drive TrueCrypt Data File.\r\nThis will allow " +
                "the automatic move of data from the original Hard \r\nDrive TrueCrypt file to the " +
                "new Hard Drive TrueCrypt file.";
            // 
            // checkBoxHDoldP
            // 
            this.checkBoxHDoldP.AutoSize = true;
            this.checkBoxHDoldP.Location = new System.Drawing.Point(181, 60);
            this.checkBoxHDoldP.Name = "checkBoxHDoldP";
            this.checkBoxHDoldP.Size = new System.Drawing.Size(102, 17);
            this.checkBoxHDoldP.TabIndex = 31;
            this.checkBoxHDoldP.Text = "Show Password";
            this.checkBoxHDoldP.UseVisualStyleBackColor = true;
            this.checkBoxHDoldP.CheckedChanged += new System.EventHandler(this.checkBoxPassCheckedChanged);
            // 
            // passHDoldP
            // 
            this.passHDoldP.Location = new System.Drawing.Point(6, 60);
            this.passHDoldP.Name = "passHDoldP";
            this.passHDoldP.Size = new System.Drawing.Size(153, 20);
            this.passHDoldP.TabIndex = 2;
            this.passHDoldP.UseSystemPasswordChar = true;
            // 
            // panelHDoldS
            // 
            this.panelHDoldS.Controls.Add(this.label4);
            this.panelHDoldS.Controls.Add(this.checkBoxSsameP);
            this.panelHDoldS.Controls.Add(this.checkBoxHDoldS);
            this.panelHDoldS.Controls.Add(this.passHDoldS);
            this.panelHDoldS.Location = new System.Drawing.Point(15, 212);
            this.panelHDoldS.Name = "panelHDoldS";
            this.panelHDoldS.Size = new System.Drawing.Size(332, 84);
            this.panelHDoldS.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(317, 26);
            this.label4.TabIndex = 25;
            this.label4.Text = "Type the Password for the OLD S Hard Drive TrueCrypt Data File.\r\nIn order to allo" +
                "w the automatic data move.";
            // 
            // checkBoxSsameP
            // 
            this.checkBoxSsameP.AutoSize = true;
            this.checkBoxSsameP.Location = new System.Drawing.Point(181, 66);
            this.checkBoxSsameP.Name = "checkBoxSsameP";
            this.checkBoxSsameP.Size = new System.Drawing.Size(102, 17);
            this.checkBoxSsameP.TabIndex = 35;
            this.checkBoxSsameP.Text = "Same as OLD P";
            this.checkBoxSsameP.UseVisualStyleBackColor = true;
            this.checkBoxSsameP.CheckedChanged += new System.EventHandler(this.checkBoxSsameP_CheckedChanged);
            // 
            // checkBoxHDoldS
            // 
            this.checkBoxHDoldS.AutoSize = true;
            this.checkBoxHDoldS.Location = new System.Drawing.Point(181, 44);
            this.checkBoxHDoldS.Name = "checkBoxHDoldS";
            this.checkBoxHDoldS.Size = new System.Drawing.Size(102, 17);
            this.checkBoxHDoldS.TabIndex = 32;
            this.checkBoxHDoldS.Text = "Show Password";
            this.checkBoxHDoldS.UseVisualStyleBackColor = true;
            this.checkBoxHDoldS.CheckedChanged += new System.EventHandler(this.checkBoxPassCheckedChanged);
            // 
            // passHDoldS
            // 
            this.passHDoldS.Location = new System.Drawing.Point(6, 52);
            this.passHDoldS.Name = "passHDoldS";
            this.passHDoldS.Size = new System.Drawing.Size(153, 20);
            this.passHDoldS.TabIndex = 3;
            this.passHDoldS.UseSystemPasswordChar = true;
            // 
            // panelHD
            // 
            this.panelHD.Controls.Add(this.panelHDoldS);
            this.panelHD.Controls.Add(this.label1);
            this.panelHD.Controls.Add(this.panelHDoldP);
            this.panelHD.Controls.Add(this.panelHDNewP);
            this.panelHD.Location = new System.Drawing.Point(3, 4);
            this.panelHD.Name = "panelHD";
            this.panelHD.Size = new System.Drawing.Size(359, 300);
            this.panelHD.TabIndex = 21;
            // 
            // panelTrav
            // 
            this.panelTrav.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelTrav.Controls.Add(this.label6);
            this.panelTrav.Controls.Add(this.panelTravOldP);
            this.panelTrav.Controls.Add(this.panelTravNewP);
            this.panelTrav.Location = new System.Drawing.Point(3, 306);
            this.panelTrav.Name = "panelTrav";
            this.panelTrav.Size = new System.Drawing.Size(359, 214);
            this.panelTrav.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "TRAVELER TrueCrypt Data File";
            // 
            // panelTravOldP
            // 
            this.panelTravOldP.Controls.Add(this.label7);
            this.panelTravOldP.Controls.Add(this.checkBoxTravOldP);
            this.panelTravOldP.Controls.Add(this.passTravOldP);
            this.panelTravOldP.Location = new System.Drawing.Point(15, 121);
            this.panelTravOldP.Name = "panelTravOldP";
            this.panelTravOldP.Size = new System.Drawing.Size(332, 84);
            this.panelTravOldP.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(325, 39);
            this.label7.TabIndex = 25;
            this.label7.Text = "Type the Password for the OLD P TravelerTrueCrypt Data File. This\r\nwill allow the" +
                " automatic move of data from the original Traveler \r\nTrueCrypt file to the new T" +
                "raveler TrueCrypt file.";
            // 
            // checkBoxTravOldP
            // 
            this.checkBoxTravOldP.AutoSize = true;
            this.checkBoxTravOldP.Location = new System.Drawing.Point(181, 58);
            this.checkBoxTravOldP.Name = "checkBoxTravOldP";
            this.checkBoxTravOldP.Size = new System.Drawing.Size(102, 17);
            this.checkBoxTravOldP.TabIndex = 34;
            this.checkBoxTravOldP.Text = "Show Password";
            this.checkBoxTravOldP.UseVisualStyleBackColor = true;
            this.checkBoxTravOldP.CheckedChanged += new System.EventHandler(this.checkBoxPassCheckedChanged);
            // 
            // passTravOldP
            // 
            this.passTravOldP.Location = new System.Drawing.Point(6, 58);
            this.passTravOldP.Name = "passTravOldP";
            this.passTravOldP.Size = new System.Drawing.Size(153, 20);
            this.passTravOldP.TabIndex = 6;
            this.passTravOldP.UseSystemPasswordChar = true;
            // 
            // panelTravNewP
            // 
            this.panelTravNewP.Controls.Add(this.label5);
            this.panelTravNewP.Controls.Add(this.label8);
            this.panelTravNewP.Controls.Add(this.checkBoxTravnewP);
            this.panelTravNewP.Controls.Add(this.passTravNewPConfirm);
            this.panelTravNewP.Controls.Add(this.passTravNewP);
            this.panelTravNewP.Location = new System.Drawing.Point(15, 29);
            this.panelTravNewP.Name = "panelTravNewP";
            this.panelTravNewP.Size = new System.Drawing.Size(332, 89);
            this.panelTravNewP.TabIndex = 26;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(281, 26);
            this.label8.TabIndex = 25;
            this.label8.Text = "Type the Password to be used when creating the NEW P \r\nTraveler TrueCrypt Data Fi" +
                "le";
            // 
            // checkBoxTravnewP
            // 
            this.checkBoxTravnewP.AutoSize = true;
            this.checkBoxTravnewP.Location = new System.Drawing.Point(181, 37);
            this.checkBoxTravnewP.Name = "checkBoxTravnewP";
            this.checkBoxTravnewP.Size = new System.Drawing.Size(102, 17);
            this.checkBoxTravnewP.TabIndex = 33;
            this.checkBoxTravnewP.Text = "Show Password";
            this.checkBoxTravnewP.UseVisualStyleBackColor = true;
            this.checkBoxTravnewP.CheckedChanged += new System.EventHandler(this.checkBoxPassCheckedChanged);
            // 
            // passTravNewP
            // 
            this.passTravNewP.Location = new System.Drawing.Point(6, 37);
            this.passTravNewP.Name = "passTravNewP";
            this.passTravNewP.Size = new System.Drawing.Size(153, 20);
            this.passTravNewP.TabIndex = 4;
            this.passTravNewP.UseSystemPasswordChar = true;
            // 
            // passTravNewPConfirm
            // 
            this.passTravNewPConfirm.Location = new System.Drawing.Point(6, 63);
            this.passTravNewPConfirm.Name = "passTravNewPConfirm";
            this.passTravNewPConfirm.Size = new System.Drawing.Size(153, 20);
            this.passTravNewPConfirm.TabIndex = 5;
            this.passTravNewPConfirm.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(169, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Confirm Password";
            // 
            // passHDnewPConfirm
            // 
            this.passHDnewPConfirm.Location = new System.Drawing.Point(6, 77);
            this.passHDnewPConfirm.Name = "passHDnewPConfirm";
            this.passHDnewPConfirm.Size = new System.Drawing.Size(153, 20);
            this.passHDnewPConfirm.TabIndex = 1;
            this.passHDnewPConfirm.UseSystemPasswordChar = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(169, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 13);
            this.label9.TabIndex = 34;
            this.label9.Text = "Confirm Password";
            // 
            // PasswordCollectForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(363, 569);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.panelTrav);
            this.Controls.Add(this.panelHD);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PasswordCollectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Password collection";
            this.panelHDNewP.ResumeLayout(false);
            this.panelHDNewP.PerformLayout();
            this.panelHDoldP.ResumeLayout(false);
            this.panelHDoldP.PerformLayout();
            this.panelHDoldS.ResumeLayout(false);
            this.panelHDoldS.PerformLayout();
            this.panelHD.ResumeLayout(false);
            this.panelHD.PerformLayout();
            this.panelTrav.ResumeLayout(false);
            this.panelTrav.PerformLayout();
            this.panelTravOldP.ResumeLayout(false);
            this.panelTravOldP.PerformLayout();
            this.panelTravNewP.ResumeLayout(false);
            this.panelTravNewP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBoxHDnewP;
        private System.Windows.Forms.Panel panelHDNewP;
        private System.Windows.Forms.Panel panelHDoldP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxHDoldP;
        private System.Windows.Forms.Panel panelHDoldS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxHDoldS;
        private System.Windows.Forms.CheckBox checkBoxSsameP;
        private System.Windows.Forms.Panel panelHD;
        private System.Windows.Forms.Panel panelTrav;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panelTravOldP;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxTravOldP;
        private System.Windows.Forms.Panel panelTravNewP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxTravnewP;
        internal System.Windows.Forms.TextBox passHDoldS;
        internal System.Windows.Forms.TextBox passHDnewP;
        internal System.Windows.Forms.TextBox passHDoldP;
        internal System.Windows.Forms.TextBox passTravOldP;
        internal System.Windows.Forms.TextBox passTravNewP;
        internal System.Windows.Forms.TextBox passTravNewPConfirm;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        internal System.Windows.Forms.TextBox passHDnewPConfirm;
    }
}

