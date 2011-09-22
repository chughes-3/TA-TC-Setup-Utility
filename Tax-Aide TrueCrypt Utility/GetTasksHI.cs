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
    public partial class GetTasksHI : Form
    {
        public static readonly string tcFilename = "TPDATA.TC"; //Used for new file name on old filename check for tsdata.tc existence when do file copy
        public const int defaultTCFileSize = 1500; //TY 2010 = 500, TY2009=340, ty2008=300 MB plus spare used by hdTCSize
        List<string> tcFilenamesOldTrav = new List<string>() { "tpdata.tc", "trdata.tc", "tqdata.tc" };
        readonly string taSWExist = "Start_Tax-Aide_Drive.exe"; //filename used to test for existence of Tax-Aide sw
        int travTcFilePossCount;
        int oldTravFileSize = 0;    // used in setting up form entries delared here because needed in several different methods
        int oldHDFileSize = 0;
        public static List<DrvInfo> travUSBDrv = new List<DrvInfo>(); //DrvInfo class defined at end of this file
        TasksBitField radioBut1 = new TasksBitField();
        TasksBitField radioBut2 = new TasksBitField();
        TasksBitField radioBut3 = new TasksBitField();
        TasksBitField tasklist1;
        TrueCryptFile tcFileHDOldLoc;
        TrueCryptFile tcFileTravOldLoc;
        private System.Reflection.Assembly assem;
        public GetTasksHI(TasksBitField tasklistpassed, TrueCryptFile tcFileHDOld, TrueCryptFile tcFileTravOld, string calledArg)
        {
            InitializeComponent();
            newFileSizeMB.Text = defaultTCFileSize.ToString() + " "; //space to offset numbers
            tasklist1 = tasklistpassed;
            tcFileHDOldLoc = tcFileHDOld;
            tcFileTravOldLoc = tcFileTravOld;
            taskChoice1.Text = "Text";
            assem = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName assemName = assem.GetName();
            //this.Text += " Ver " + assemName.Version.ToString();// used to put version into title
            //radioButton2.Enabled = false;

            #region Get data for decisions on hard drive and traveler drive
            //Build list of usb connected drives first so can test against it in tcfileobject instance
            if (calledArg != "format" || assem.Location.Substring(0, 2) != Environment.GetEnvironmentVariable("HOMEDRIVE"))
            {//ie do this if not being called from start tax-aide drive to do a format on the hard drive
                GetUSBDrives(); //returns travusbdrv which is logical drive name and vol label and a combo string tcfile poss to be set here            
            }
            //Analyze hard drive situation
            TrueCryptFilesNew.tcFileHDNewPath = Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename; //sets up new name will be changed next for vista/w7
            // Find out if hard drive tpdata exists
            if (File.Exists(Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename))
            {
                tcFileHDOldLoc.FileNamePath = Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename;
                TrueCryptFilesNew.tcFileHDNewPath = Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\" + tcFilename; //set wrongly for vista/w7 here fixed in next clause
            }
            if (DoTasksObj.osVer == 6)
            {
                TrueCryptFilesNew.tcFileHDNewPath = Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename;
                if (File.Exists(Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename))
                {
                    tcFileHDOldLoc.FileNamePath = Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename;
                    TrueCryptFilesNew.tcFileHDNewPath = Environment.GetEnvironmentVariable("PUBLIC") + "\\" + tcFilename;
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
                // Check for Traveler failed migration has to occur after have inof on usb flash drives available
                SetButtons4Traveler();
            }
            else
            {//just go straight to install on c drive
                SetButtons4HD();
                taskChoice3.Visible = false;
            }

            #endregion
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (taskChoice3.Checked == true)
            {
                //Clear tasks
                radioBut1.taskList = 0;
                radioBut2.taskList = 0;
                radioBut3.taskList = 0;
                if ((string)taskChoice3.Tag == "HD")
                {//change top buttons to HD
                    usbSelectionComboBox.Visible = false;
                    SetButtons4HD();
                    taskChoice3.Text = "Do Tasks on Traveler Drive ";
                    taskChoice3.Tag = "TRAV";
                }
                else
                {
                    SetButtons4Traveler();
                }
                newVolEQOldVol.Checked = false;
                taskChoice1.Checked = true;
            }
        }
        private void SetButtons4HD()
        {
            Check4FailedMigrationHD();
            taskChoice2.Visible = true; // in case we started with single traveler then switched to hard drive
            newFileSizeMB.Text = defaultTCFileSize.ToString() + " ";
            mBytes.Checked = true;
            if (DoTasksObj.tCryptRegEntry == null)
            {//just go straight to install on c drive
                if (String.IsNullOrEmpty(tcFileHDOldLoc.FileNamePath))
                {
                    taskChoice1.Text = "Install Software, Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    taskChoice2.Text = "Install TrueCrypt Software on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                }
                else //to cater to where people have done manual uninstall but left data drive there
                {
                    taskChoice1.Text = "Install Software, Resize TrueCrypt Volume on " + tcFileHDOldLoc.FileNamePath;
                    radioBut1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                    taskChoice2.Text = "Install TrueCrypt and Tax-Aide Software on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                }
                radioBut1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
            }
            else
            {// tc installed

                if (!string.IsNullOrEmpty(tcFileHDOldLoc.FileNamePath))
                {
                    taskChoice1.Text = "Resize TrueCrypt Volume on " + tcFileHDOldLoc.FileNamePath;
                    radioBut1.SetFlag(TasksBitField.Flag.hdTcfileOldRename);
                    radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    taskChoice2.Text = "Upgrade TrueCrypt and Tax-Aide Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTASwOldDelete);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);
                }
                else
                {
                    taskChoice1.Text = "Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                    radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                    taskChoice2.Text = "Upgrade TrueCrypt Software on (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")" + ", no Tax-Aide Icons or Scripts";
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    radioBut2.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                }
            }
        }

        private void SetButtons4Traveler()
        {
            // First check for failed migration
            if (travTcFilePossCount > 0)
            {
                int index = travUSBDrv.FindIndex(delegate(DrvInfo s) { return (s.tcFilePoss != string.Empty); });
                Check4FailedMigrationTrav(index);
            }
            newFileSizeMB.Text = defaultTCFileSize.ToString() + " ";//setup now - may will be changed later
            mBytes.Checked = true;
            if (travTcFilePossCount > 0)
            {
                taskChoice1.Tag = travUSBDrv.FindIndex(delegate(DrvInfo s) { return (s.tcFilePoss != string.Empty); });
                taskChoice1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)taskChoice1.Tag].combo;
                radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                taskChoice2.Tag = travUSBDrv.FindIndex(delegate(DrvInfo s) { return (s.tcFilePoss != string.Empty); });
                taskChoice2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)taskChoice2.Tag].combo;
                radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                taskChoice3.Text = "Do Tasks on Hard Drive ";
                taskChoice3.Tag = "HD";
                if (travUSBDrv.Count > 1) { SetupUSBComboBox(); }
            }
            else if (travUSBDrv.Count > 0)
            {
                taskChoice1.Text = "Create Traveler Volume on " + travUSBDrv[0].combo;
                taskChoice1.Tag = 0;
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                taskChoice3.Text = "Do Tasks on Hard Drive";
                taskChoice3.Tag = "HD";
                if (travUSBDrv.Count > 1)
                {
                    taskChoice2.Text = "Create Traveler Volume on " + travUSBDrv[1].combo;
                    radioBut2.SetFlag(TasksBitField.Flag.travtcFileFormat);
                    taskChoice2.Tag = 1;
                    if (travUSBDrv.Count > 2) { SetupUSBComboBox(); }
                }
                else { taskChoice2.Visible = false; } //only 1 traveler drive
            }
        }
        private void SetFormatClsStartProcess()
        {
            Log.WritWTime(assem.Location);
            //if (assem.Location.Contains("bin\\Debug")) //"bin\\debug" for debug purposes and "TrueCrypt\\Tax-Aide" for real
            if (assem.Location.Contains("TrueCrypt\\Tax-Aide")) //"bin\\debug" for debug purposes and "TrueCrypt\\Tax-Aide" for real
            {//We are on hard drive
                taskChoice1.Text = "Create TrueCrypt Volume on Hard Drive (" + Environment.GetEnvironmentVariable("HOMEDRIVE") + ")";
                radioBut1.SetFlag(TasksBitField.Flag.hdTCFileFormat);
                Log.WritWTime("Formatting TC drive on Hard Drive called from Start_Tax-Aide_Drive");
            }
            else
            {// we have Traveler
                string drv = assem.Location.Substring(0, 2);
                //Next get index in travusbdrv list

                Log.WritWTime("Formatting TC Drive on Traveler called from Start_Tax-Aide_Drive on Traveler " + drv);
                taskChoice1.Text = "Create Traveler Volume on " + drv;
                taskChoice1.Tag = travUSBDrv.FindIndex(delegate(DrvInfo s) { return (s.drvName == drv); });
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
            }
            taskChoice2.Visible = false;
            taskChoice3.Visible = false;
        }
        private void OK_Click(object sender, EventArgs e)
        {
            if (taskChoice1.Checked == true)
            {
                tasklist1.taskList |= radioBut1.taskList;   //do initial setup of tasks
                if (radioBut1.IsOn(TasksBitField.Flag.hdTCFileFormat)) //therefore resize or create a volume
                {
                    TrueCryptFilesNew.tcFileHDNewSize = CheckEditBox(taskChoice1, radioBut1);    // check on size error = 0
                    if (TrueCryptFilesNew.tcFileHDNewSize == 0)
                    {   // data entry error clear tasklist previously set
                        tasklist1.taskList = 0;
                        return;
                    }
                    if (!File.Exists(DoTasksObj.tcProgramDirectory + "\\Tax-Aide\\" + taSWExist) | !Directory.Exists(DoTasksObj.tcProgramDirectory))
                    {
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);   // to deal with case that only TC installed previously now want to create/resize
                    }
                    Check4HostUpgrade();
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))   // must be done first to set up size for traveler size checking
                {
                    tcFileTravOldLoc.FileNamePath = travUSBDrv[(int)taskChoice1.Tag].tcFilePoss;
                }
                if (radioBut1.IsOn(TasksBitField.Flag.travtcFileFormat)) //set by both resize and make new
                {//we have create a new traveler file NEW filename set in checkTravswexists
                    TrueCryptFilesNew.tcFileTravNewSize = CheckEditBox(taskChoice1, radioBut1);    // check on size error = 0
                    if (TrueCryptFilesNew.tcFileTravNewSize == 0) //checkedit box returns zero if data entry is zero
                    {   // data entry error clear tasklist previously set
                        tasklist1.taskList = 0;
                        return;
                    }
                    string drv = travUSBDrv[(int)taskChoice1.Tag].drvName;
                    if (DoTasksObj.tCryptRegEntry == null)
                    {// nothing on host so must setup fqn for traveler
                        DoTasksObj.tcProgramFQN = drv + "\\" + "\\Tax-Aide_Traveler\\truecrypt.exe";
                        DoTasksObj.tcProgramDirectory = DoTasksObj.tcProgramFQN.Substring(0, DoTasksObj.tcProgramFQN.Length - 14);
                    }
                    checkTravSwExists(drv); //checks if sw exists and sets flags for upgrades if necessary
                    //now check if need host upgrade to work with traveler
                    Check4HostUpgrade();
                }
            }
            else
            {
                if (taskChoice2.Checked == true)
                {
                    tasklist1.taskList |= radioBut2.taskList;   //do initial setup of tasks
                    //if (radioBut2.IsOn(TasksBitField.Flag.hdTcSwInstall))
                    //{
                    Check4HostUpgrade(); //whatever was selected need to check for upgrade
                    //}
                    if (radioBut2.IsOn(TasksBitField.Flag.travSwInstall))
                    {//we have a request to upgrade sw means tc vol MAY exist
                        string drv = travUSBDrv[(int)taskChoice2.Tag].drvName;
                        checkTravSwExists(drv); //checks if sw exists and sets flags for upgrades if necessary                        
                        if (travUSBDrv[(int)taskChoice2.Tag].tcFilePoss != string.Empty)
                        {// here when user has chosen upgrade trav software but not resize
                            tcFileTravOldLoc.FileNamePath = travUSBDrv[(int)taskChoice2.Tag].tcFilePoss;
                            if (tasklist1.IsOn(TasksBitField.Flag.travTASwOldDelete) && !tasklist1.IsOn(TasksBitField.Flag.travTASwOldIsver6_2))
                            {//we are dong full upgrade due to old sw, so must set data move but only if pre 6.2
                                tasklist1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                                tasklist1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                                // set the file size to be the same as old
                                FileInfo f = new FileInfo(tcFileTravOldLoc.FileNamePath);
                                TrueCryptFilesNew.tcFileTravNewSize = (int)Math.Ceiling(f.Length / (double)1048576);
                            }
                        }
                    }
                    if (radioBut2.IsOn(TasksBitField.Flag.travtcFileFormat))
                    {//we have create a new traveler file selected on red but 2
                        string drv = travUSBDrv[(int)taskChoice2.Tag].drvName;
                        TrueCryptFilesNew.tcFileTravNewSize = CheckEditBox(taskChoice2, radioBut2);
                        if (TrueCryptFilesNew.tcFileTravNewSize == 0)
                        {
                            tasklist1.taskList = 0;
                            return;
                        }
                        // we only get here if no traveler files exist on 2 usb drives but traveler sw may so check
                        if (tasklist1.IsOn(TasksBitField.Flag.travTASwOldDelete))
                        {//we do not have a tpdata but do have old sw setup delete old sw path
                            tcFileTravOldLoc.FileNamePath = travUSBDrv[(int)taskChoice2.Tag].drvName + "\\";
                        }
                        checkTravSwExists(drv); // that should get the correct task flags set
                        if (DoTasksObj.tCryptRegEntry == null)
                        {// nothing on host so must setup fqn for traveler
                            DoTasksObj.tcProgramFQN = drv + "\\" + "\\Tax-Aide_Traveler\\truecrypt.exe";
                            DoTasksObj.tcProgramDirectory = DoTasksObj.tcProgramFQN.Substring(0, DoTasksObj.tcProgramFQN.Length - 14);
                        }
                    }
                }
                else
                {
                    Log.WritSection("We Have a problem with radio button selection");
                    Log.WritWTime("Error info as we die >> Old Paths = " + tcFileTravOldLoc.FileNamePath + " & " + tcFileHDOldLoc.FileNamePath);
                    Log.WritWTime("HD Size = " + TrueCryptFilesNew.tcFileHDNewSize + "MB, Trav Size = " + TrueCryptFilesNew.tcFileTravNewSize + "MB");
                    MessageBox.Show("We have a fatal error");
                    Environment.Exit(1);
                }
            }
            //MessageBox.Show(selectedFilePath);
            //Log.WritSection(string.Format("tasklist = {0:X}",tasklist1.taskList));
            Log.WritWTime("Dialog Close >> Old Paths = " + tcFileTravOldLoc.FileNamePath + " & " + tcFileHDOldLoc.FileNamePath);
            Log.WritWTime("HD Size = " + TrueCryptFilesNew.tcFileHDNewSize + "MB, Trav Size = " + TrueCryptFilesNew.tcFileTravNewSize + "MB");
            Close();
            this.DialogResult = DialogResult.OK;
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!taskChoice1.Checked)
            {
                newVolEQOldVol.Visible = false; //The only time this is visible is when taskchoice 1 is slected
                if (taskChoice2.Checked & !radioBut2.IsOn(TasksBitField.Flag.travtcFileFormat))
                {//only time need vol size is if radio button 2 is on usb tc file create
                    groupBox2.Enabled = false;
                    sizeLabel.Visible = false;
                }
                else
                {//radiobutton 2 checked and file format (or radion3 but that is irrelevant)
                    groupBox2.Enabled = true;
                    if (radioBut2.IsOn(TasksBitField.Flag.travtcFileFormat))
                    {

                        string drvStr = travUSBDrv[(int)taskChoice2.Tag].drvName;
                        DriveInfo drv = new DriveInfo(drvStr);
                        int space = (int)(drv.TotalFreeSpace / 1048576);
                        if (!Directory.Exists(drvStr + "\\traveler"))
                        {
                            space -= 10; // for traveler software
                        }
                        if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                        {// we have old traveler vol
                            FileInfo f = new FileInfo(travUSBDrv[(int)taskChoice1.Tag].tcFilePoss);
                            oldTravFileSize = (int)(f.Length / 1048576);
                            space += oldTravFileSize;
                        }
                        string spaceStr = space.ToString();
                        newFileSizeMB.Text = spaceStr + " ";
                        mBytes.Checked = true;
                        spaceStr += "MB";
                        if (space > 9999)
                        {
                            space /= 1024;
                            spaceStr = space.ToString();
                            newFileSizeMB.Text = spaceStr + " ";
                            spaceStr += "GB";
                            gBytes.Checked = true;
                        }
                        sizeLabel.Text = drvStr + " has " + spaceStr + " of Available Space ";
                        if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                        {// we have old traveler vol
                            sizeLabel.Text += "with an existing " + oldTravFileSize.ToString() + "MB TrueCrypt Volume";
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
                    string drvStr = travUSBDrv[(int)taskChoice1.Tag].drvName;
                    DriveInfo drv = new DriveInfo(drvStr);
                    int space = (int)(drv.TotalFreeSpace / 1048576);
                    if (!Directory.Exists(drvStr + "\\traveler"))
                    {
                        space -= 10; // for traveler software
                    }
                    newVolEQOldVol.Visible = false; // in case do NOT have old file
                    oldTravFileSize = 0;
                    if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                    {// we have old traveler vol
                        FileInfo f = new FileInfo(travUSBDrv[(int)taskChoice1.Tag].tcFilePoss);
                        oldTravFileSize = (int)(f.Length / 1048576);
                        space += oldTravFileSize;
                    }
                    string spaceStr = space.ToString();
                    newFileSizeMB.Text = spaceStr + " ";
                    mBytes.Checked = true;
                    spaceStr += "MB";
                    if (space > 9999)
                    {
                        space /= 1024;
                        spaceStr = space.ToString();
                        newFileSizeMB.Text = spaceStr + " ";
                        spaceStr += "GB";
                        gBytes.Checked = true;
                    }
                    sizeLabel.Text = drvStr + " has " + spaceStr + " of Available Space ";
                    if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                    {// we have old traveler vol
                        sizeLabel.Text += "with an existing " + oldTravFileSize.ToString() + "MB TrueCrypt Volume";
                        newVolEQOldVol.Visible = true;
                    }
                    else
                    {
                        sizeLabel.Text = "                      " + sizeLabel.Text; //center the label
                    }
                    sizeLabel.Visible = true;
                }
                else
                {//radio button 1 but not traveler
                    if (!String.IsNullOrEmpty(tcFileHDOldLoc.FileNamePath))
                    {
                        FileInfo f = new FileInfo(tcFileHDOldLoc.FileNamePath);
                        oldHDFileSize = (int)(f.Length / 1048576);
                        sizeLabel.Text = "            " + "Existing Hard Drive TrueCrypt Volume is " + oldHDFileSize.ToString() + "MB";
                        newFileSizeMB.Text = oldHDFileSize.ToString() + " ";
                        mBytes.Checked = true;
                        sizeLabel.Visible = true;
                        newVolEQOldVol.Visible = true;
                    }
                    else
                    {
                        sizeLabel.Visible = false;
                        newVolEQOldVol.Visible = false;
                    }
                }
            }
        }
        void checkTravSwExists(string drv)
        {
            TrueCryptFilesNew.tcFileTravNewPath = drv + "\\" + tcFilename;
            if (Directory.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler"))
            {   //existence of this directory means at 6.2 plus
                string travVer = "";
                string travVerTA = "";
                if (File.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe"))
                {
                    travVer = FileVersionInfo.GetVersionInfo(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe").FileVersion;
                    travVerTA = FileVersionInfo.GetVersionInfo(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\Tax-Aide TrueCrypt Utility.exe").FileVersion;
                }
                Log.WritWTime(String.Format("travVer/Truecrypt = {0}, travVerTa = {1}",travVer,travVerTA));
                if (!File.Exists(drv.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe") || string.Compare(travVer, DoTasksObj.tcSetupVersion) < 0 || string.Compare(travVerTA,DoTasksObj.taTcUtilVersion) < 0 ) //caters to directory existing but no files in it or being at less than current release
                {
                    tasklist1.SetFlag(TasksBitField.Flag.travTASwOldDelete);
                    tasklist1.SetFlag(TasksBitField.Flag.travTASwOldIsver6_2);//Flag set to say>6.2 therfore NO data upgrade forced
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
            if (DoTasksObj.tCryptRegEntry != null)
            {
                string str = FileVersionInfo.GetVersionInfo(DoTasksObj.tcProgramFQN).FileVersion;
                if (string.Compare(FileVersionInfo.GetVersionInfo(DoTasksObj.tcProgramFQN).FileVersion, DoTasksObj.tcSetupVersion) < 0)
                {//upgrade on host
                    tasklist1.SetFlag(TasksBitField.Flag.hdTASwOldDelete);
                    tasklist1.SetFlag(TasksBitField.Flag.hdTcSwUninstall);
                    tasklist1.SetFlag(TasksBitField.Flag.hdTcSwInstall);
                    if (!string.IsNullOrEmpty(tcFileHDOldLoc.FileNamePath) || tasklist1.IsOn(TasksBitField.Flag.hdTCFileFormat))
                    {//in case of call to format from startTaxAideDrive get here with no old data file but file format set
                        tasklist1.SetFlag(TasksBitField.Flag.hdTaxaideSwInstall);//to make sure we install TA sw if data file does not need to be upgraded
                        Log.WritWTime(tcFileHDOldLoc.FileNamePath + ", " + FileVersionInfo.GetVersionInfo(DoTasksObj.tcProgramFQN).FileVersion);
                    }
                    else
                    {
                        Log.WritWTime("No Hard Drive TC File");
                    }
                    if (!string.IsNullOrEmpty(tcFileHDOldLoc.FileNamePath) && string.Compare(FileVersionInfo.GetVersionInfo(DoTasksObj.tcProgramFQN).FileVersion, DoTasksObj.tcDataUpgrade) < 0)
                    {
                        if (tasklist1.TestTrav())
                        {
                            if (DialogResult.Cancel == MessageBox.Show("The TrueCrypt Software on the hard drive has to be upgraded to be compatible with the Traveler Software on the USB drive. An existing TrueCrypt Volume has been detected. After the hard drive upgrade the contents of the old volume will be copied across to a newly created volume, you will be asked next for the new size of the hard drive volume", DoTasksObj.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand))
                            {
                                Environment.Exit(1);
                            }
                            //Need to get new vol size here for hard drive upgrade
                        }
                        else
                        {//not traveler so doing hard drive but need to test if upgrade spec'd
                            if (!tasklist1.IsOn(TasksBitField.Flag.hdTCFileFormat))
                            {// NO file format so we are doing upgrade but have to do data file
                                if (DialogResult.Cancel == MessageBox.Show("An existing hard drive TrueCrypt Volume has been detected.The TrueCrypt Software on the hard drive that is being upgraded is sufficiently old that this data volume must also be regenerated, you will be asked next for the new size of the hard drive volume", DoTasksObj.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand))
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
                        if (TrueCryptFilesNew.tcFileHDNewSize == 0)
                        {//Not previously set so we need to set it here we get to this point with a simple HD resize with an old release of TC SW
                            hdTCSize hdtcFileSize = new hdTCSize((int)tcFileHDOldLoc.size / 1000000);
                            hdtcFileSize.ShowDialog();
                            TrueCryptFilesNew.tcFileHDNewSize = hdtcFileSize.hdFileSizeNew;
                        }
                    }
                }
            }

        }

        private int CheckEditBox(RadioButton radioButton, TasksBitField radBut)
        {
            int input; float inputf = 0;
            newFileSizeMB.Text.Trim();
            if (!Int32.TryParse(newFileSizeMB.Text, out input))
            {
                if (gBytes.Checked && Single.TryParse(newFileSizeMB.Text, out inputf))
                {
                    input = (int)(inputf * 1000);
                }
                else
                {
                    MessageBox.Show("Non Numeric or non Integer Megabyte entry in Volume Size\n\n\t" + newFileSizeMB.Text, DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (gBytes.Checked && inputf == 0) { input *= 1000; }
            if (radBut.IsOn(TasksBitField.Flag.travtcFileFormat))
            {//we have traveler so must check sizing
                long oldFSize = 0;
                if (!string.IsNullOrEmpty(tcFileTravOldLoc.FileNamePath)) //ie have old file which is taking up flash room
                {
                    oldFSize = tcFileTravOldLoc.size;
                }
                DriveInfo drv = new DriveInfo(travUSBDrv[(int)radioButton.Tag].drvName);
                long maxSize = drv.TotalFreeSpace - 10000000 + oldFSize;
                if (maxSize / 1048576 < input)
                {
                    MessageBox.Show("Volume Size is larger that possible on this flash drive\n\t" + input.ToString() + " MBytes\n\nThe Maximum is " + (maxSize / 1048576).ToString() + " MBytes", DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newFileSizeMB.Text = "";
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (radBut.IsOn(TasksBitField.Flag.hdTCFileFormat) && radBut.IsOn(TasksBitField.Flag.hdTcfileOldRename))
            {// we have to do HD format check if old files and that size is appropriate
                long totalOldFileSize = tcFileHDOldLoc.size;
                string oldSDrvpath = tcFileHDOldLoc.FileNamePath.Substring(0, tcFileHDOldLoc.FileNamePath.LastIndexOf('\\')) + "\\tsdata.tc";
                if (File.Exists(oldSDrvpath))
                {
                    FileInfo f = new FileInfo(oldSDrvpath);
                    totalOldFileSize += f.Length;
                }
                totalOldFileSize /= 1048576;
                if (totalOldFileSize > input)
                {
                    if (DialogResult.No == MessageBox.Show("Are you sure you meant this Volume size\n" + input.ToString() + "MBytes\n\nThis is less than the size of the existing TrueCrypt file(s) \nwhich is " + totalOldFileSize.ToString() + "MBytes", DoTasksObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        newFileSizeMB.Focus();
                        return 0;
                    }
                }
            }
            if (input > 7000)
            {
                if (DialogResult.No == MessageBox.Show("Are you sure you meant this Volume size\n" + input.ToString() + "MB will take a really long time to format", DoTasksObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (input < 0)
            {
                MessageBox.Show("Negative entry in Volume Size\n\n\t" + newFileSizeMB.Text, DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                newFileSizeMB.Focus();
                return 0;
            }
            if (input < 5)
            {
                if (DialogResult.No == MessageBox.Show("Are you sure you meant this Volume size\n\n\t" + input.ToString() + "MB is really small??", DoTasksObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (newFileSizeMB.ForeColor == SystemColors.GrayText)
            {
                if (DialogResult.No == MessageBox.Show("There has been no entry in the Volume Size Box. Did you intend to use the default of " + newFileSizeMB.Text + "MBytes?", DoTasksObj.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    newFileSizeMB.Focus();
                    return 0;
                }
            }
            if (input == 0) { return 0; }
            return input;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Check4FailedMigrationHD()
        {
            string regKeyMigrationHD = (string)Microsoft.Win32.Registry.GetValue(DoTasksObj.regKeyName, "TFTAOld", ""); //get this here in case started with old file existing from previous run of utility
            string tcMigFilePath =  Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\"  + "oldtpdata.tc";
            if (DoTasksObj.osVer == 6)
            {
                string tcMigFilePath1 = Environment.GetEnvironmentVariable("PUBLIC") + "\\" + "oldtpdata.tc";
                if (!File.Exists(tcMigFilePath1) && File.Exists(tcMigFilePath))
                {//deals with case of v/W7 and oldtpdata exists at c root rather than public
                    tcMigFilePath1 = tcMigFilePath;
                }
                tcMigFilePath = tcMigFilePath1;  //sets up public as path for V/W7 unless old exists in root
            }
            if (File.Exists(TrueCryptFilesNew.tcFileHDNewPath) && (File.Exists(tcMigFilePath)))
            {// we have old and new files existing therefore maybe failed migration
                migrationFileActionForm migUserQuestion = new migrationFileActionForm();
                DialogResult diaResult = migUserQuestion.ShowDialog();
                if (diaResult == DialogResult.Yes) //delete old tc files HD
                {
                    if (regKeyMigrationHD != "")
                    {
                        string[] oldFilePaths = regKeyMigrationHD.Split(new char[] { ',' });
                        foreach (var item in oldFilePaths)
                        {
                            File.Delete(item);
                        }
                        Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(DoTasksObj.regsubKeyName);
                        rk.DeleteValue("TFTAOld");
                    }
                    else
                    {
                        File.Delete(tcMigFilePath);
                        File.Delete(tcMigFilePath.Substring(0, tcMigFilePath.LastIndexOf("\\") + 1) + "oldtsdata.tc");
                    }
                    return; //continue with utility
                }
                if (diaResult == DialogResult.Retry)
                {// delete existing new p and continue
                    File.Delete(tcFileHDOldLoc.FileNamePath); //int the unlikely event that regkey does not exist
                    tcFileHDOldLoc.FileNamePath = string.Empty;
                    return;
                }
            }
            else
            {
                if (regKeyMigrationHD != "" && !File.Exists(tcMigFilePath))
                {//We have reg key set but no old file
                    DialogResult mbResult = MessageBox.Show("There is a migration data flag set, but no old Hard Drive TPDATA file, \r\nDelete the Migration Flag?", DoTasksObj.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error); //Delete reg entries and return on OK, exit on cancel
                    if (mbResult == DialogResult.OK)
                    {
                        DeleteMigrationRegEntries(false);
                        return;
                    }
                    else
                        Environment.Exit(1);
                }
            }
        }

        private void Check4FailedMigrationTrav(int index2travUSBdrv)
        {//travdrv has tpdata on it. check for migration file on hd Use new HD location to find if old traveler file
            string regKeyMigrationTrav = (string)Microsoft.Win32.Registry.GetValue(DoTasksObj.regKeyName, "TFTATravOld", "");
            if (File.Exists(TrueCryptFilesNew.tcFileHDNewPath.Substring(0, TrueCryptFilesNew.tcFileHDNewPath.LastIndexOf("\\") + 1) + "oldtravtpdata.tc"))
            {
                migrationFileActionForm migUserQuestion = new migrationFileActionForm();
                // Change form text over to traveler questions
                migUserQuestion.delInitialExplain.Text = "A TrueCrypt TPDATA file (the P drive) exists on the Traveler Drive\n" + travUSBDrv[index2travUSBdrv].combo + "In addition an old Traveler TrueCrypt file exists \non the Hard Drive.This looks like a failed migration of user data from \nan old Traveler based Truecrypt file to a new Traveler TrueCrypt file.";
                migUserQuestion.delNewPStartTAutil.Text = "Delete the current Traveler TPDATA file. Continue the Tax-Aide Utility \nto create a new Traveler file and complete the migration that failed.";
                migUserQuestion.delOldTCFiles.Text = "Delete the old Traveler TrueCrypt file. Assumes data migration is \ndone correctly, and the old file is not needed. Warning - it \nwill be permanently deleted. Then continue the utility.";
                migUserQuestion.radBtGrpBox.Text += " for " + travUSBDrv[index2travUSBdrv].combo;
                DialogResult diaResult = migUserQuestion.ShowDialog();
                if (diaResult == DialogResult.Yes) //delete old tc file Trav
                {
                    if (regKeyMigrationTrav != "")
                    {
                        File.Delete(regKeyMigrationTrav);
                        Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(DoTasksObj.regsubKeyName);
                        rk.DeleteValue("TFTATravOld");
                    }
                    else
                        File.Delete(tcFileHDOldLoc.FileNamePath.Substring(0, tcFileHDOldLoc.FileNamePath.LastIndexOf("\\") + 1) + "oldtravtpdata.tc");
                    return; //continue with utility
                }
                if (diaResult == DialogResult.Retry)
                {// delete existing new p and continue
                    File.Delete(travUSBDrv[index2travUSBdrv].tcFilePoss);
                    travUSBDrv[index2travUSBdrv].tcFilePoss = string.Empty;
                    travTcFilePossCount--;
                    return;
                }
            }
            else
            {//no old traveler check if reg key exists
                if (regKeyMigrationTrav != "")
                {// we have reg key no old travler
                    DialogResult mbResult = MessageBox.Show("There is a migration data flag set, but no old Traveler TPDATA file, \r\nDelete the Traveler Migration Flag?", DoTasksObj.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error); //Delete reg entries and return on OK, exit on cancel
                    if (mbResult == DialogResult.OK)
                    {
                        DeleteMigrationRegEntries(true);
                        return;
                    }
                    else
                        Environment.Exit(1);
                }
            }
        }
        private void DeleteMigrationRegEntries(Boolean removable)
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(DoTasksObj.regsubKeyName);
            if (removable == false)
            {
                rk.DeleteValue("TFTAOld");
            }
            else
            {
                rk.DeleteValue("TFTATravOld");
            }
        }

        private void Edit_Entry(object sender, EventArgs e) //ystem.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, S
        {
            newFileSizeMB.Font = new Font(this.Font, FontStyle.Regular);
            newFileSizeMB.ForeColor = SystemColors.WindowText;
            newFileSizeMB.Text = "";
            newVolEQOldVol.Checked = false;
        }
        private void newVolEQOldVol_CheckedChanged(object sender, EventArgs e)
        {
            if (newVolEQOldVol.Checked == true)
            {
                newFileSizeMB.Font = new Font(this.Font, FontStyle.Regular);
                newFileSizeMB.ForeColor = SystemColors.WindowText;
                if (radioBut1.IsOn(TasksBitField.Flag.travTcfileOldCopy))
                {
                    newFileSizeMB.Text = oldTravFileSize.ToString();
                    mBytes.Checked = true;
                }
                if (radioBut1.IsOn(TasksBitField.Flag.hdTcfileOldRename))
                {
                    newFileSizeMB.Text = oldHDFileSize.ToString();
                    mBytes.Checked = true;
                }
            }
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
        private void SetupUSBComboBox()
        {
            usbSelectionComboBox.Visible = true;
            usbSelectionComboBox.SelectedIndex = 0;
            int count = 0;
            if (travUSBDrv.Count < 9) { count = travUSBDrv.Count; }
            else { count = 8; }
            for (int i = 0; i < count; i++)
            {
                string str = travUSBDrv[i].combo;
                if (travUSBDrv[i].tcFilePoss != string.Empty)
                {
                    str += " with " + travUSBDrv[i].tcFilePoss;
                }
                if (!usbSelectionComboBox.Items.Contains(str))
                {
                    usbSelectionComboBox.Items.Add(str);
                }
            }
        }

        private void usbSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usbSelectionComboBox.SelectedIndex == 0) { return; }    //0 is basic message so change nothing
            radioBut1.taskList = 0;
            radioBut2.taskList = 0;
            radioBut3.taskList = 0;
            int selectedDriveIndex = usbSelectionComboBox.SelectedIndex - 1;
            if (travUSBDrv[selectedDriveIndex].tcFilePoss != string.Empty)
            {//need 2 buttons for the drive cause TC file exists
                taskChoice1.Tag = selectedDriveIndex;
                taskChoice1.Text = "Resize Traveler Volume on " + travUSBDrv[(int)taskChoice1.Tag].combo;
                radioBut1.SetFlag(TasksBitField.Flag.travTcfileOldCopy);
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                taskChoice2.Tag = selectedDriveIndex;
                taskChoice2.Text = "Upgrade TrueCrypt Software on " + travUSBDrv[(int)taskChoice2.Tag].combo;
                radioBut2.SetFlag(TasksBitField.Flag.travSwInstall);
                taskChoice2.Visible = true;
            }
            else
            {//no tc file so just drive
                taskChoice1.Text = "Create Traveler Volume on " + travUSBDrv[selectedDriveIndex].combo;
                taskChoice1.Tag = selectedDriveIndex;
                radioBut1.SetFlag(TasksBitField.Flag.travtcFileFormat);
                taskChoice2.Visible = false;
            }
            taskChoice1.Checked = false;
            taskChoice1.Checked = true;    //forces update of size in groupbox plus makes sure something checked

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
