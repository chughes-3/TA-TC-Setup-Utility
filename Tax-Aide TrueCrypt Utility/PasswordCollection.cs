using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TaxAide_TrueCrypt_Utility;

namespace TaxAide_TrueCrypt_Utility
{
    public partial class PasswordCollectForm : Form
    {
        public PasswordCollectForm(TasksBitField tasklist, bool tsdataExists)
        {
            InitializeComponent();
            // setup form panels and size for passwords needed
            if(!tasklist.TestTrav())
            {// No traveler
                this.Height -= panelTrav.Height;  //HD only no traveler
                this.Controls.Remove(panelTrav);
            }
            else if (!tasklist.TestHD())
            {//Traveler only no hard drive
                this.Controls.Remove(panelHD); 
                this.Height -= panelHD.Height;
            }
            if (tasklist.TestTrav() && !tasklist.IsOn(TasksBitField.Flag.travTcfileOldCopy))
            {// we have NO old Trav file therefore no old P
                this.panelTrav.Controls.Remove(panelTravOldP);
                this.panelTrav.Height -= panelTravOldP.Height;
                this.panelTrav.Top += panelTravOldP.Height;
                this.Height -= panelTravOldP.Height;
            }
            if (tasklist.TestHD() && !tasklist.IsOn(TasksBitField.Flag.hdTcfileOldRename))
            {// we have NO old HD files therefore no old P,S
            this.panelHD.Controls.Remove(panelHDoldP); //no HD old p ie just create new P
            this.Height -= panelHDoldP.Height;
            }
            if (tasklist.TestHD() && !tsdataExists)
            {//no HD old S drive
                this.panelHD.Controls.Remove(panelHDoldS); 
                this.Height -= panelHDoldS.Height;
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
        private void checkBoxPassCheckedChanged(object sender, EventArgs e)
        {//Shows or Hides passwords
            CheckBox chkbx = (CheckBox)sender;
            foreach (Control item in chkbx.Parent.Controls)
            {
                if (item.GetType() == typeof(TextBox))
                {
                    TextBox txtbx = (TextBox)item;
                    if (chkbx.Checked == true)
                        txtbx.UseSystemPasswordChar = false;
                    else
                        txtbx.UseSystemPasswordChar = true;
                }
            }
        }
        private void checkBoxSsameP_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkbx = (CheckBox)sender;
            foreach (Control item in chkbx.Parent.Controls)
            {
                if (item.GetType() == typeof(TextBox))
                {
                    TextBox txtbx = (TextBox)item;
                    if (chkbx.Checked == true)
                        passHDoldS.Text = passHDoldP.Text;
                    else
                        passHDoldS.Text = string.Empty;
                }
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            //Store entered passwords in appropriate objects
            return;
        }
    }
}
