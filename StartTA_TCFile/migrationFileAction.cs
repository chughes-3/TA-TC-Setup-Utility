using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StartTA_TCFile
{
    public partial class migrationFileActionForm : Form
    {
        public migrationFileActionForm()
        {
            InitializeComponent();           
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (OpnPDrv.Checked)
            {//just open the sucker
                DialogResult = DialogResult.OK;
                return;
            }
            if (delNewPStartTAutil.Checked)
            {// restart process
                DialogResult = DialogResult.Retry;
                return;
            }
            if (delOldTCFiles.Checked)
            {// delete old file, open P drive
                DialogResult = DialogResult.Yes;
                return;
            }
            else
                Environment.Exit(1);
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void migrationFileActionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("closed = " + e.CloseReason.ToString());
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Environment.Exit(1); 
            }
        }

    }
}
