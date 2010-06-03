using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Management;

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
            System.Reflection.Assembly assem = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName assemName = assem.GetName();
            this.Text += " Ver " + assemName.Version.ToString();
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
            GetUSBDrives();
            for (int i = 0; i < travUSBDrv.Count; i++)
            //foreach (string drv in travUSBDrv)
            {
                //travUSBDrv.Add(drv.ToString().Substring(0,2) + " (" + drv.VolumeLabel + ")");
                if (File.Exists(travUSBDrv[i] + @"\" + tcFilenamesOldTrav[0]))
                {
                    travTcFilePoss.Add(travUSBDrv[i] + @"\" + tcFilenamesOldTrav[0]);
                    travTcFilePossCount += 1;
                }
                else if (File.Exists(travUSBDrv[i] + @"\" + tcFilenamesOldTrav[1]))
                {
                    travTcFilePoss.Add(travUSBDrv[i] + @"\" + tcFilenamesOldTrav[1]);
                    travTcFilePossCount += 1;
                }
                else if (File.Exists(travUSBDrv[i] + @"\" + tcFilenamesOldTrav[2]))
                {
                    travTcFilePoss.Add(travUSBDrv[i] + @"\" + tcFilenamesOldTrav[2]);
                    travTcFilePossCount += 1;
                }
                else
                {
                    travTcFilePoss.Add(string.Empty);
                }
                DriveInfo drvInfo = new DriveInfo(travUSBDrv[i]);
                travUSBDrv[i] += " (" + drvInfo.VolumeLabel + ")";

            }
            Log.WritSection("Trav TC File poss count = " + travTcFilePossCount.ToString() + ", Flash Drives count = " + travUSBDrv.Count.ToString());
    #endregion

    #region Decisions to setup initial form
            //Priority to flash keys if they are inserted. button 1 is only one that contains requirements for volume size to be filled out button 2 is strictly for software upgrades except for 2 blank travelers
            if (travTcFilePossCount > 0)
            {
                radioButton1.Tag = travTcFilePoss.FindIndex(delegate(string s) { return (s != string.Empty); });
                radioButton1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)radioButton1.Tag];
                radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                radioButton2.Tag = travTcFilePoss.FindIndex(delegate(string s) { return (s != string.Empty); });
                radioButton2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)radioButton2.Tag];
                radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                radioButton3.Text = "Do Tasks on Hard Drive ";
                radioButton3.Tag = "HD";
                if (travUSBDrv.Count >1) {showOtherUsbs.Visible = true;}
            }
            else if (travUSBDrv.Count > 0)
            {
                radioButton1.Text = "Create Traveler Volume on " + travUSBDrv[0];
                radioButton1.Tag = 0;
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                Log.WritWTime("USB Drive = " + travUSBDrv[0]);
                radioButton3.Text = "Do Tasks on Hard Drive";
                radioButton3.Tag = "HD";
                if (travUSBDrv.Count > 1)
                {
                    radioButton2.Text = "Create Traveler Volume on " + travUSBDrv[1];
                    radioBut2.SetFlag(TasksBitField.Flag.travtcFileFormat);
                    radioButton2.Tag = 1;
                    if (travUSBDrv.Count > 2) { showOtherUsbs.Visible = true; }
                }
                else { radioButton2.Visible = false; }
            }
            else if (TrueCryptSWObj.tCryptRegEntry == null)
            {//just go straight to install on c drive
                radioButton1.Text = "Install Software, Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                radioBut1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                radioButton2.Text = "Only install TrueCrypt Software on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
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
                    radioButton2.Text = "Upgrade TrueCrypt Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                    radioButton3.Visible = false;
                }
                else
                {
                    radioButton1.Text = "Create TrueCrypt Volume on Hard Drive(" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    radioButton2.Text = "Only Upgrade TrueCrypt Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    radioButton3.Visible = false;
                }
            }
    #endregion
        }

   #region Radio button 3 checked changed redo radiobutton content
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
                        radioButton1.Text = "Install Software, Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                        radioBut1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                        radioBut1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                        radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                        radioButton2.Text = "Only install TrueCrypt Software on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
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
                            radioButton2.Text = "Upgrade TrueCrypt Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                            radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                            radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                            radioBut2.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                            radioButton2.Visible = true;
                            radioButton3.Text = "Do Tasks on Traveler Drive ";
                            radioButton3.Tag = "TRAV";
                        }
                        else
                        {
                            radioButton1.Text = "Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                            radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                            radioButton2.Text = "Only Upgrade TrueCrypt Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
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
                        radioButton1.Tag = travTcFilePoss.FindIndex(delegate(string s) { return (s != string.Empty); });
                        radioButton1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)radioButton1.Tag];
                        radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                        radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                        radioButton2.Tag = travTcFilePoss.FindIndex(delegate(string s) { return (s != string.Empty); });
                        radioButton2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)radioButton2.Tag];
                        radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                        radioButton3.Text = "Do Tasks on Hard Drive ";
                        radioButton3.Tag = "HD";
                        Log.WritWTime("TCFile Poss = " + travUSBDrv[(int)radioButton1.Tag]);
                        if (travUSBDrv.Count > 1) { showOtherUsbs.Visible = true; }
                    }
                    else if (travUSBDrv.Count > 0)
                    {
                        radioButton1.Text = "Create Traveler Volume on " + travUSBDrv[0];
                        radioButton1.Tag = 0;
                        radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                        Log.WritWTime("USB Drive = " + travUSBDrv[0]);
                        radioButton3.Text = "Do Tasks on Hard Drive";
                        radioButton3.Tag = "HD";
                        if (travUSBDrv.Count > 1)
                        {
                            radioButton2.Text = "Create Traveler Volume on " + travUSBDrv[1];
                            radioBut2.SetFlag(TasksBitField.Flag.travtcFileFormat);
                            radioButton2.Tag = 1;
                            if (travUSBDrv.Count > 2) { showOtherUsbs.Visible = true; }
                        }
                        else { radioButton2.Visible = false; }
                    }
                }
                radioButton1.Checked = true;
            }
        } 
        #endregion


        private void OK_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                tasklist1.taskList |= radioBut1.taskList;   //do initial setup of tasks
                if (radioBut1.IsOn(TasksBitField.Flag.hdTCFileFormat)) //therefore resize or create a volume
                {
                    TrueCryptFilesNew.tcFileHDSizeNew = CheckEditBox(radioButton1, radioBut1);    // check on size error = 0
                    if (TrueCryptFilesNew.tcFileHDSizeNew == 0)
                    {   // data entry error clear tasklist previously set
                        tasklist1.taskList = 0;
                        return;
                    }
                    if (!File.Exists(TrueCryptSWObj.tcProgramDirectory + "\\" + taSWExist) | !Directory.Exists(TrueCryptSWObj.tcProgramDirectory))
                    {
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);   // to deal with case that only TC installed previously now want to create/resize
                    }
                    Check4HostUpgrade();
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))   // must be done first to set up size for traveler size checking
                {   
                    tcFileTravOldLoc.FileNamePath = travTcFilePoss[(int)radioButton1.Tag]; 
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travtcFileFormat)) //set by both resize and make new
                {//we have create a new traveler file NEW filename set in checkTravswexists
                    TrueCryptFilesNew.tcFileTravSizeNew = CheckEditBox(radioButton1, radioBut1);    // check on size error = 0
                    if (TrueCryptFilesNew.tcFileTravSizeNew == 0) //checkedit box returns zero if data entry is zero
                    {   // data entry error clear tasklist previously set
                        tasklist1.taskList = 0;
                        return;
                    }
                    string drv = travUSBDrv[(int)radioButton1.Tag].Substring(0, 2);  //tag stores label name as well as drive
                    if (TrueCryptSWObj.tCryptRegEntry == null)
                    {// nothing on host so must setup fqn for traveler
                        TrueCryptSWObj.tcProgramFQN = drv + "\\" + "\\Tax-Aide_Traveler\\truecrypt.exe";
                        TrueCryptSWObj.tcProgramDirectory = TrueCryptSWObj.tcProgramFQN.Substring(0, TrueCryptSWObj.tcProgramFQN.Length - 14);
                    }
                    checkTravSwExists(drv); //checks if sw exists and sets flags for upgrades if necessary
                    //now check if need host upgrade to work with traveler
                    Check4HostUpgrade();
                }
            }
            else
            {
                if (radioButton2.Checked == true)
                {

                    tasklist1.taskList |= radioBut2.taskList;   //do initial setup of tasks
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
                        TrueCryptFilesNew.tcFileTravSizeNew = CheckEditBox(radioButton2, radioBut2);    // check on size error = 0
                        if (TrueCryptFilesNew.tcFileTravSizeNew == 0)
                        {   // data entry error clear tasklist previously set
                            tasklist1.taskList = 0;
                            return;
                        }
                        string drv = travUSBDrv[(int)radioButton2.Tag].Substring(0, 2);
                        if (TrueCryptSWObj.tCryptRegEntry == null)
                        {// nothing on host so must setup fqn for traveler
                            TrueCryptSWObj.tcProgramFQN = drv + "\\" + "\\Tax-Aide_Traveler\\truecrypt.exe";
                            TrueCryptSWObj.tcProgramDirectory = TrueCryptSWObj.tcProgramFQN.Substring(0, TrueCryptSWObj.tcProgramFQN.Length - 14);
                        }
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
            //Log.WritSection(string.Format("tasklist = {0:X}",tasklist1.taskList));
            Log.WritWTime("Dialog Close >> Old Paths = " + tcFileTravOldLoc.FileNamePath + " & " + tcFileHDOldLoc.FileNamePath);
            Log.WritWTime("HD Size = " + TrueCryptFilesNew.tcFileHDSizeNew + "MB, Trav Size = " + TrueCryptFilesNew.tcFileTravSizeNew + "MB");
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
                radioButton1.Checked = false;
                radioButton1.Checked = true;    //forces update of size in groupbox plus makes sure something checked
            }
            //this.Height += 60;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton1.Checked)
            {

                if (radioButton2.Checked & !radioBut2.IsOn(TasksBitField.Flag.travtcFileFormat))
                {//only time need vol size is if radio button 2 is on usb tc file create
                    groupBox2.Enabled = false;
                    sizeLabel.Visible = false;
                }
                else
                {//radiobutton 2 checked and file format (or radion3 but that is irrelevant)
                    groupBox2.Enabled = true;
                    if (radioBut2.IsOn(TasksBitField.Flag.travtcFileFormat))
                    {
                        string drvStr = travUSBDrv[(int)radioButton2.Tag].Substring(0, 2);
                        DriveInfo drv = new DriveInfo(drvStr);
                        int space = (int)drv.TotalFreeSpace / 1048576;
                        if (!Directory.Exists(drvStr + "\\traveler"))
                        {
                            space -= 10; // for traveler software
                        }
                        sizeLabel.Text = drvStr + " has " + space.ToString() + "MB of Available Space ";
                        if (radioBut2.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                        {// we have old traveler vol
                            FileInfo f = new FileInfo(travTcFilePoss[(int)radioButton2.Tag]);
                            int fSize = (int)f.Length / 1048576;
                            sizeLabel.Text += "with an existing " + fSize.ToString() + "MB TrueCrypt Volume";
                        }
                        else
                        {
                            sizeLabel.Text = "                  " + sizeLabel.Text; //center the label
                        }
                        sizeLabel.Visible = true;
                    }
                    else
                    {//radio button 1 but not traveler
                        sizeLabel.Visible = false;
                    }
                }
            }
            else
            {
                groupBox2.Enabled = true;
                // radio button 1 checked have to do size computation only fr traveler
                if (radioBut1.IsOn(TasksBitField.Flag.travtcFileFormat))
                {//we have to create new traveler
                    string drvStr = travUSBDrv[(int)radioButton1.Tag].Substring(0, 2);
                    DriveInfo drv = new DriveInfo(drvStr);
                    int space = (int)drv.TotalFreeSpace / 1048576;
                    if (!Directory.Exists(drvStr + "\\traveler"))
                    {
                        space -= 10; // for traveler software
                    }
                    sizeLabel.Text = drvStr + " has " + space.ToString() + "MB of Available Space ";
                    if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                    {// we have old traveler vol
                        FileInfo f = new FileInfo(travTcFilePoss[(int)radioButton1.Tag]);
                        int fSize = (int)f.Length / 1048576;
                        sizeLabel.Text += "with an existing " + fSize.ToString() + "MB TrueCrypt Volume";
                    }
                    else
                    {
                        sizeLabel.Text = "                  " + sizeLabel.Text; //center the label
                    }
                    sizeLabel.Visible = true;
                }
                else
                {//radio button 1 but not traveler
                    sizeLabel.Visible = false;
                }
            }
        }
        void checkTravSwExists(string drv)
        {
            TrueCryptFilesNew.tcFilePathTravNew = drv + "\\" + tcFilename;
            if (File.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe"))
            {   //existence of this directory means at 6.2 or 6.3
                string str = FileVersionInfo.GetVersionInfo(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe").FileVersion;
                Log.WriteStrm.WriteLine("FileList Traveler TrueCrypt Program Path = " + drv + "\\Tax-Aide_Traveler\\TrueCrypt.exe" + " Version=" + str);
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
                else
                {
                    Log.WriteStrm.WriteLine("");
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
                    if (tcFileHDOldLoc.FileNamePath != null)
                    {
                        Log.WritWTime(tcFileHDOldLoc.FileNamePath + ", " + FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion); 
                    }
                    else
                    {
                        Log.WritWTime("No Hard Drive TC File");  
                    }
                    if (tcFileHDOldLoc.FileNamePath != null & string.Compare(FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion, "6.2") < 0)
                    {
                        if (tasklist1.TestTrav())
                        {
                            if (DialogResult.Cancel == MessageBox.Show("The TrueCrypt Software on the hard drive has to be upgraded to be compatible with the Traveler Software on the USB drive. An existing TrueCrypt Volume has been detected. After the hard drive upgrade the contents of the old volume will be copied across to a newly created volume, you will be asked next for the new size of the hard drive volume", TrueCryptSWObj.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand))
                            {
                                Environment.Exit(1);
                            }
                            //Need to get new vol size here for hard drive upgrade
                        }
                        else
                        {//not traveler so doing hard drive but need to test if upgrade spec'd
                            if (!tasklist1.IsOn(TasksBitField.Flag.hdTCFileFormat))
                            {// NO file format so we are doing upgrade but have to do data file
                                if (DialogResult.Cancel == MessageBox.Show("An existing hard drive TrueCrypt Volume has been detected.The TrueCrypt Software on the hard drive that is being upgraded is sufficiently old that this data volume must also be regenerated, you will be asked next for the new size of the hard drive volume", TrueCryptSWObj.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand))
                                {
                                    Environment.Exit(1);
                                }
                                //Need to get new vol size here for hard drive upgrade
                            }

                        }
                        tasklist1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                        tasklist1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                        //let's get teh new HD size via a dialog with the user
                        if (TrueCryptFilesNew.tcFileHDSizeNew == 0)
                        {//Not previously set so we need to set it here we get to this point with a simple HD resize with an old release of TC SW
                            hdTCSize hdtcFileSize = new hdTCSize((int)tcFileHDOldLoc.size / 1000000);
                            hdtcFileSize.ShowDialog();
                            TrueCryptFilesNew.tcFileHDSizeNew = hdtcFileSize.hdFileSizeNew; 
                        }
                    }
                }
            }

        }

        private int CheckEditBox(RadioButton radioButton, TasksBitField radBut)
        {
            int input;
            if (!Int32.TryParse(newFileSizeMB.Text, out input))
            {
                MessageBox.Show("Non Numeric entry in Volume Size\n\n\t" + newFileSizeMB.Text, TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                newFileSizeMB.Focus();
                return 0;
            }
            if (gBytes.Checked) { input *= 1000; }
            if (input > 7000)
            {
                if (DialogResult.No == MessageBox.Show("Are you sure you meant this Volume size\n" + input.ToString() + "MB will take a really long time to format", TrueCryptSWObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (input < 5)
            {
                if (DialogResult.No == MessageBox.Show("Are you sure you meant this Volume size\n\n\t" + input.ToString() + "MB is really small??", TrueCryptSWObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (newFileSizeMB.ForeColor == SystemColors.GrayText)
            {
                if (DialogResult.No == MessageBox.Show("There has been no entry in the Volume Size Box. Did you intend to use the default of 950 MBytes?", TrueCryptSWObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (input == 0) { return 0; }
            if (radBut.IsOn(TasksBitField.Flag.travtcFileFormat))
            {//we have traveler so must check sizing
                long oldFSize = 0;
                if (tcFileTravOldLoc.FileNamePath != null) //ie have old file which is taking up flash room
                {
                    oldFSize = tcFileTravOldLoc.size;
                }
                DriveInfo drv = new DriveInfo(travUSBDrv[(int)radioButton.Tag].Substring(0, 2));
                long maxSize = drv.TotalFreeSpace - 10000000 + oldFSize;
                if (maxSize < input * 1048576)
                {
                    MessageBox.Show("Volume Size is larger that possible on this flash drive\n\t" + newFileSizeMB.Text + " MBytes\n\nThe Maximum is " + (maxSize / 1048576).ToString() + " MBytes", TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newFileSizeMB.Text = "";
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            return input;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Edit_Entry(object sender, EventArgs e) //ystem.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, S
        {
            newFileSizeMB.Font = new Font(this.Font, FontStyle.Regular);
            newFileSizeMB.ForeColor = SystemColors.WindowText;
            newFileSizeMB.Text = "";
        }
        private void GetUSBDrives() // just that from hardware up
        {
            ManagementClass logicalToPartition = new ManagementClass("Win32_LogicalDiskToPartition");
            ManagementClass partitionToDiskDrv = new ManagementClass("Win32_DiskDriveToDiskPartition");
            ManagementClass diskDrvs = new ManagementClass("Win32_DiskDrive");
            List<ManagementObject> usbDrvs = new List<ManagementObject>();
            List<ManagementObject> partitions = new List<ManagementObject>();
            //List<ManagementObject> logicalDrvs = new List<ManagementObject>();
            foreach (ManagementObject udrv in diskDrvs.GetInstances())
            {
                if (udrv.GetPropertyValue("PNPDeviceID").ToString().StartsWith("USBSTOR"))
                {
                    usbDrvs.Add(udrv);
                }
            }
            foreach (ManagementObject ud in usbDrvs)
            {
                foreach (ManagementObject parti in partitionToDiskDrv.GetInstances())
                {
                    if (parti.GetPropertyValue("Antecedent").ToString().Contains(ud.GetPropertyValue("DeviceID").ToString().Replace(@"\", @"\\")))
                    {
                        partitions.Add(parti);
                        break; //make sure only get one partition not 2
                    }
                }
            }
            foreach (ManagementObject partit in partitions)
            {
                foreach (ManagementObject logDrv in logicalToPartition.GetInstances())
                {
                    if (partit.GetPropertyValue("Dependent").ToString() == logDrv.GetPropertyValue("Antecedent").ToString())
                    {
                        //logicalDrvs.Add(logDrv);
                        travUSBDrv.Add(logDrv.GetPropertyValue("Dependent").ToString().Substring(logDrv.GetPropertyValue("Dependent").ToString().Length - 3, 2));
                    }
                }
            }
        }
    }
}
