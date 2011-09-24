using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StartTA_TCFile
{
    static class StartTAMain
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ProgramData thisProg = new ProgramData();
            TrueCryptSWObj tcSoftware = new TrueCryptSWObj(thisProg);
            TC_Data_File tcData = new TC_Data_File(thisProg, tcSoftware);
            tcData.OpenTcFile();
            tcData.PDrivePerms();
            tcData.SharePDrive();
            MessageBox.Show("Drive " + TC_Data_File.tCryptDriveLetter + " is mounted and shared as " + TC_Data_File.shareName + "\rIf on a network the Workstations can be started now",thisProg.mbCaption,MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
