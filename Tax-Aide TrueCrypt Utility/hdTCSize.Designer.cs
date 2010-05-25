namespace TaxAide_TrueCrypt_Utility
{
    partial class hdTCSize
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
            this.Title = new System.Windows.Forms.Label();
            this.existingSize = new System.Windows.Forms.Label();
            this.fileSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mBytes = new System.Windows.Forms.RadioButton();
            this.gBytes = new System.Windows.Forms.RadioButton();
            this.but_Cancel = new System.Windows.Forms.Button();
            this.but_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(12, 21);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(228, 39);
            this.Title.TabIndex = 0;
            this.Title.Text = "Enter the Volume size for the upgraded\r\nhard drive TrueCrypt Volume. \r\nThe size o" +
                "f the existing volume is \r\n";
            // 
            // existingSize
            // 
            this.existingSize.AutoSize = true;
            this.existingSize.Location = new System.Drawing.Point(104, 69);
            this.existingSize.Name = "existingSize";
            this.existingSize.Size = new System.Drawing.Size(23, 13);
            this.existingSize.TabIndex = 1;
            this.existingSize.Text = "MB";
            // 
            // fileSize
            // 
            this.fileSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileSize.ForeColor = System.Drawing.SystemColors.GrayText;
            this.fileSize.Location = new System.Drawing.Point(41, 126);
            this.fileSize.Name = "fileSize";
            this.fileSize.Size = new System.Drawing.Size(60, 20);
            this.fileSize.TabIndex = 2;
            this.fileSize.Text = "950 ";
            this.fileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.fileSize.Enter += new System.EventHandler(this.OnEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "New Size";
            // 
            // mBytes
            // 
            this.mBytes.AutoSize = true;
            this.mBytes.Checked = true;
            this.mBytes.Location = new System.Drawing.Point(118, 128);
            this.mBytes.Name = "mBytes";
            this.mBytes.Size = new System.Drawing.Size(59, 17);
            this.mBytes.TabIndex = 4;
            this.mBytes.TabStop = true;
            this.mBytes.Text = "Mbytes";
            this.mBytes.UseVisualStyleBackColor = true;
            // 
            // gBytes
            // 
            this.gBytes.AutoSize = true;
            this.gBytes.Location = new System.Drawing.Point(183, 128);
            this.gBytes.Name = "gBytes";
            this.gBytes.Size = new System.Drawing.Size(58, 17);
            this.gBytes.TabIndex = 5;
            this.gBytes.Text = "Gbytes";
            this.gBytes.UseVisualStyleBackColor = true;
            // 
            // but_Cancel
            // 
            this.but_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.but_Cancel.Location = new System.Drawing.Point(32, 163);
            this.but_Cancel.Name = "but_Cancel";
            this.but_Cancel.Size = new System.Drawing.Size(69, 23);
            this.but_Cancel.TabIndex = 6;
            this.but_Cancel.Text = "Cancel";
            this.but_Cancel.UseVisualStyleBackColor = true;
            this.but_Cancel.Click += new System.EventHandler(this.but_Cancel_Click);
            // 
            // but_OK
            // 
            this.but_OK.Location = new System.Drawing.Point(173, 163);
            this.but_OK.Name = "but_OK";
            this.but_OK.Size = new System.Drawing.Size(76, 23);
            this.but_OK.TabIndex = 7;
            this.but_OK.Text = "OK";
            this.but_OK.UseVisualStyleBackColor = true;
            this.but_OK.Click += new System.EventHandler(this.but_OK_Click);
            // 
            // hdTCSize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 200);
            this.Controls.Add(this.but_OK);
            this.Controls.Add(this.but_Cancel);
            this.Controls.Add(this.gBytes);
            this.Controls.Add(this.mBytes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fileSize);
            this.Controls.Add(this.existingSize);
            this.Controls.Add(this.Title);
            this.Name = "hdTCSize";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tax-Aide TrueCrypt utility";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label existingSize;
        private System.Windows.Forms.TextBox fileSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton mBytes;
        private System.Windows.Forms.RadioButton gBytes;
        private System.Windows.Forms.Button but_Cancel;
        private System.Windows.Forms.Button but_OK;
    }
}