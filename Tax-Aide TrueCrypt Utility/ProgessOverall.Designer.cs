namespace TaxAide_TrueCrypt_Utility
{
    partial class ProgessOverall
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgessOverall));
            this.statusText = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.statusSecond = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusText
            // 
            this.statusText.AutoSize = true;
            this.statusText.Location = new System.Drawing.Point(36, 50);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(55, 13);
            this.statusText.TabIndex = 0;
            this.statusText.Text = "Initial Text";
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(36, 13);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(236, 13);
            this.title.TabIndex = 1;
            this.title.Text = "Progress through the tasks shown below";
            // 
            // statusSecond
            // 
            this.statusSecond.AutoSize = true;
            this.statusSecond.Location = new System.Drawing.Point(36, 76);
            this.statusSecond.Name = "statusSecond";
            this.statusSecond.Size = new System.Drawing.Size(31, 13);
            this.statusSecond.TabIndex = 2;
            this.statusSecond.Text = "Initial";
            this.statusSecond.Visible = false;
            // 
            // ProgessOverall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 114);
            this.Controls.Add(this.statusSecond);
            this.Controls.Add(this.title);
            this.Controls.Add(this.statusText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProgessOverall";
            this.Text = "Tax-Aide TrueCrypt Utility";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label statusText;
        private System.Windows.Forms.Label title;
        public System.Windows.Forms.Label statusSecond;
    }
}