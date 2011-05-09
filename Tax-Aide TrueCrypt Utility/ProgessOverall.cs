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
    public partial class ProgessOverall : Form
    {
        public ProgessOverall()
        {
            InitializeComponent();
        }
        public void ProgShow()
        {
            this.statusText.Text = "Starting Tasklist";
            this.ShowDialog();
        }
        public void ProgUpdate(string updateTxt)
        {
            this.statusText.Text = updateTxt;
            this.Update();
        }
        public void ProgUpdate2(string updateTxt)
        {
            this.statusSecond.Text = updateTxt;
            this.statusSecond.Visible = true;
            this.Update();
        }
        public void ProgUpdLin2notVis()
        {
            this.statusSecond.Visible = false;
            this.Update();
        }
    }
}
