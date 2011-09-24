using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using StartTA_TCFile;
using System.Management;
using System.IO;

namespace Stop_Tax_Aide_Drive
{
    class TC_Data_FileStop
    {
        public const string tCryptDriveLetter = "P";
        const string tCryptDriveName = tCryptDriveLetter + ":\\";
        public const string shareNameLegacy = "TWSRVR_" + tCryptDriveLetter;
        public const string shareName = "TaxWiseServer_" + tCryptDriveLetter;

        ProgramData thisProginst;
        TrueCryptSWObj tcSWobj;
        public TC_Data_FileStop(ProgramData thisProgPass, TrueCryptSWObj tcSWObjPass)
        {
            thisProginst = thisProgPass;
            tcSWobj = tcSWObjPass;
        }
        public void CloseTcFile(string drvLetter)
        {
            Process tcproc = new Process();
            tcproc.StartInfo.FileName = tcSWobj.tcProgramFQN;
            tcproc.StartInfo.Arguments = " /q /f /d" + tCryptDriveLetter;
            try
            {
                tcproc.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception on starting TrueCrypt \r\n" + e.ToString(), thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            tcproc.WaitForExit();
            if (tcproc.ExitCode != 0)
            {
                MessageBox.Show("Drive Closing failed in some way, Exiting", thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
        public void DeleteShares()
        {
            ManagementObjectCollection shares = new ManagementClass("Win32_Share").GetInstances();
            foreach (ManagementObject shr in shares)
            {
                if (shr.GetPropertyValue("Name").ToString() == shareName | shr.GetPropertyValue("Name").ToString() == shareNameLegacy)
                {
                    try { shr.Delete(); }
                    catch (Exception) { continue;}
                }
            }
        }
        public void CleanUp()
        {
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Stop Traveler.lnk");
            if (!File.Exists(tCryptDriveName))
                MessageBox.Show("Tax-Aide P drive closed and any network shares removed", thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            else
                MessageBox.Show("Drive Closing failed in some way\r\n     Exiting", thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
