using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TaxAide_TrueCrypt_Utility
{
    public partial class ProgressForm : Form
    {
        public ProgressForm(string message)
        {
            InitializeComponent();
            labelFileCop.Text = message;
        }
        public void DoProgressStep()
        {
            progressBar1.PerformStep();
            this.Update();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
