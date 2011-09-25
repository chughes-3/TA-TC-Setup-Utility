using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;   // used to find all users or public desktops
using Microsoft.VisualBasic.FileIO; //to use also add a reference to visual basic dll in references

using shelllink; //the shelllink namespace for creating shortcuts

namespace TaxAide_TrueCrypt_Utility
{
    #region DoTasksObj Class
    
    class DoTasksObj
    {
        readonly string tcSetupProgramName = "TrueCrypt Setup 7.0a.exe";    //  Checks for upgrades done in FileList.CHeck4Hostupgrade & FileList.checkTravSwExists these are strings MUST change when change the release of TC
        public static readonly string tcSetupVersion = "7.0"; // this is version below which TC software upgrade on trav of hd will happen
        public const string taTcUtilVersion = "0.42.7.0"; //this is version below which force a script upgrade - primarily a traveler issue DO NOT set this above level of currect software or problmes
        internal static readonly string tcDataUpgrade = "6.2"; //version below which we force a data file copy during upgrade
        public static readonly string tcDriveLetter = "P:";  //used to open drives to copy old to new
        internal static readonly string programName = "Tax-Aide TrueCrypt Utility.exe"; //used when copy this program to install directory
        public static readonly string mbCaption = "AARP Tax-Aide TrueCrypt Utility";
        public static string tcProgramFQN = string.Empty;
        public static string tcProgramDirectory;
        public static int osVer;    //5=xp 6=vista or win7
        public static string tCryptRegEntry;
        TasksBitField tasklist;
        IntPtr hWnd;   //window handle for TrueCrypt main window (both setup an setup /u and format)
        public ProgessOverall progOverall;
        delegate void ProgUpdateDelegate(string txtMessUpdate); // delegate for invoking progress window update 1 string parameter
        delegate void ProgUpdateDelegate1();//same as above but no argument
        ProgUpdateDelegate progUpdate; // prgress window methods that are invoked on a different thread
        ProgUpdateDelegate progUpdateLin2;
        ProgUpdateDelegate1 progUpdateLin2notVis;
        internal const string regKeyName = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; //used to store old file names
        internal const string regsubKeyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        #region DoTasksObj constructor Static string initialization
        public DoTasksObj() // initialize all static entries
        {
            //do initial analysis of state of system
            string str = string.Empty;
            if (System.Environment.OSVersion.ToString().Contains("6.1"))
            {
                str = "Windows 7";
            }
            else if (System.Environment.OSVersion.ToString().Contains("6.0"))
            {
                str = "Vista";
            }
            else if (System.Environment.OSVersion.ToString().Contains("5.1"))
            {
                str = "Win XP";
            }
            else
            {
                MessageBox.Show("Unknown OS, Exiting", DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            Log.WritSection("OS Version = " + System.Environment.OSVersion.ToString() + " or " + str);
            osVer = Environment.OSVersion.Version.Major;
            tCryptRegEntry = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\TrueCryptVolume\Shell\open\command", "", "");
            Log.WriteStrm.WriteLine("TrueCrypt Registry Entry = " + tCryptRegEntry);
            if (tCryptRegEntry != null)
            {
                tcProgramFQN = tCryptRegEntry.Substring(1, tCryptRegEntry.Length - 10); //registry entry has a leading quote that needs to go
                tcProgramDirectory = tcProgramFQN.Substring(0, tcProgramFQN.Length - 14);
                List<string> taTCDrvs = new List<string>() { "P:\\", "Q:\\", "R:\\", "S:\\" };
                foreach (var item in taTCDrvs)
                {
                    if (Directory.Exists(item))
                    {
                        tcDriveClose(item.Substring(0,1));   //Forces closing 
                    }
                }
            }
            if (Directory.Exists("P:\\"))
            {
            MessageBox.Show("The P Drive exists. Please close it and restart this program", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
            }
            if (Directory.Exists("S:\\"))
            {
                MessageBox.Show("The S Drive exists. Please close it and restart this program", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
                        
        #endregion

        public void DoTasks(TasksBitField tasklistPassed,TrueCryptFile tcFileHDOld, TrueCryptFile tcFileTravOld)
        {
            tasklist = tasklistPassed;
            TrueCryptFile tcFileHDoldS = null;
            progOverall = new ProgessOverall(); //get teh progress form initialized
            Thread progressThread = new Thread(new ThreadStart(progOverall.ProgShow)); // starts progress window in new thread initial text is in progshow method
            progressThread.Start();
            Thread.Sleep(200); //allow window to appear
            progUpdate = new ProgUpdateDelegate(progOverall.ProgUpdate); //delegate for later use in updating window
            progUpdateLin2 = new ProgUpdateDelegate(progOverall.ProgUpdate2); //used for second line in traveler section
            progUpdateLin2notVis = new ProgUpdateDelegate1(progOverall.ProgUpdLin2notVis); //used to make second line in traveler section not visible
            Log.WritSection("Starting TaskList");


     #region HD Rename old TC File(s)
            if (tasklist.IsOn(TasksBitField.Flag.hdTcfileOldRename))
            {
                Log.WritWTime("HD Renaming = " + tcFileHDOld.FileNamePath);
                tcFileHDOld.ReName("oldtpdata.tc");
                string fileNames = "";
                //check for existence of s file
                if (File.Exists(tcFileHDOld.FileNamePath.Substring(0,tcFileHDOld.FileNamePath.LastIndexOf('\\')) + "\\tsdata.tc"))
                {//create a new tcfile object
                    tcFileHDoldS = new TrueCryptFile();
                    tcFileHDoldS.FileNamePath = tcFileHDOld.FileNamePath.Substring(0,tcFileHDOld.FileNamePath.LastIndexOf('\\')) + "\\tsdata.tc";
                    Log.WritWTime("HD Renaming = " + tcFileHDoldS.FileNamePath);
                    tcFileHDoldS.ReName("oldtsdata.tc");
                    fileNames = tcFileHDoldS.FileNamePath +",";    // we know we will be adding p
                }
                fileNames += tcFileHDOld.FileNamePath;   //done to ensure order of copying later is s then P
                Microsoft.Win32.Registry.SetValue(regKeyName, "TFTAOld", fileNames);
            }            
     #endregion
            // Get all passwords here After we know whether HD S exists
            string regKeyMigrationHD = (string)Microsoft.Win32.Registry.GetValue(regKeyName, "TFTAOld", ""); //get this here in case started with old file existing from previous run of utility
            string regKeyMigrationTrav = (string)Microsoft.Win32.Registry.GetValue(regKeyName, "TFTATravOld", ""); //same for Traveler, this key not set at this point in a normal run of utility
            PasswordCollectForm passCollect = new PasswordCollectForm(tasklist, regKeyMigrationTrav != "",regKeyMigrationHD != "",tcFileHDoldS != null);
            if (tasklist.IsOn(TasksBitField.Flag.hdTCFileFormat) || tasklist.IsOn(TasksBitField.Flag.travtcFileFormat))
            {//need to get passwords
                passCollect.ShowDialog(); 
            }

     #region HD TA SW old Delete
            if (tasklist.IsOn(TasksBitField.Flag.hdTASwOldDelete))
            {
                Log.WritWTime("Removing HD Old TA Files");
                RemoveOldTaxAideFiles();
            }
     #endregion

     #region HD TC SW uninstall
            if (tasklist.IsOn(TasksBitField.Flag.hdTcSwUninstall))
            {   
                Log.WritWTime("Upgrade TrueCrypt on the Hard Drive");
                progOverall.Invoke(progUpdate, new object[] { "Hard Drive TrueCrypt Uninstall, re-install" });
                //Close any TrueCrypt Drives
                Process proc1 = Process.Start(tcProgramFQN, " /q /d /f");
                while (proc1.HasExited == false)
                {
                    Thread.Sleep(100);
                }
                if (proc1.ExitCode != 0)
                {
                    MessageBox.Show(" TrueCrypt Drive Closing failed in some way before uninstall\nPlease close the TrueCrypt Drives manually and restart\n   Exiting", mbCaption,MessageBoxButtons.OK,MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    Application.Exit();
                }
                Log.WritSection("Uninstalling Hard Drive TrueCrypt Version " + FileVersionInfo.GetVersionInfo(tcProgramFQN).FileVersion);
                TCWin.actionsList = new TcActionUninstall();   //sets up correct list of windows for tc uninstall
                StartUpDriveTC(tcProgramFQN.Substring(0, tcProgramFQN.Length - 4) + " setup.exe", " /u", "uninstall", "TrueCrypt Setup");
                Thread.Sleep(2000); //Let uninstall settle
                //now have to make sure uninstall has really gone
                Process[] myProcArray = Process.GetProcessesByName("TrueCrypt Setup");
                if (myProcArray.GetLength(0) > 0)
                {//the calling procedure is still running likely due to slow processor need to kill it.
                    Log.WritWTime("TrueCrypt uninstall did not exit properly have to kill it");
                    myProcArray[0].Kill();
                    Thread.Sleep(1000); //wait for kill to take effect
                }
        #region Delete any directories and files in truecrypt folder for 64 bit conversion
                string[] directories;
                if (Directory.Exists(tcProgramDirectory)) // make sure directory is truly gone because tc7.0 move to prog files from prog file(x86)
                {
                    directories = Directory.GetDirectories(tcProgramDirectory);
                    foreach (var dir in directories)
                    {
                        DeleteFilesInDir(dir);
                    }
                    DeleteFilesInDir(tcProgramDirectory); //finally the main directory
                } 
        #endregion
            } 
     #endregion

     #region HD TC SW Install
            if (tasklist.IsOn(TasksBitField.Flag.hdTcSwInstall))
            {
                progOverall.Invoke(progUpdate, new object[] { "Extracting TrueCrypt Setup File" });
                CopyFileFromThisAssembly(tcSetupProgramName, Environment.GetEnvironmentVariable("temp"));
                Log.WritSection("Installing  on Hard Drive " + tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4));
                progOverall.Invoke(progUpdate, new object[] { "TrueCrypt Hard Drive Install" });
                TCWin.actionsList = new TcActionInstall();
                StartUpDriveTC(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, "", "install", tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4));
                //setup static field entries in case this is first time install
                tCryptRegEntry = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\TrueCryptVolume\Shell\open\command", "", "");
                Log.WriteStrm.WriteLine("TrueCrypt Registry Entry = " + tCryptRegEntry);
                tcProgramFQN = tCryptRegEntry.Substring(1, tCryptRegEntry.Length - 10); //registry entry has a leading quote that needs to go
                tcProgramDirectory = tcProgramFQN.Substring(0, tcProgramFQN.Length - 14);
                File.Delete(Dlls.GetAllUsersDesktopFolderPath() + "\\TrueCrypt.lnk"); //gets newly installed truecrypt icon off desktop for xp
                if (DoTasksObj.osVer >= 6)
                {
                    File.Delete(Dlls.GetSharedDesktop() + "\\TrueCrypt.lnk"); //gets newly installed truecrypt icon off desktop for vista 7 
                }
            } 
     #endregion

     #region HD Tax-Aide SW Install
            if (tasklist.IsOn(TasksBitField.Flag.hdTaxaideSwInstall))
            {
                progOverall.Invoke(progUpdate, new object[] { "Copying Tax-Aide script files" });
                if (!Directory.Exists(tcProgramDirectory + "\\Tax-Aide"))
                {
                    Directory.CreateDirectory(tcProgramDirectory + "\\Tax-Aide");
                }
                CopyTAFilesFromThisAssembly(tcProgramDirectory + "\\Tax-Aide");
            } 
     #endregion

     #region HD TC File Create/Format and OLD File Copy
            if (tasklist.IsOn(TasksBitField.Flag.hdTCFileFormat))
            {
                progOverall.Invoke(progUpdate, new object[] { "TrueCrypt Volume Creation" });
                Log.WritSection("Start HD TrueCrypt Volume creation/Formatting");
                TcActionFormat fo = new TcActionFormat();
                TCWin.actionsList = fo;
                fo.SetEditBoxes(TrueCryptFilesNew.tcFileHDNewPath, TrueCryptFilesNew.tcFileHDNewSize.ToString(), passCollect.passHDnewP.Text + "\t" + passCollect.passHDnewP.Text);  // password entry by duplicating it and separating by tab because TCAction only does single text entries.
                StartUpDriveTC(tcProgramDirectory + "\\TrueCrypt Format.exe","", "format", "Truecrypt volume creation wizard");
                progOverall.Invoke(progUpdate, new object[] { "Migrating old Hard Drive TrueCrypt volume(s) files\r across to new Hard Drive TrueCrypt Volume" });
                //test for file move due to old file 
                if (regKeyMigrationHD != "") //thie string loaded early for password checking
                {//We have old files to move
                    Log.WritSection("Have to Migrate old HD TrueCrypt volume(s) files across to new TrueCrypt Volume");
                    string[] oldFilePaths = regKeyMigrationHD.Split(new char[] { ',' });
                    //Prepare the user
                    tcDriveOpen(TrueCryptFilesNew.tcFileHDNewPath, "NEW P", "", tcDriveLetter, passCollect.passHDnewP.Text);
                    foreach (var item in oldFilePaths)
                    {
                        if (item.Contains("oldtsdata"))
                        {
                            tcDriveOpen(item, "OLD S", "", "S",passCollect.passHDoldS.Text);
                        }
                        else { tcDriveOpen(item, "OLD P", "", "S", passCollect.passHDoldP.Text); }
                        try
                        {
                            Log.WritWTime("About to Copy old " + item);
                            CopyDirectories("s:\\", tcDriveLetter + "\\");
                        }
                        catch (Exception e)
                        {
                            Log.WritSection("Help = " + e.Message);
                            MessageBox.Show("A problem occurred during copying. Restart the Utility to allow the recovery routines to work. The exception was\n" + e.Message);
                            tcDriveClose("s");
                            tcDriveClose("p");
                            Environment.Exit(1);
                        }
                        tcDriveClose("s");
                    }
                    tcDriveClose(tcDriveLetter.Substring(0,1)); //close p after copying old drives across
                    foreach (var item in oldFilePaths)
                    {// s already closed in loop so now delete files
                        File.Delete(item); 
                    }
                    Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(regsubKeyName);
                    rk.DeleteValue("TFTAOld");
                }
            }
            if (!tasklist.TestTrav())
            {//no traveler
                progressThread.Abort();
            }
     #endregion

     #region Trav TC file move to HD
            if (tasklist.IsOn(TasksBitField.Flag.travTcfileOldCopy))
            {
                Log.WritSection("Copying Old traveler TC File to hard drive");
                progOverall.Invoke(progUpdate, new object[] { "Copy old Traveler TrueCrypt volume to the hard drive" });
                string moveLoc = TrueCryptFilesNew.tcFileHDNewPath.Substring(0, TrueCryptFilesNew.tcFileHDNewPath.Length - GetTasksHI.tcFilename.Length) + "oldtrav" + GetTasksHI.tcFilename; // picks up path we are using on this system for tcfiles and standard new file name modified by old
                try
                {
                    FileSystem.MoveFile(tcFileTravOld.FileNamePath, moveLoc, UIOption.AllDialogs);
                }
                catch (Exception e)
                {
                    Log.WritWTime("There was some kind of problem copying the existing TrueCrypt Traveler Volume. The exception is " + e.ToString());
                    MessageBox.Show("There was some kind of problem copying the existing TrueCrypt Traveler Volume. The exception is " + e.ToString() + "Exiting", DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);                    
                }
                //Now let's set registry key for old file note traveler old file object path not updated by move
                Microsoft.Win32.Registry.SetValue(regKeyName, "TFTATravOld", moveLoc);    // record in registry in case of reboot
                regKeyMigrationTrav = moveLoc; //setsup value for later testing for migration
            } 
     #endregion

     #region Trav SW old delete
            if (tasklist.IsOn(TasksBitField.Flag.travTASwOldDelete))
            {
                Log.WritWTime("Traveler TrueCrypt being upgraded=" + TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2)); //must use new filename for disk because old may not exist
                Log.WritWTime(" Now deleting old traveler files");
                if (tasklist.IsOn(TasksBitField.Flag.travTASwOldIsver6_2))
                {
                    RemoveTravelerTCFiles(TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler\\");
                }
                else
                {//pre 6.2 traveler
                    RemoveTravelerTCFiles(TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 3)); 
                }
            } 
     #endregion

     #region Trav Sw Install
            if (tasklist.IsOn(TasksBitField.Flag.travSwInstall))
            {
                progOverall.Invoke(progUpdate, new object[] { "Done deleting software, now for Traveler software install" });
                Directory.CreateDirectory(TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler"); //TA files copied at the end
                if (File.Exists(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName))    //COPY TC setup program so it is available to start script
                {   //copies TC setup file to traveler so traveler script has it to upgrade
                    progOverall.Invoke(progUpdateLin2, new object[] { "Copying TrueCrypt Setup from Windows\\temp directory" });
                    try
                    {
                        File.Copy(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler\\" + tcSetupProgramName, true);
                    }
                    catch (Exception e)
                    {
                        Log.WritWTime("Exception in TC setup file Copying is " + e.ToString());
                        MessageBox.Show("Error in Copying file see log File, Exiting", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);                        
                    }
                }
                else
                {
                    progOverall.Invoke(progUpdateLin2, new object[] { "Extracting TrueCrypt Setup" });
                    CopyFileFromThisAssembly(tcSetupProgramName, TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler");
                }
                if (tCryptRegEntry != null)
                {
                    progOverall.Invoke(progUpdateLin2, new object[] { "Copying TrueCrypt Software to Traveler\r from hard drive" });
                    try
                    {
                        File.Copy(tcProgramDirectory + "\\TrueCrypt.exe", TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe", true);
                        File.Copy(tcProgramDirectory + "\\TrueCrypt.sys", TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.sys", true);
                        progOverall.Invoke(progUpdateLin2, new object[] { "Copying TrueCrypt FORMATTING Software to Traveler\r from hard drive" });
                        File.Copy(tcProgramDirectory + "\\TrueCrypt Format.exe", TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt Format.exe", true);
                        File.Copy(tcProgramDirectory + "\\TrueCrypt-x64.sys", TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt-x64.sys", true);
                    }
                    catch (Exception e)
                    {
                        Log.WritWTime("Exception in Truecrypt Files Copying is " + e.ToString());
                        MessageBox.Show("Error in Copying files see log File, Exiting", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                    Log.WriteStrm.WriteLine("Traveler TrueCrypt upgraded to latest TrueCrypt release by copying host\'s files");
                }
                else  // no TC on host so must use Traveler TC
                {   //have to extract TC files from setup since no TC on host have to cpy setup to temp
                    progOverall.Invoke(progUpdateLin2, new object[] { "Extracting Tax-Aide TrueCrypt Software to Traveler" });
                    tcProgramFQN = TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 2) + "\\Tax-Aide_Traveler\\truecrypt.exe";
                    tcProgramDirectory = tcProgramFQN.Substring(0, tcProgramFQN.Length - 14);
                    CopyFileFromThisAssembly(tcSetupProgramName, Environment.GetEnvironmentVariable("temp"));
                    Log.WritSection("Traveler TrueCrypt being upgraded to latest TrueCrypt release by extracting from TC setup");
                    progOverall.Invoke(progUpdateLin2, new object[] { "Extracting TrueCrypt Software to Traveler" });
                    TcActionExtract ex = new TcActionExtract();
                    TCWin.actionsList = ex;
                    ex.SetPath(TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 3) + "Tax-Aide_Traveler");
                    StartUpDriveTC(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, "", "extract", tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4));
                }
                //TrueCrypt files setup copy Taxaide files here
                progOverall.Invoke(progUpdateLin2, new object[] { "Extracting Tax-Aide Scripts to Traveler" });
                CopyTAFilesFromThisAssembly(TrueCryptFilesNew.tcFileTravNewPath.Substring(0, 3) + "Tax-Aide_Traveler");
                Log.WritSection("TrueCrypt Program Path to be used for Traveler purposes = " + tcProgramFQN);
                progOverall.Invoke(progUpdateLin2notVis);
            } 
     #endregion

     #region Trav tc File Create/Format and OLD File copy
            if (tasklist.IsOn(TasksBitField.Flag.travtcFileFormat))
            {
                progOverall.Invoke(progUpdate, new object[] { "Create and Format Traveler TrueCrypt Volume" });
                Log.WritSection("Traveler TrueCrypt Volume formatting starting");
                TcActionFormat trfo = new TcActionFormat();
                TCWin.actionsList = trfo;
                trfo.SetEditBoxes(TrueCryptFilesNew.tcFileTravNewPath, TrueCryptFilesNew.tcFileTravNewSize.ToString(), passCollect.passTravNewP.Text + "\t" + passCollect.passTravNewP.Text); // specify password as a single text entry by the use of Tab key which is recognized in TCAction later
                StartUpDriveTC(tcProgramDirectory + "\\TrueCrypt Format.exe", "", "format", "Truecrypt volume creation wizard");
                //copy old files
                //test for file move due to old file 
                if (regKeyMigrationTrav != "")
                {//We have old files to move
                    Log.WritSection("Have to Migrate old Traveler files across to new TrueCrypt Volume");
                    progOverall.Invoke(progUpdate, new object[] { "Migrate old Traveler files across to new TrueCrypt Volume" });
                    tcDriveOpen(TrueCryptFilesNew.tcFileTravNewPath, "NEW P", "Traveler", tcDriveLetter, passCollect.passTravNewP.Text);
                    tcDriveOpen(regKeyMigrationTrav, "OLD", "Traveler", "S", passCollect.passTravOldP.Text);
                    try
                    {
                        Log.WritWTime("About to Copy old " + regKeyMigrationTrav);
                        CopyDirectories("s:\\", tcDriveLetter + "\\");
                    }
                    catch (Exception e)
                    {
                        Log.WritSection("Help = " + e.Message);
                        MessageBox.Show("A problem occurred during copying. Restart the Utility to allow the recovery routines to work. The exception was\n" + e.Message);
                        tcDriveClose("s");
                        tcDriveClose("p");
                        Environment.Exit(1);
                    }
                    tcDriveClose("s");
                    tcDriveClose(tcDriveLetter);
                    File.Delete(regKeyMigrationTrav);
                    Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                    rk.DeleteValue("TFTATravOld");
                }
            } 
     #endregion
              
            if (File.Exists(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName))
            {//a little housekeeping neatness
                File.Delete(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName);
            }
            progressThread.Abort();//close down status window
            Log.WritSection("Successfully Completed TaskList");
            MessageBox.Show("Tasks Successfully Completed", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        // Function to  setup and run TC install, uninstall on hard drive
        private void StartUpDriveTC(string prgPath,string prgOptions, string uninTxt, string mainWinTitle)
        {   //edit box1 for Volume location path and editbox 2 for size strings, prgpath is for truecrypt either HD or traveler, unintxt is action (install, uninstall, extract, format, Main winodw title differs for differing actions 
            Log.WriteStrm.WriteLine("StartUpDriveTC path=" + prgPath + " opt=" + prgOptions);
            Process proc2 = new Process();
            proc2.StartInfo.FileName = prgPath;
            proc2.StartInfo.Arguments = prgOptions;
            try
            {
                proc2.Start();
            }
            catch (Exception e)
            {
                Log.WritWTime("Problem starting " + uninTxt + ", exception =" + e.Message.ToString());
                MessageBox.Show("Problem starting " + uninTxt + ", exception =" + e.Message.ToString(),mbCaption,MessageBoxButtons.OK,MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                Environment.Exit(1);
            }
            TCWin.mainWinTitle = mainWinTitle;
            while (Win32.FindWindow(null, mainWinTitle) == IntPtr.Zero) //wait for program startup
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(1000); //first window takes a while to stabalize
            hWnd = Win32.FindWindow("CustomDlg", mainWinTitle);//CustomDlg
            if (hWnd == IntPtr.Zero)
            {
                Log.WritWTime("Automated TrueCrypt " + uninTxt + " failed.\nPlease " + uninTxt + " TrueCrypt manually then restart this program");
                MessageBox.Show("Automated TrueCrypt " + uninTxt + " failed.\nPlease " + uninTxt + " TrueCrypt manually then restart this program", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            while (Win32.IsWindowVisible((IntPtr) hWnd) == false)
            {
                Thread.Sleep(100); //again waiting for format to become visible belt and suspenders
                Log.WriteStrm.WriteLine(uninTxt + "start up window still set to non visible - we have entered this section of code");
            }
            int i = 0;
            while (Win32.FindWindow("CustomDlg", TCWin.mainWinTitle) != IntPtr.Zero)
            {
                if (uninTxt == "format")  // format seems to take extra time to unpack itself
                {
                    int staticTextLength = 0;
                    int count = 0;
                    while (staticTextLength < 60)
                    {
                        TCWin winExistTest = new TCWin(hWnd);    // need to make sure child windows exist - only need for format 
                        staticTextLength = winExistTest.staticText.Length;
                        count++;
                        Thread.Sleep(200);
                    } 
                    if (count > 1)
                    {
                        Log.WriteStrm.WriteLine(uninTxt + "Format window still < 60 chars " + count.ToString() + " Times");
                    } 
                }
                TCWin tcWin = new TCWin(hWnd); //create object for data associatied with window
                i = tcWin.DoActionWait();   //do the action specified in the List for this window and wait for the window to change
                if (i != 1)
                {
                    break;
                }
            }
            // Make sure the process has ended
            if (proc2.HasExited == false)
	            {
                    Thread.Sleep(500); 
            	} 
        }
        //Function to remove Taxaide files from desktop and TrueCrypt Program directories
        private void RemoveOldTaxAideFiles() //for Desktop
        {
            string desktopFolder;
            if (osVer < 6)
            {
                desktopFolder = Dlls.GetAllUsersDesktopFolderPath();
            }
            else
            {
                desktopFolder = Dlls.GetSharedDesktop();
            }
            //delete old manual
            string[] docs = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"TrueCrypt*Tax-Aide Manual*.pdf");
            if (docs.Length > 0)
            {
                File.Delete(docs[0]); 
            }
            List<string> files = new List<string>();
            if (Directory.Exists(tcProgramDirectory + "\\Tax-Aide"))
            {//newer release exists go after these files
                files.Add(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Stop Tax-Aide Drive.lnk");
                files.Add(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Start Tax-Aide Drive.lnk");
                files.AddRange(Directory.GetFiles(tcProgramDirectory + "\\Tax-Aide"));
            }
            else
            {// go after files in old release
                files.AddRange(Directory.GetFiles(desktopFolder, "*tc*"));
                files.AddRange(Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "tc*"));
                files.AddRange(Directory.GetFiles(tcProgramDirectory, "TC*"));
                files.AddRange(Directory.GetFiles(tcProgramDirectory, "*.ico"));
                files.AddRange(Directory.GetFiles(tcProgramDirectory, "Identify*"));
                files.AddRange(Directory.GetFiles(tcProgramDirectory, "uac*"));
                if (File.Exists(tcProgramDirectory + "\\EditV32.exe")) { files.Add(tcProgramDirectory + "\\EditV32.exe"); }
                if (File.Exists(tcProgramDirectory + "\\ExtTC.exe")) { files.Add(tcProgramDirectory + "\\ExtTC.exe"); }
            }
            foreach (var item in files)
            {
                try
                {
                    File.Delete(item);
                }
                catch (Exception e)
                {
                    Log.WritWTime("Tax-Aide old file delete failed, Exception = " + e.ToString());
                    Log.WritWTime("Non-Fatal Continuing");
                    continue;
                }
            }
            if (Directory.Exists(tcProgramDirectory + "\\Tax-Aide"))
            {
                try
                {
                    Directory.Delete(tcProgramDirectory + "\\Tax-Aide");
                }
                catch (Exception e)
                {
                    Log.WritWTime("Cannot Delete Tax-Aide Dir" + e.ToString());
                }
            }
            Log.WritWTime("Count of TaxAide script files removed from Hard Drive = " + files.Count.ToString());
        }
        private void RemoveTravelerTCFiles(string path) //for Traveler
        {
            List<string> files = new List<string>();
            try
            {
                if (!tasklist.IsOn(TasksBitField.Flag.travTASwOldIsver6_2))
                {
                    files.AddRange(Directory.GetFiles(path, "TC*")); 
                }
                files.AddRange(Directory.GetFiles(path, "truecrypt*"));
            }
            catch (System.NullReferenceException )
            {
                Log.WritWTime("null ref in remove Traveler files=" + path);
                return;               
            } 
            foreach (var item in files)
            {
                File.Delete(item);
            }
            File.Delete(path + "autorun.inf");
            File.Delete(path + "Configuration.xml");
            File.Delete(path + "StartTraveler.bat");
            File.Delete(path + "IdentifyTRAV.vbs");
            File.Delete(path + "decryption.ico");
            File.Delete(path + "IdentifyEXT.vbs");
            File.Delete(path + "StartExternal.bat");
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TC stop");
            if (tasklist.IsOn(TasksBitField.Flag.travTASwOldIsver6_2))
            {//6.2 or greater
                DeleteFilesInDir(path);
                File.Delete(path.Substring(0, 3) + "autorun.inf");
                File.Delete(path.Substring(0, 3) + "Start Traveler.bat");
                File.Delete(path.Substring(0, 3) + "Start Traveler.exe");
            }
        }
        private void CopyTAFilesFromThisAssembly(string destDir)
        {
            //CopyFileFromThisAssembly("decryption.ico", destDir);
            //CopyFileFromThisAssembly("encryption.ico", destDir);
            progOverall.Invoke(progUpdateLin2, new object[] { "Start Tax-Aide Drive Script Copying" });
            CopyFileFromThisAssembly("Start_Tax-Aide_Drive.exe", destDir);
            progOverall.Invoke(progUpdateLin2, new object[] { "Tax-Aide Scripts Copying" });
            CopyFileFromThisAssembly("Stop_Tax-Aide_Drive.exe", destDir);
            System.Reflection.Assembly assem = System.Reflection.Assembly.GetExecutingAssembly(); // to copy this file to folder
            string thisProgFilePath = assem.Location;
            File.Copy(thisProgFilePath, destDir + "\\" + programName, true);
            if (GetTasksHI.travUSBDrv.Exists(delegate(DrvInfo s) { return s.drvName.Equals(destDir.Substring(0, 2).ToUpper()); }))  //tests whether this is a usb connected drive
            {
                Log.WritWTime("Copying unique traveler files from Assembly to Drive");
                CopyFileFromThisAssembly("autorun.inf", destDir.Substring(0, 3));
                CopyFileFromThisAssembly("Start Traveler.exe", destDir.Substring(0, 3));
            }
            else
            {
                ShellLink desktopShortcut = new ShellLink();
                desktopShortcut.ShortCutFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Start Tax-Aide Drive.lnk";
                desktopShortcut.Target = destDir + "\\" + "Start_Tax-Aide_Drive.exe";
                desktopShortcut.IconPath = destDir + "\\" + "Start_Tax-Aide_Drive.exe";
                desktopShortcut.Save();
                desktopShortcut.Dispose();
                ShellLink desktopShortcut1 = new ShellLink();
                desktopShortcut1.ShortCutFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Stop Tax-Aide Drive.lnk";
                desktopShortcut1.Target = destDir + "\\" + "Stop_Tax-Aide_Drive.exe";
                desktopShortcut1.IconPath = destDir + "\\" + "Stop_Tax-Aide_Drive.exe";
                desktopShortcut1.Save();
                desktopShortcut1.Dispose();
            }
            progOverall.Invoke(progUpdateLin2, new object[] { "Tax-Aide Scripts Copying" });
            progOverall.Invoke(progUpdateLin2notVis);
        }
        private void CopyFileFromThisAssembly(string filename, string destPath) 
        {
            //create a buffer originally had 2k but 64k speeded things up
            byte[] dataBuffer = new byte[0x10000];
            int n = 0x10000;
            try
            {
                using (Stream fsSource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TaxAide_TrueCrypt_Utility.Embedded." + filename))
                {
                    using (FileStream fsNew = new FileStream(destPath + "\\" + filename, FileMode.Create, FileAccess.Write))
                    {
                        BinaryReader fsSourceBin = new BinaryReader(fsSource);
                        BinaryWriter fsNewBin = new BinaryWriter(fsNew);
                        while (n > 0)
                        {
                            n = fsSourceBin.Read(dataBuffer, 0, 0x10000);
                            fsNewBin.Write(dataBuffer, 0, n);
                        }
                        fsNewBin.Flush();// I would have thought using fixed this but one error on xp was fixed by this.
                    }
                }
            }
            catch (Exception e)
            {
                Log.WritWTime("Exception in Reflection File Copying is " + e.ToString());
                MessageBox.Show("Error in Copying file see log File, Exiting", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
        private void tcDriveOpen(string tcFilePath, string mbExt, string Traveler, string tcDrvLetter, string password)
        {
            //fire up truecrypt
            Log.WritWTime("Opening = " + tcFilePath + " for moving files");
            tcDrvLetter = tcDrvLetter.Substring(0, 1);  // make sure we have just a letter
            Process proc2 = Process.Start(tcProgramFQN, " /q /v " + tcFilePath + " /l" + tcDrvLetter + " /p \"" + password  + "\"");
            proc2.WaitForExit();
            if (proc2.ExitCode != 0)
            {
                MessageBox.Show("TrueCrypt volume opening failed in some way. Reboot may be needed, when you start the P drive the copying will be attempted again.\n\tExiting", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                Environment.Exit(1);
            }
        }
        private void tcDriveClose(string tcDrvLetter)
        {
            Process proc2 = Process.Start(tcProgramFQN, " /q /f /d" + tcDrvLetter);
            proc2.WaitForExit();
            if (proc2.ExitCode != 0)
            {
                MessageBox.Show("TrueCrypt volume Closing failed in some way\n\tExiting", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                Environment.Exit(1);
            }
        }
        private void CopyDirectories(string source, string dest)//Used to copy data from old tc file to new tc file
        {
            string[] dirList = Directory.GetDirectories(source);
            foreach (var item in dirList)
            {
                if (CheckDirHiddenorSys(new DirectoryInfo(item)) == false)
                {
                    Log.WritWTime("Copying " + item);
                    FileSystem.CopyDirectory(item, dest + item.Substring(item.LastIndexOf('\\') + 1), UIOption.AllDialogs); 
                }
            }
            string[] fileList = Directory.GetFiles(source);
            Log.WritWTime("Copying " + source + " root files");
            foreach (string str in fileList)
            {
                File.Copy(str , dest + str.Substring(str.LastIndexOf('\\')+1));
            }
        }
        private bool CheckDirHiddenorSys(DirectoryInfo dirinfo)//Do not copy hidden and system (Sys Vol and Recycle bin
        {           //using a Function call avoids creating ne dirinfo for every directory cannot seem to reassign existing dirinfo variable
            if ((dirinfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden | (dirinfo.Attributes & FileAttributes.System) == FileAttributes.System)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void DeleteFilesInDir(string dir)
        {
            string[] files;
            files = Directory.GetFiles(dir);
            try
            {
                foreach (var item in files)
                {
                    File.Delete(item);
                }
                Directory.Delete(dir);
            }
            catch (Exception e)
            {
                Log.WritSection("Exception in delete of file or directory" + e.ToString());
            }
        }

    }
    #endregion

    public struct TrueCryptFilesNew //holds file paths & sizes for new names
    {
        public static string tcFileHDNewPath;
        public static string tcFileTravNewPath;
        public static int tcFileHDNewSize;
        public static int tcFileTravNewSize;
    }

    #region Truecrypt File Class or the File that is going to be resized or moved
    public class TrueCryptFile //This class is about the TC file that is going to be resized
    {
        public bool Traveler { get; set; }
        public long size = 0;
        private string fqdName;
        public string FileNamePath
        {
            get
            {
                return fqdName;
            }
            set
            {
                fqdName = value;
                Traveler = false;   // initialize then change to traveler if tests successful StartsWith(fqdName.Substring(0, 2).ToUpper())
                if (fqdName != String.Empty)
                {
                    if (GetTasksHI.travUSBDrv.Exists(delegate(DrvInfo s) { return s.drvName.Equals(fqdName.Substring(0, 2).ToUpper()); }))
                    {//at this point travusb has drive  name then volume name in list
                        Traveler = true;
                    }
                    if (File.Exists(fqdName))
                    {
                        FileInfo f = new FileInfo(fqdName);
                        size = f.Length;
                    }
                }
            }
        }
        public void ReName(string newName)
        {
            try
            {
                File.Move(fqdName, fqdName.Substring(0, fqdName.LastIndexOf("\\") + 1) + newName);
            }
            catch (Exception e)
            {
                Log.WritWTime("There was some kind of problem renaming the existing TrueCrypt Volume. The exception is " + e.ToString());
                MessageBox.Show("There was some kind of problem renaming the existing TrueCrypt Volume. The exception is " + e.ToString() + "Exiting", DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            } 
            fqdName = fqdName.Substring(0, fqdName.LastIndexOf("\\") + 1) + newName; // updated because used to load registry
        }
        //Replaced by VB file methods
//        public void TCFileMove(string newPathName)
//        {
//            //Debug.WriteLine(newPath);
//            ProgressForm pBar1 = new ProgressForm("Copying File " + fqdName + " to " + newPathName);
////            try
//            {
//                //create a buffer
//                byte[] dataBuffer = new byte[2048];
//                int n = 2048;
//                using (FileStream fsSource = new FileStream(fqdName, FileMode.Open, FileAccess.Read))
//                {
//                    using (FileStream fsNew = new FileStream(newPathName, FileMode.Create, FileAccess.Write))
//                    {
//                        int stepSize = (int) fsSource.Length / 204800; //buffer size and 100 steps
//                        int step = 0;
//                        if (stepSize > 100) //only show progress bar for > 400MB
//                        {
//                            pBar1.Show();
//                        } 
//                        while (n > 0)
//                        {
//                            n = fsSource.Read(dataBuffer, 0, 2048);
//                            fsNew.Write(dataBuffer, 0, n);
//                            step++;
//                            if (step >= stepSize)
//                            {
//                                pBar1.DoProgressStep();
//                                step = 0;
//                            }
//                        }
//                    }
//                }
//                File.Delete(fqdName);     //deletes old file
//                //fqdName = newPathName;  //Do not update this object because too many things use original name moved name in registry
//            }
////            catch (Exception e)
//            {
// //               Console.WriteLine(e.ToString());
//            }
//        }
    }
    #endregion
    public class Log
    {
        public static StreamWriter WriteStrm; //declared as static so only 1 instance so get at it via class not object
        public static string logPathFile;
        public Log(string str)//by having class NOT static can declare constructor with an argument to pass string through
        {
            WriteStrm = new StreamWriter(str,true);
            logPathFile = str;
            WriteStrm.AutoFlush = true;
            WriteStrm.WriteLine("***");
            WriteStrm.WriteLine("***");
            WriteStrm.WriteLine("***");
            WriteStrm.WriteLine("            AARP Tax-Aide TrueCrypt Utilities Log Startup: " + DateTime.Now.ToString());
        }
        public static void WritWTime(string str)//declare as static so that can access via class ie the instantiated object name is never used in this class.
        {
            WriteStrm.WriteLine(DateTime.Now.ToLongTimeString() + ": " + str);
        }
        public static void WritSection(string str)// puts in space before writing.
        {
            WriteStrm.WriteLine(" ");
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
            //string str1 = System.Reflection.MethodInfo.GetCurrentMethod().Name;
            WriteStrm.WriteLine("Method Name = " + methodBase.Name);
            WriteStrm.WriteLine(DateTime.Now.ToLongTimeString() + ": " + str);
        }
    }
    public class Dlls
    {
        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
        private const int MAX_PATH = 260;
        private const int CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019;
        public static string GetAllUsersDesktopFolderPath()
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            SHGetFolderPath(IntPtr.Zero, CSIDL_COMMON_DESKTOPDIRECTORY, IntPtr.Zero, 0, sbPath);
            return sbPath.ToString();
        }
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHGetKnownFolderPath(ref Guid rfid, uint dwFlags, IntPtr hToken, out StringBuilder path);
        public static string GetSharedDesktop()
        {
            Guid FOLDERID_PublicDesktop = new Guid(0xC4AA340D, 0xF20F, 0x4863, 0xAF, 0xEF, 0xF8, 0x7E, 0xF2, 0xE6, 0xBA, 0x25);
            StringBuilder path = new StringBuilder(260);
            uint retval = SHGetKnownFolderPath(ref FOLDERID_PublicDesktop, 0, IntPtr.Zero, out path);
            //if (retval == 0)
            return path.ToString();
        }
    }

}
