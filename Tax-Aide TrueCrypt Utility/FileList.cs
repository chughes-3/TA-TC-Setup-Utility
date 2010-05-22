using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace TaxAide_TrueCrypt_Utility
{
    public partial class FileList : Form
    {
        public static readonly string tcFilename = "TPDATA.TC"; //Used for new file name on old filename check for tsdata.tc existence when do file copy
        List<string> tcFilenamesOldTrav = new List<string>() { "tpdata.tc", "trdata.tc", "tqdata.tc" };
        readonly string taSWExist = "Start_Tax-Aide_Drive.exe"; //filename used to test for existence of Tax-Aide sw
        int travTcFilePossCount;
        List<string> travTcFilePoss = new List<string>();
        List<string> travUSBDrv = new List<string>();
        TasksBitField radioBut1 = new TasksBitField();
        TasksBitField radioBut2 = new TasksBitField();
        TasksBitField radioBut3 = new TasksBitField();
        TasksBitField tasklist1;
        TrueCryptFile tcFileHDOldLoc;
        TrueCryptFile tcFileTravOldLoc;
        public FileList(TasksBitField tasklistpassed,TrueCryptFile tcFileHDOld,TrueCryptFile tcFileTravOld)
        {
            InitializeComponent();
            tasklist1 = tasklistpassed;
            tcFileHDOldLoc = tcFileHDOld;
            tcFileTravOldLoc = tcFileTravOld;
            radioButton1.Text = "Text";
            //radioButton2.Enabled = false;

      #region Get data for decisions on hard drive and traveler drive
            TrueCryptFilesNew.tcFilePathHDNew = Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename; //sets up new name will be changed next for vista/w7
            // Find out if hard drive tpdata exists
            if (File.Exists(Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename))
            {
                tcFileHDOldLoc.FileNamePath = Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename;
                TrueCryptFilesNew.tcFilePathHDNew = Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename; //set wrongly for vista/w7 here fixed in next clause
            }
            if (TrueCryptSWObj.osVer == 6)
            {
                TrueCryptFilesNew.tcFilePathHDNew = Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename;
                if (File.Exists(Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename))
                {
                    tcFileHDOldLoc.FileNamePath = Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename;
                    TrueCryptFilesNew.tcFilePathHDNew = Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename;
                }
            }
            //now get any traveler drives
            foreach (DriveInfo drv in DriveInfo.GetDrives())
            {
                if ((drv.DriveType == DriveType.Removable) & drv.IsReady)
                {
                    travUSBDrv.Add(drv.ToString().Substring(0,2) + " (" + drv.VolumeLabel + ")");
                    if (File.Exists(drv.ToString().Substring(0, 2) + @"\" + tcFilenamesOldTrav[0]))
                    {
                        travTcFilePoss.Add(drv.ToString().Substring(0, 2) + @"\" + tcFilenamesOldTrav[0]);
                        travTcFilePossCount += 1;
                    }
                    else if (File.Exists(drv.ToString().Substring(0, 2) + @"\" + tcFilenamesOldTrav[1]))
                    {
                        travTcFilePoss.Add(drv.ToString().Substring(0, 2) + @"\" + tcFilenamesOldTrav[1]);
                        travTcFilePossCount += 1;
                    }
                    else if (File.Exists(drv.ToString().Substring(0, 2) + @"\" + tcFilenamesOldTrav[2]))
                    {
                        travTcFilePoss.Add(drv.ToString().Substring(0, 2) + @"\" + tcFilenamesOldTrav[2]);
                        travTcFilePossCount += 1;
                    }
                    else
                    {
                        travTcFilePoss.Add(string.Empty);
                    }
                }
            }
    #endregion

    #region Decisions to setup initial form

            if (travTcFilePossCount > 0)
            {
                radioButton1.Tag = travTcFilePoss.FindLastIndex(delegate(string s) { return (s != string.Empty); });
                radioButton1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)radioButton1.Tag];
                radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                radioButton2.Tag = travTcFilePoss.FindLastIndex(delegate(string s) { return (s != string.Empty); });
                radioButton2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)radioButton2.Tag];
                radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                radioButton3.Text = "Do Tasks on Hard Drive ";
                radioButton3.Tag = "HD";
                if (travUSBDrv.Count >1) {showOtherUsbs.Visible = true;}
            }
            else if (travUSBDrv.Count > 0)
            {
                radioButton1.Text = "Create Traveler Volume on " + travUSBDrv[travUSBDrv.Count - 1];
                radioButton1.Tag = travUSBDrv.Count - 1;
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                Log.WritWTime("USB Drive = " + travUSBDrv[travUSBDrv.Count - 1]);
                radioButton3.Text = "Do Tasks on Hard Drive";
                radioButton3.Tag = "HD";
                if (travUSBDrv.Count > 1)
                {
                    radioButton2.Text = "Create Traveler Volume on " + travUSBDrv[travUSBDrv.Count - 2];
                    radioBut2.SetFlag(TasksBitField.Flag.travtcFileFormat);
                    if (travUSBDrv.Count > 2) { showOtherUsbs.Visible = true; }
                }
                else { radioButton2.Visible = false; }
            }
            else if (TrueCryptSWObj.tCryptRegEntry == null)
            {//just go straight to install on c drive
                radioButton1.Text = "Install Software, Create TrueCrypt Volume on Hard Drive";
                radioBut1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                radioButton2.Text = "Only install TrueCrypt Software on Hard Drive";
                radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                radioButton3.Visible = false;
            }
            else
            {// tc installed

                if (tcFileHDOld.FileNamePath != null)
                {
                    radioButton1.Text = "Resize TrueCrypt Volume on " + tcFileHDOld.FileNamePath;
                    radioBut1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                    radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    radioButton2.Text = "Upgrade TrueCrypt Software";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                    radioButton3.Visible = false;
                }
                else
                {
                    radioButton1.Text = "Create TrueCrypt Volume on Hard Drive";
                    radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    radioButton2.Text = "Only Upgrade TrueCrypt Software";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    radioButton3.Visible = false;
                }
            }
    #endregion
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                //Clear tasks
                radioBut1.taskList = 0;
                radioBut2.taskList = 0;
                radioBut3.taskList = 0;
                if ((string)radioButton3.Tag == "HD")
                {//change top buttons to HD
                    showOtherUsbs.Visible = false;
                    if (TrueCryptSWObj.tCryptRegEntry == null)
                    {//just go straight to install on c drive
                        radioButton1.Text = "Install Software, Create TrueCrypt Volume on Hard Drive";
                        radioBut1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                        radioBut1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                        radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                        radioButton2.Text = "Only install TrueCrypt Software on Hard Drive";
                        radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                        radioButton3.Text = "Do Tasks on Traveler Drive ";
                        radioButton3.Tag = "TRAV";
                    }
                    else
                    {// tc installed

                        if (tcFileHDOldLoc.FileNamePath != null)
                        {
                            radioButton1.Text = "Resize TrueCrypt Volume on " + tcFileHDOldLoc.FileNamePath;
                            radioBut1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                            radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                            radioButton2.Text = "Upgrade TrueCrypt Software";
                            radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                            radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                            radioBut2.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                            radioButton2.Visible = true;
                            radioButton3.Text = "Do Tasks on Traveler Drive ";
                            radioButton3.Tag = "TRAV";
                        }
                        else
                        {
                            radioButton1.Text = "Create TrueCrypt Volume on Hard Drive";
                            radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                            radioButton2.Text = "Only Upgrade TrueCrypt Software";
                            radioButton2.Visible = true;
                            radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                            radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                           radioButton3.Text = "Do Tasks on Traveler Drive ";
                            radioButton3.Tag = "TRAV";
                        }
                    }
                }
                else
                {
                    if (travTcFilePossCount > 0)
                    {
                        radioButton1.Tag = travTcFilePoss.FindLastIndex(delegate(string s) { return (s != string.Empty); });
                        radioButton1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)radioButton1.Tag];
                        radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                        radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                        radioButton2.Tag = travTcFilePoss.FindLastIndex(delegate(string s) { return (s != string.Empty); });
                        radioButton2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)radioButton2.Tag];
                        radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                        radioButton3.Text = "Do Tasks on Hard Drive ";
                        radioButton3.Tag = "HD";
                        Log.WritWTime("TCFile Poss = " + travUSBDrv[(int)radioButton1.Tag]);
                        if (travUSBDrv.Count > 1) { showOtherUsbs.Visible = true; }
                    }
                    else if (travUSBDrv.Count > 0)
                    {
                        radioButton1.Text = "Create Traveler Volume on " + travUSBDrv[travUSBDrv.Count - 1];
                        radioButton1.Tag = travUSBDrv.Count - 1;
                        radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                        Log.WritWTime("USB Drive = " + travUSBDrv[travUSBDrv.Count - 1]);
                        radioButton3.Text = "Do Tasks on Hard Drive";
                        radioButton3.Tag = "HD";
                        if (travUSBDrv.Count > 1)
                        {
                            radioButton2.Text = "Create Traveler Volume on " + travUSBDrv[travUSBDrv.Count - 2];
                            radioBut2.SetFlag(TasksBitField.Flag.travtcFileFormat);
                            if (travUSBDrv.Count > 2) { showOtherUsbs.Visible = true; }
                        }
                        else { radioButton2.Visible = false; }
                    }
                }
            radioButton1.Checked = true;
            }
        }


        private void OK_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                tasklist1.taskList |= radioBut1.taskList;   //do initial setup of tasks
                if (radioBut1.IsOn(TasksBitField.Flag.hdTCFileFormat)) //therefore resize or create a volume
                {
                    Log.WriteStrm.WriteLine(string.Format("radiobut1 = 0x{0:X}",radioBut1.taskList));
                    if (!File.Exists(TrueCryptSWObj.tcProgramDirectory + "\\" + taSWExist) | !Directory.Exists(TrueCryptSWObj.tcProgramDirectory))
                    {
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);   // to deal with case that only TC installed previously now want to create/resize
                    }
                    Check4HostUpgrade();
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travtcFileFormat)) //set by both resize and make new
                {//we have create a new traveler file NEW filename set in checkTravswexists
                    string drv = travUSBDrv[(int)radioButton1.Tag].Substring(0, 2);  //tag stores label name as well as drive
                    checkTravSwExists(drv); //checks if sw exists and sets flags for upgrades if necessary
                    //now check if need host upgrade to work with traveler
                    Check4HostUpgrade();
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                {   // we have a traveler file resize check if traveler needs upgrading and check host also
                    tcFileTravOldLoc.FileNamePath = travTcFilePoss[(int)radioButton1.Tag]; //this works for resize
                }
            }
            else
            {
                if (radioButton2.Checked == true)
                {

                    tasklist1.taskList |= radioBut2.taskList;   //do initial setup of tasks
                    Log.WriteStrm.WriteLine(string.Format("radiobut2 = {0:X}", radioBut2.taskList));
                    if (radioBut2.IsOn(TasksBitField.Flag.hdTcSwInstall))
                    {
                        Check4HostUpgrade();
                    }
                    if (radioBut2.IsOn(TasksBitField.Flag.travSwInstall))
                    {//we have a request to upgrade sw means tc vol MAY exist
                        string drv = travUSBDrv[(int)radioButton2.Tag].Substring(0, 2);
                        checkTravSwExists(drv); //checks if sw exists and sets flags for upgrades if necessary                        
                        if (travTcFilePoss[(int)radioButton2.Tag] != string.Empty)
                        {
                            tcFileTravOldLoc.FileNamePath = travTcFilePoss[(int)radioButton2.Tag];
                            if (tasklist1.IsOn(TasksBitField.Flag.travTASwOldDelete))
                            {//we are dong full upgrade due to old sw, so must set data move
                                tasklist1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                            }                            
                        }

                    }
                    if (radioBut2.IsOn(TasksBitField.Flag.travtcFileFormat)) //set by make new
                    {//we have create a new traveler file
                        string drv = travUSBDrv[(int)radioButton2.Tag].Substring(0, 2);
                        checkTravSwExists(drv); //checks if sw exists and sets flags for upgrades if necessary plus sets new traveler filepath
                        //now check if need host upgrade to work with traveler
                        Check4HostUpgrade();

                    }
                }
                else
                {
                    Log.WritSection("We Have a problem with radio button selection");
                }
            }
            //MessageBox.Show(selectedFilePath);
            Log.WritWTime("Close of FileList Old Paths = " + tcFileTravOldLoc.FileNamePath + " & " + tcFileHDOldLoc.FileNamePath);
            Close();
        }

        private void showOtherUsbs_Click(object sender, EventArgs e)
        {
            USBDriveSelection usbSelect = new USBDriveSelection(travUSBDrv,travTcFilePoss);
            usbSelect.ShowDialog();
            if (usbSelect.DialogResult == DialogResult.OK)
            {
                radioBut1.taskList = 0;
                radioBut2.taskList = 0;
                radioBut3.taskList = 0;
                if (travTcFilePoss[usbSelect.selectedDrv] != string.Empty)
                {//need 2 buttons for the drive cause TC file exists
                    radioButton1.Tag = usbSelect.selectedDrv;
                    radioButton1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)radioButton1.Tag];
                    radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                    radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                    radioButton2.Tag = usbSelect.selectedDrv;
                    radioButton2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)radioButton2.Tag];
                    radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                    radioButton2.Visible = true;
                }
                else
                {//no tc file so just drive
                    radioButton1.Text = "Create Traveler Volume on " + travUSBDrv[usbSelect.selectedDrv];
                    radioButton1.Tag = usbSelect.selectedDrv;
                    radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                    radioButton2.Visible = false;
                }
            }
            //this.Height += 60;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton1.Checked)
            {
                groupBox2.Enabled = false;
            }
            else
            {
                groupBox2.Enabled = true;

            }
        }
        void checkTravSwExists(string drv)
        {
            TrueCryptFilesNew.tcFilePathTravNew = drv + "\\" + tcFilename;
            Log.WriteStrm.WriteLine("End of checktravsw New Path = " + TrueCryptFilesNew.tcFilePathTravNew);
            if (File.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe"))
            {   //existence of this directory means at 6.2 or 6.3
                string str = FileVersionInfo.GetVersionInfo(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe").FileVersion;
                Log.WriteStrm.WriteLine("Traveler TrueCrypt Program Path = " + drv + "\\Tax-Aide_Traveler\\TrueCrypt.exe" + " Version=" + str);
                //in future test for traveler sw release here and upgrade
            }
            else
            {
                if (File.Exists(drv + "\\truecrypt.exe"))
                {
                    tasklist1.SetFlag(TasksBitField.Flag.travTASwOldDelete);                    
                } 
                tasklist1.SetFlag(TasksBitField.Flag.travSwInstall);
                tasklist1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                Log.WriteStrm.Write("FileList Traveler TrueCrypt to be upgraded/installed ");
                if (File.Exists(drv + "\\TrueCrypt.exe"))
                {
                    Log.WriteStrm.WriteLine("from version " + FileVersionInfo.GetVersionInfo(drv.Substring(0, 2) + "TrueCrypt.exe").FileVersion);
                }
            }
        }
        void Check4HostUpgrade()
        {
            if (TrueCryptSWObj.tCryptRegEntry != null)
            {
                string str = FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion;
                if (string.Compare(FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion, TrueCryptSWObj.tcSetupVersion) <= 0)
                {//upgrade on host
                    tasklist1.SetFlag(TasksBitField.Flag.hdTASwOldDelete);
                    tasklist1.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    tasklist1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    Log.WritWTime(tcFileHDOldLoc.FileNamePath + ", " + FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion);
                    if (tcFileHDOldLoc.FileNamePath != null & string.Compare(FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion, "6.2") < 0)
                    {
                        if (tasklist1.TestTrav())
                        {
                            if (DialogResult.Cancel == MessageBox.Show("The TrueCrypt Software on the hard drive has to be upgraded\n to be compatible with the Traveler Software on the USB drive.\n An existing TrueCrypt Volume has been detected. After the hard drive upgrade the contents of the old volume will be copied across to a newly created volume, you will be asked next for the new size of the hard drive volume", "Tax-Aide TrueCrypt Utilities", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand))
                            {
                                Environment.Exit(1);
                            }
                            //Need to get new vol size here for hard drive upgrade
                        }
                        else
                        {//not traveler so doing hard drive but need to test if upgrade spec'd
                            if (!tasklist1.IsOn(TasksBitField.Flag.hdTCFileFormat))
                            {// NO file format so we are doing upgrade but have to do data file
                               if (DialogResult.Cancel == MessageBox.Show("An existing hard drive TrueCrypt Volume has been detected.The TrueCrypt Software on the hard drive that is being upgraded is sufficiently old that this data volume must also be regenerated, you will be asked next for the new size of the hard drive volume", "Tax-Aide TrueCrypt Utilities", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand))
                                {
                                    Environment.Exit(1);
                                }
                                //Need to get new vol size here for hard drive upgrade
                            }

                        }
                        tasklist1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                        tasklist1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    }
                }
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
