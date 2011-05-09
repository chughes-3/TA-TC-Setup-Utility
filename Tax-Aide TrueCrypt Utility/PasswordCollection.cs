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
        public PasswordCollectForm(TasksBitField tasklist, bool oldTravExists, bool oldHDExists, bool tsdataExists)
        {
            InitializeComponent();
            // setup form panels and size for passwords needed
            this.ActiveControl = this.passHDnewP;
            if(!tasklist.TestTrav())
            {// No traveler
                this.Height -= panelTrav.Height;  //HD only no traveler
                this.Controls.Remove(panelTrav);
            }
            else if (!tasklist.TestHD())
            {//Traveler only no hard drive
                this.Controls.Remove(panelHD); 
                this.Height -= panelHD.Height;
                this.ActiveControl = this.passTravNewP;
            }
            if (tasklist.TestTrav() && !(tasklist.IsOn(TasksBitField.Flag.travTcfileOldCopy)|| oldTravExists))
            {// we have NO old Trav file therefore no old P
                this.panelTrav.Controls.Remove(panelTravOldP);
                this.panelTrav.Height -= panelTravOldP.Height;
                this.panelTrav.Top += panelTravOldP.Height;
                this.Height -= panelTravOldP.Height;
            }
            if (tasklist.TestHD() && !(tasklist.IsOn(TasksBitField.Flag.hdTcfileOldRename) || oldHDExists))
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
            if (this.Contains(passHDnewP) && (passHDnewP.Text != passHDnewPConfirm.Text))
            {//We have an error unequal passwords
                checkBoxHDnewP.Checked = true;
                passHDnewP.BackColor = Color.Yellow;
                passHDnewPConfirm.BackColor = Color.Yellow;
                passHDnewP.Focus();
                return;
            }
            if (this.Contains(passTravNewP) && (passTravNewP.Text != passTravNewPConfirm.Text))
            {
                checkBoxTravnewP.Checked = true;
                passTravNewP.BackColor = Color.Yellow;
                passTravNewPConfirm.BackColor = Color.Yellow;
                passTravNewP.Focus();
                return; 
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}
