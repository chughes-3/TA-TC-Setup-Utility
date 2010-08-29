using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TaxAide_TrueCrypt_Utility
{
    public partial class hdTCSize : Form
    {
        public int hdFileSizeNew;
        public hdTCSize(int oldFileSizeMB)
        {
            InitializeComponent();
            existingSize.Text = oldFileSizeMB.ToString() + " MBytes";
        }

        private void OnEnter(object sender, EventArgs e)
        {
            fileSize.Font = new Font(this.Font, FontStyle.Regular);
            fileSize.ForeColor = SystemColors.WindowText;
            fileSize.Text = "";

        }

        private void but_Cancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void but_OK_Click(object sender, EventArgs e)
        {
            int input;
            if (!Int32.TryParse(fileSize.Text, out input))
            {
                MessageBox.Show("Non Numeric entry in Volume Size\n\n\t" + fileSize.Text, TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                fileSize.Focus();
                return;
            }
            if (gBytes.Checked) { input *= 1000; }
            if (input > 7000)
            {
                if (DialogResult.No == MessageBox.Show("Are you sure you meant this Volume size\n" + input.ToString() + "MB will take a really long time to format", TrueCryptSWObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    fileSize.Focus();
                    return;
                }
            }
            if (input < 0)
            {
                MessageBox.Show("Negative entry in Volume Size\n\n\t" + fileSize.Text, TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                fileSize.Focus();
                return;
            }
            if (input < 5)
            {
                if (DialogResult.No == MessageBox.Show("Are you sure you meant this Volume size\n\n\t" + input.ToString() + "MB is really small??", TrueCryptSWObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    fileSize.Focus();
                    return;
                }
            }
            if (fileSize.ForeColor == SystemColors.GrayText)
            {
                if (DialogResult.No == MessageBox.Show("There has been no entry in the Volume Size Box. Did you intend to use the default of 950 MBytes?", TrueCryptSWObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    fileSize.Focus();
                    return;
                }
            }

            hdFileSizeNew = input;
            Close();
        }
    }
}
