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
    public partial class MyMessBox : Form
    {
        public MyMessBox()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
