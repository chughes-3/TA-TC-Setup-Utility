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
        public static List<DrvInfo> travUSBDrv = new List<DrvInfo>(); //DrvInfo class defined at end of this file
        TasksBitField radioBut1 = new TasksBitField();
        TasksBitField radioBut2 = new TasksBitField();
        TasksBitField radioBut3 = new TasksBitField();
        TasksBitField tasklist1;
        TrueCryptFile tcFileHDOldLoc;
        TrueCryptFile tcFileTravOldLoc;
        private System.Reflection.Assembly assem;
        public FileList(TasksBitField tasklistpassed, TrueCryptFile tcFileHDOld, TrueCryptFile tcFileTravOld, string calledArg)
        {
            InitializeComponent();
            tasklist1 = tasklistpassed;
            tcFileHDOldLoc = tcFileHDOld;
            tcFileTravOldLoc = tcFileTravOld;
            radioButton1.Text = "Text";
            assem = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName assemName = assem.GetName();
            this.Text += " Ver " + assemName.Version.ToString();
            //radioButton2.Enabled = false;
            
      #region Get data for decisions on hard drive and traveler drive
            //Build list of usb connected drives first so can test against it in tcfileobject instance
            if (calledArg != "format" || assem.Location.Substring(0,2) != Environment.GetEnvironmentVariable("HOMEDRIVE") )
            {//ie do this if not being called from start tax-aide drive to do a format on the hard drive
                GetUSBDrives(); //returns travusbdrv which is logical drive name and vol label and a combo string tcfile poss to be set here            
            }
            //Analyze hard drive situation
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
            //Now analyze usb drive situation
            //for (int i = 0; i < travUSBDrv.Count; i++)
            foreach (DrvInfo drv in travUSBDrv)
            {
                if (File.Exists(drv.drvName + @"\" + tcFilenamesOldTrav[0]))
                {
                    drv.tcFilePoss = drv.drvName + @"\" + tcFilenamesOldTrav[0];
                    travTcFilePossCount++;
                }
                else if (File.Exists(drv.drvName + @"\" + tcFilenamesOldTrav[1]))
                {
                    drv.tcFilePoss = drv.drvName + @"\" + tcFilenamesOldTrav[1];
                    travTcFilePossCount++;
                }
                else if (File.Exists(drv.drvName + @"\" + tcFilenamesOldTrav[2]))
                {
                    drv.tcFilePoss = drv.drvName + @"\" + tcFilenamesOldTrav[2];
                    travTcFilePossCount++;
                }
            }
            Log.WritSection("Trav TC File poss count = " + travTcFilePossCount.ToString() + ", USB Drives count = " + travUSBDrv.Count.ToString());
    #endregion

    #region Decisions to setup initial form
            //1st priority to see if running from an existing installation, if so only thing can do is format. After that priority to flash keys if they are inserted. button 1 is only one that contains requirements for volume size to be filled out button 2 is strictly for software upgrades except for 2 blank travelers
            //if (assem.Location.Contains("TrueCrypt\\Tax-Aide")||assem.Location.Contains("Tax-Aide_Traveler"))
            if (calledArg == "format")
            {//we are running from an existing install probably called from Start_Tax-Aide_drive.exe
                SetFormatClsStartProcess();
            }
            else if (travTcFilePossCount > 0 | travUSBDrv.Count > 0)
            {
                SetButtons4Traveler();
            }
            else 
            {//just go straight to install on c drive
                SetButtons4HD();
                radioButton3.Visible = false;
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
                    SetButtons4HD();
                        radioButton3.Text = "Do Tasks on Traveler Drive ";
                        radioButton3.Tag = "TRAV";
                }
                else
                {
                    SetButtons4Traveler();
                }
                radioButton1.Checked = true;
            }
        } 
        #endregion
        private void SetButtons4HD()
        {
            if (TrueCryptSWObj.tCryptRegEntry == null)
            {//just go straight to install on c drive
                if (tcFileHDOldLoc.FileNamePath == null)
                {
                    radioButton1.Text = "Install Software, Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                }
                else //to cater to where people have done manual uninstall but left data drive there
                {
                    radioButton1.Text = "Install Software, Resize TrueCrypt Volume on " + tcFileHDOldLoc.FileNamePath;
                    radioBut1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                }
                radioBut1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                radioButton2.Text = "Install TrueCrypt Software on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")" + ", no Tax-Aide Icons or Scripts";
                radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
            }
            else
            {// tc installed

                if (tcFileHDOldLoc.FileNamePath != null)
                {
                    radioButton1.Text = "Resize TrueCrypt Volume on " + tcFileHDOldLoc.FileNamePath;
                    radioBut1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                    radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    radioButton2.Text = "Upgrade TrueCrypt and Tax-Aide Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTASwOldDelete);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                    radioButton2.Visible = true;
                }
                else
                {
                    radioButton1.Text = "Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    radioButton2.Text = "Upgrade TrueCrypt Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")" + ", no Tax-Aide Icons or Scripts";
                    radioButton2.Visible = true;
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                }
            }
        }

        private void SetButtons4Traveler()
        {
            if (travTcFilePossCount > 0)
            {
                radioButton1.Tag = travUSBDrv.FindIndex(delegate(DrvInfo s) { return (s.tcFilePoss != string.Empty); });
                radioButton1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)radioButton1.Tag].combo;
                radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                radioButton2.Tag = travUSBDrv.FindIndex(delegate(DrvInfo s) { return (s.tcFilePoss != string.Empty); });
                radioButton2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)radioButton2.Tag].combo;
                radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                radioButton3.Text = "Do Tasks on Hard Drive ";
                radioButton3.Tag = "HD";
                if (travUSBDrv.Count > 1) { showOtherUsbs.Visible = true; }
            }
            else if (travUSBDrv.Count > 0)
            {
                radioButton1.Text = "Create Traveler Volume on " + travUSBDrv[0].combo;
                radioButton1.Tag = 0;
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                radioButton3.Text = "Do Tasks on Hard Drive";
                radioButton3.Tag = "HD";
                if (travUSBDrv.Count > 1)
                {
                    radioButton2.Text = "Create Traveler Volume on " + travUSBDrv[1].combo;
                    radioBut2.SetFlag(TasksBitField.Flag.travtcFileFormat);
                    radioButton2.Tag = 1;
                    if (travUSBDrv.Count > 2) { showOtherUsbs.Visible = true; }
                }
                else { radioButton2.Visible = false; }
            }
        }
        private void SetFormatClsStartProcess()
        {
            Log.WritWTime(assem.Location);
            //if (assem.Location.Contains("bin\\Debug")) //"bin\\debug" for debug purposes and "TrueCrypt\\Tax-Aide" for real
            if (assem.Location.Contains("TrueCrypt\\Tax-Aide")) //"bin\\debug" for debug purposes and "TrueCrypt\\Tax-Aide" for real
            {//We are on hard drive
                radioButton1.Text = "Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                Log.WritWTime("Formatting TC drive on Hard Drive called from Start_Tax-Aide_Drive");
            }
            else
            {// we have Traveler
                string drv = assem.Location.Substring(0, 2);
                //Next get index in travusbdrv list
                
                Log.WritWTime("Formatting TC Drive on Traveler called from Start_Tax-Aide_Drive on Traveler " + drv);
                radioButton1.Text = "Create Traveler Volume on " + drv;
                radioButton1.Tag = travUSBDrv.FindIndex(delegate(DrvInfo s) { return (s.drvName == drv); });
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
            }
            radioButton2.Visible = false;
            radioButton3.Visible = false;
        }
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
                    if (!File.Exists(TrueCryptSWObj.tcProgramDirectory + "\\Tax-Aide\\" + taSWExist) | !Directory.Exists(TrueCryptSWObj.tcProgramDirectory))
                    {
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);   // to deal with case that only TC installed previously now want to create/resize
                    }
                    Check4HostUpgrade();
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))   // must be done first to set up size for traveler size checking
                {
                    tcFileTravOldLoc.FileNamePath = travUSBDrv[(int)radioButton1.Tag].tcFilePoss; 
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travtcFileFormat)) //set by both resize and make new
                {//we have create a new traveler file NEW filename set in checkTravswexists
                    TrueCryptFilesNew.tcFileTravSizeNew = CheckEditBox(radioButton1, radioBut1);    // check on size error = 0
                    if (TrueCryptFilesNew.tcFileTravSizeNew == 0) //checkedit box returns zero if data entry is zero
                    {   // data entry error clear tasklist previously set
                        tasklist1.taskList = 0;
                        return;
                    }
                    string drv = travUSBDrv[(int)radioButton1.Tag].drvName;
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
                    //if (radioBut2.IsOn(TasksBitField.Flag.hdTcSwInstall))
                    //{
                        Check4HostUpgrade(); //whatever was selected need to check for upgrade
                    //}
                    if (radioBut2.IsOn(TasksBitField.Flag.travSwInstall))
                    {//we have a request to upgrade sw means tc vol MAY exist
                        string drv = travUSBDrv[(int)radioButton2.Tag].drvName;
                        checkTravSwExists(drv); //checks if sw exists and sets flags for upgrades if necessary                        
                        if (travUSBDrv[(int)radioButton2.Tag].tcFilePoss != string.Empty)
                        {
                            tcFileTravOldLoc.FileNamePath = travUSBDrv[(int)radioButton2.Tag].tcFilePoss;
                            if (tasklist1.IsOn(TasksBitField.Flag.travTASwOldDelete) && !tasklist1.IsOn(TasksBitField.Flag.travTASwOldIsver6_2))
                            {//we are dong full upgrade due to old sw, so must set data move but only if pre 6.2
                                tasklist1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                                tasklist1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                                // set the file size to be the same as old
                                FileInfo f = new FileInfo(tcFileTravOldLoc.FileNamePath);
                                TrueCryptFilesNew.tcFileTravSizeNew = (int)Math.Ceiling(f.Length / (double)1048576);
                            }                            
                        }
                    }
                    if (radioBut2.IsOn(TasksBitField.Flag.travtcFileFormat)) 
                    {//we have create a new traveler file
                        string drv = travUSBDrv[(int)radioButton2.Tag].drvName;
                        if (TrueCryptSWObj.tCryptRegEntry == null)
                        {// nothing on host so must setup fqn for traveler
                            TrueCryptSWObj.tcProgramFQN = drv + "\\" + "\\Tax-Aide_Traveler\\truecrypt.exe";
                            TrueCryptSWObj.tcProgramDirectory = TrueCryptSWObj.tcProgramFQN.Substring(0, TrueCryptSWObj.tcProgramFQN.Length - 14);
                        }
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
            this.DialogResult = DialogResult.OK;
        }

        private void showOtherUsbs_Click(object sender, EventArgs e)
        {
            USBDriveSelection usbSelect = new USBDriveSelection();
            usbSelect.ShowDialog();
            if (usbSelect.DialogResult == DialogResult.OK)
            {
                radioBut1.taskList = 0;
                radioBut2.taskList = 0;
                radioBut3.taskList = 0;
                if (travUSBDrv[usbSelect.selectedDrv].tcFilePoss != string.Empty)
                {//need 2 buttons for the drive cause TC file exists
                    radioButton1.Tag = usbSelect.selectedDrv;
                    radioButton1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)radioButton1.Tag].combo;
                    radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                    radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                    radioButton2.Tag = usbSelect.selectedDrv;
                    radioButton2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)radioButton2.Tag].combo;
                    radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                    radioButton2.Visible = true;
                }
                else
                {//no tc file so just drive
                    radioButton1.Text = "Create Traveler Volume on " + travUSBDrv[usbSelect.selectedDrv].combo;
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
                        string drvStr = travUSBDrv[(int)radioButton2.Tag].drvName;
                        DriveInfo drv = new DriveInfo(drvStr);
                        int space = (int)(drv.TotalFreeSpace / 1048576);
                        if (!Directory.Exists(drvStr + "\\traveler"))
                        {
                            space -= 10; // for traveler software
                        }
                        string spaceStr = space.ToString() + "MB";
                        if (space > 9999)
                        {
                            space /= 1024;
                            spaceStr = space.ToString() + "GB";
                        }
                        sizeLabel.Text = drvStr + " has " + spaceStr + " of Available Space ";
                        if (radioBut2.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                        {// we have old traveler vol
                            FileInfo f = new FileInfo(travUSBDrv[(int)radioButton2.Tag].tcFilePoss);
                            int fSize = (int)(f.Length / 1048576);
                            sizeLabel.Text += "with an existing " + fSize.ToString() + "MB TrueCrypt Volume";
                        }
                        else
                        {
                            sizeLabel.Text = "                  " + sizeLabel.Text; //center the label
                        }
                        sizeLabel.Visible = true;
                    }
                    else
                    {
                        sizeLabel.Visible = false;
                    }
                }
            }
            else //radio button 1 is checked
            {
                groupBox2.Enabled = true;
                // radio button 1 checked have to do size computation only fr traveler
                if (radioBut1.IsOn(TasksBitField.Flag.travtcFileFormat))
                {//we have to create new traveler
                    string drvStr = travUSBDrv[(int)radioButton1.Tag].drvName;
                    DriveInfo drv = new DriveInfo(drvStr);
                    int space = (int)(drv.TotalFreeSpace / 1048576);
                    if (!Directory.Exists(drvStr + "\\traveler"))
                    {
                        space -= 10; // for traveler software
                    }
                    string spaceStr = space.ToString() + "MB";
                    if (space > 9999)
                    {
                        space /= 1024;
                        spaceStr = space.ToString() + "GB";
                    }
                    sizeLabel.Text = drvStr + " has " + spaceStr + " of Available Space ";
                    if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                    {// we have old traveler vol
                        FileInfo f = new FileInfo(travUSBDrv[(int)radioButton1.Tag].tcFilePoss);
                        int fSize = (int)(f.Length / 1048576);
                        sizeLabel.Text += "with an existing " + fSize.ToString() + "MB TrueCrypt Volume";
                    }
                    else
                    {
                        sizeLabel.Text = "                      " + sizeLabel.Text; //center the label
                    }
                    sizeLabel.Visible = true;
                }
                else
                {//radio button 1 but not traveler
                    if (tcFileHDOldLoc.FileNamePath != null)
                    {
                        FileInfo f = new FileInfo(tcFileHDOldLoc.FileNamePath);
                        int fSize = (int)(f.Length / 1048576);
                        sizeLabel.Text = "            " + "Existing Hard Drive TrueCrypt Volume is " + fSize.ToString() + "MB";
                        sizeLabel.Visible = true;
                    }
                    else sizeLabel.Visible = false;
                }
            }
        }
        void checkTravSwExists(string drv)
        {
            TrueCryptFilesNew.tcFilePathTravNew = drv + "\\" + tcFilename;
            if (Directory.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler"))
            {   //existence of this directory means at 6.2 plus
                string travVer = "";
                if (File.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe"))
                {
                    travVer = FileVersionInfo.GetVersionInfo(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe").FileVersion; 
                }
                if (!File.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe") || string.Compare(travVer,TrueCryptSWObj.tcSetupVersion) < 0) //caters to directory existing but no files in it or being at less than current release
                {
                    tasklist1.SetFlag(TasksBitField.Flag.travTASwOldDelete);
                    tasklist1.SetFlag(TasksBitField.Flag.travTASwOldIsver6_2);
                    tasklist1.SetFlag(TasksBitField.Flag.travSwInstall);
                    //file format not set because no forced data upgrade in this section
                    Log.WriteStrm.WriteLine("FileList Traveler TrueCrypt to be upgraded from version " + travVer);
                }
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
                    Log.WriteStrm.WriteLine("from version " + FileVersionInfo.GetVersionInfo(drv.Substring(0, 2) + "\\TrueCrypt.exe").FileVersion);
                }
                else
                {
                    Log.WriteStrm.WriteLine("");
                }
            }
        }
        internal void Check4HostUpgrade() //called in this file and from program.cs for called from startTaxAideDrive
        {
            if (TrueCryptSWObj.tCryptRegEntry != null)
            {
                string str = FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion;
                if (string.Compare(FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion, TrueCryptSWObj.tcSetupVersion) < 0)
                {//upgrade on host
                    tasklist1.SetFlag(TasksBitField.Flag.hdTASwOldDelete);
                    tasklist1.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    tasklist1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    if (tcFileHDOldLoc.FileNamePath != null || tasklist1.IsOn(TasksBitField.Flag.hdTCFileFormat))
                    {//in case of call to format from startTaxAideDrive get here with no old data file but file format set
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);//to make sure we install TA sw if data file does not need to be upgraded
                        Log.WritWTime(tcFileHDOldLoc.FileNamePath + ", " + FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion); 
                    }
                    else
                    {
                        Log.WritWTime("No Hard Drive TC File");  
                    }
                    if (tcFileHDOldLoc.FileNamePath != null & string.Compare(FileVersionInfo.GetVersionInfo(TrueCryptSWObj.tcProgramFQN).FileVersion, TrueCryptSWObj.tcDataUpgrade) < 0)
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
            int input; float inputf=0;
            if (!Int32.TryParse(newFileSizeMB.Text, out input))
            {
                if (gBytes.Checked && Single.TryParse(newFileSizeMB.Text, out inputf))
                {
                    input = (int)(inputf * 1000);
                }
                else
                {
                    MessageBox.Show("Non Numeric or non Integer Megabyte entry in Volume Size\n\n\t" + newFileSizeMB.Text, TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (gBytes.Checked && inputf==0) { input *= 1000; }
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
                DriveInfo drv = new DriveInfo(travUSBDrv[(int)radioButton.Tag].drvName);
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
                        DrvInfo mydrive = new DrvInfo();
                        mydrive.drvName = logDrv.GetPropertyValue("Dependent").ToString().Substring(logDrv.GetPropertyValue("Dependent").ToString().Length - 3, 2);
                        DriveInfo drvInf = new DriveInfo(mydrive.drvName);
                        mydrive.volName = drvInf.VolumeLabel;
                        mydrive.combo = mydrive.drvName + " (" + mydrive.volName + ")";
                        mydrive.tcFilePoss = string.Empty;
                        travUSBDrv.Add(mydrive);
                    }
                }
            }
        }
    }
    public class DrvInfo
    {
        public string drvName;  //C: or G: whatever
        public string volName;  //Volume name
        public string combo;    //drvname (Volname)
        public string tcFilePoss;
    }

}
