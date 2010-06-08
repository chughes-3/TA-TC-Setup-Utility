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
    public partial class USBDriveSelection : Form
    {
        public int selectedDrv;
        List<RadioButton> RadButtList = new List<RadioButton>();
        public USBDriveSelection()
        {
            InitializeComponent();
            foreach (Control child in this.Controls)     // get a list of radio buttons so can manipulate them
            {
                RadioButton radio = child as RadioButton;
                if (radio != null)
                {
                    RadButtList.Add(radio);
                }
            }
            RadButtList.Reverse();
            int count = 0;
            if (FileList.travUSBDrv.Count < 7) { count = FileList.travUSBDrv.Count; }
            else { count = 6; }
            for (int i = 0; i < count; i++)
            {
                RadButtList[i].Text = FileList.travUSBDrv[i].combo;
                if (FileList.travUSBDrv[i].tcFilePoss != string.Empty)
                {
                    RadButtList[i].Text += " with " + FileList.travUSBDrv[i].tcFilePoss;
                }
                RadButtList[i].Visible = true;
            }
            RadButtList[count - 1].Checked = true;  //to get off first button enabled since that will have already been selested
        }

        private void USBDriveSelection_Load(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            selectedDrv = RadButtList.FindIndex(delegate(RadioButton radio) { return radio.Checked; });
            Close();
        }
    }
}
