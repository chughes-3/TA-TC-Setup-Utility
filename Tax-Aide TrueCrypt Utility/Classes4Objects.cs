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
    #region TrueCryptSWObj Class
    
    class TrueCryptSWObj
    {
        readonly string tcSetupProgramName = "TrueCrypt Setup 6.3a.exe";    // these are strings MUST change when change the release of TC
        public static readonly string tcSetupVersion = "6.3"; // this is version below which upgrade will happen
        public static readonly string tcDriveLetter = "P:";  //used to open drives to copy old to new
        public static readonly string mbCaption = "AARP Tax-Aide TrueCrypt Utility";
        public static string tcProgramFQN = string.Empty;
        public static string tcProgramDirectory;
        public static int osVer;    //5=xp 6=vista or win7
        public static string tCryptRegEntry;
        TasksBitField tasklist;
        int hWnd;   //window handle for TrueCrypt main window (both setup an setup /u and format)
        public ProgessOverall progOverall;
        string regKeyName = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; //used to store old file names
        string regsubKeyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        #region TrueCryptSWobj constructor Static string initialization
        public TrueCryptSWObj() // initialize all static entries
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
                MessageBox.Show("Unknown OS, Exiting", TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
        }
                        
        #endregion

        public void TCSWObjDoTasks(TasksBitField tasklistPassed,TrueCryptFile tcFileHDOld, TrueCryptFile tcFileTravOld)
        {
            tasklist = tasklistPassed;
            progOverall = new ProgessOverall(); //get teh progress form initialized
            progOverall.statusText.Text = "Starting Tasklist";
            progOverall.Show();

     #region Do tasks in task list

     #region HD Rename old TC File(s)
            if (tasklist.IsOn(TasksBitField.Flag.hdTcfileOldRename))
            {   
                tcFileHDOld.ReName("oldtpdata.tc");
                string fileNames = "";
                //check for existence of s file
                if (File.Exists(tcFileHDOld.FileNamePath.Substring(0,tcFileHDOld.FileNamePath.LastIndexOf('\\')) + "\\tsdata.tc"))
                {//create a new tcfile obkject
                    TrueCryptFile tcFileHDoldS = new TrueCryptFile();
                    tcFileHDoldS.FileNamePath = tcFileHDOld.FileNamePath.Substring(0,tcFileHDOld.FileNamePath.LastIndexOf('\\')) + "\\tsdata.tc";
                    tcFileHDoldS.ReName("oldtsdata.tc");
                    fileNames = tcFileHDoldS.FileNamePath +",";    // we know we will be adding p
                }
                fileNames += tcFileHDOld.FileNamePath;   //done to ensure order of copying later is s then P
                Microsoft.Win32.Registry.SetValue(regKeyName, "TFTAOld", fileNames);
            }            
     #endregion

     #region HD TA SW old Delete
            if (tasklist.IsOn(TasksBitField.Flag.hdTASwOldDelete))
            {
                RemoveOldTaxAideFiles();
            }
     #endregion

     #region HD TC SW uninstall
            if (tasklist.IsOn(TasksBitField.Flag.hdTcSwUninstall))
            {   
                Log.WritWTime("Upgrade TrueCrypt on the Hard Drive");
                progOverall.statusText.Text = "Hard Drive TrueCrypt Uninstall, reinistall";
                progOverall.Update();
                Thread.Sleep(200);
                //Close any TrueCrypt Drives
                Process proc1 = Process.Start(tcProgramFQN, " /q /d /f");
                while (proc1.HasExited == false)
                {
                    Thread.Sleep(100);
                }
                if (proc1.ExitCode != 0)
                {
                    MessageBox.Show(" TrueCrypt Drive Closing failed in some way before uninstall\nPlease close the TrueCrypt Drives manually and restart\n   Exiting", mbCaption,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                    Application.Exit();
                }
                Log.WritSection("Uninstalling Hard Drive TrueCrypt Version " + FileVersionInfo.GetVersionInfo(tcProgramFQN).FileVersion);
                StartUpDriveTC(tcProgramFQN.Substring(0, tcProgramFQN.Length - 4) + " setup.exe", " /u", "uninstall", "TrueCrypt Setup", "", "");
                Thread.Sleep(2000); //Let uninstall settle
                // MessageBox.Show("the pause that refreshes"); ; 
            } 
     #endregion

     #region HD TC SW Install
            if (tasklist.IsOn(TasksBitField.Flag.hdTcSwInstall))
            {
                progOverall.statusText.Text = "Extracting TrueCrypt Setup File";
                progOverall.Update();
                CopyFileFromThisAssembly(tcSetupProgramName, Environment.GetEnvironmentVariable("temp"));
                Log.WritSection("Installing  on Hard Drive " + tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4));
                progOverall.statusText.Text = "TrueCrypt Hard Drive Install";
                progOverall.Update();
                StartUpDriveTC(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, "", "install", tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4), "", "");
                //setup static field entries in case this is first time install
                tCryptRegEntry = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\TrueCryptVolume\Shell\open\command", "", "");
                Log.WriteStrm.WriteLine("TrueCrypt Registry Entry = " + tCryptRegEntry);
                tcProgramFQN = tCryptRegEntry.Substring(1, tCryptRegEntry.Length - 10); //registry entry has a leading quote that needs to go
                tcProgramDirectory = tcProgramFQN.Substring(0, tcProgramFQN.Length - 14);
                File.Delete(Dlls.GetAllUsersDesktopFolderPath() + "\\TrueCrypt.lnk"); //gets newly installed truecrypt icon off desktop
            } 
     #endregion

     #region HD Tax-Aide SW Install
            if (tasklist.IsOn(TasksBitField.Flag.hdTaxaideSwInstall))
            {
                progOverall.statusText.Text = "Copying Tax-Aide script files";
                progOverall.Update();
                Thread.Sleep(200);
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
                progOverall.statusText.Text = "TrueCrypt Volume Creation";
                progOverall.Update();
                Thread.Sleep(300);
                Log.WritSection("Start HD TrueCrypt Volume creation/Formatting");
                StartUpDriveTC(tcProgramDirectory + "\\TrueCrypt Format.exe","", "format", "Truecrypt volume creation wizard", TrueCryptFilesNew.tcFilePathHDNew, TrueCryptFilesNew.tcFileHDSizeNew.ToString());
                if (tasklist.TestTrav() == false)
                {
                    progOverall.Close(); //close the progress window
                }
                //test for file move due to old file 
                string str1 = (string)Microsoft.Win32.Registry.GetValue(regKeyName, "TFTAOld", "");
                if (str1 != "")
                {//We have old files to move
                    Log.WritSection("Have to Migrate old HD TrueCrypt volume(s) files across to new TrueCrypt Volume");
                    MessageBox.Show("We now have to copy the contents of the old TrueCrypt Volume to the New TrueCrypt Volume. You will be asked first for the password to the new Truecrypt Volume and then the password for the old TrueCrypt Volume. Then the contents of the old volume will be copied to the new Volume. If there are 2 old volumes the program will then go on to ask you for the password to the second old volume so that copying can be done", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    string[] oldFilePaths = str1.Split(new char[] { ',' });
                    //Prepare the user
                    tcDriveOpen(TrueCryptFilesNew.tcFilePathHDNew, "NEW P", "", tcDriveLetter);
                    foreach (var item in oldFilePaths)
                    {
                        if (item.Contains("oldtsdata"))
                        {
                            tcDriveOpen(item, "OLD S", "", "S");
                        }
                        else { tcDriveOpen(item, "OLD P", "", "S"); }
                        try
                        {
                            Log.WritWTime("About to Copy old " + item);
                            CopyDirectories("s:\\", tcDriveLetter + "\\");
                        }
                        catch (Exception e)
                        {
                            Log.WritSection("Help = " + e.Message);
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
     #endregion

     #region Trav TC file move to HD
            if (tasklist.IsOn(TasksBitField.Flag.travTcfileOldCopy))
            {
                Log.WritWTime("Copying Old TC File to hard drive");
                progOverall.statusText.Text = "Copy old Traveler TrueCrypt volume to the hard drive";
                progOverall.Update();
                Thread.Sleep(200);
                string moveLoc = TrueCryptFilesNew.tcFilePathHDNew.Substring(0, TrueCryptFilesNew.tcFilePathHDNew.Length - FileList.tcFilename.Length) + "oldtrav" + FileList.tcFilename; // picks up path we are using on this system for tcfiles and standard new file name modified by old
                try
                {
                    FileSystem.MoveFile(tcFileTravOld.FileNamePath, moveLoc, UIOption.AllDialogs);
                }
                catch (Exception e)
                {
                    Log.WritWTime("There was some kind of problem copying the existing TrueCrypt Traveler Volume. The exception is " + e.ToString());
                    MessageBox.Show("There was some kind of problem copying the existing TrueCrypt Traveler Volume. The exception is " + e.ToString() + "Exiting", TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);                    
                }
                //Now let's set registry key for old file note traveler old file object path not updated by move
                Microsoft.Win32.Registry.SetValue(regKeyName, "TFTATravOld", moveLoc);    // record in registry in case of reboot
            } 
     #endregion

     #region Trav SW old delete
            if (tasklist.IsOn(TasksBitField.Flag.travTASwOldDelete))
            {
                Log.WriteStrm.WriteLine("Traveler TrueCrypt being upgraded="+tcFileTravOld.FileNamePath.Substring(0, 3));
                
                RemoveTravelerTCFiles(tcFileTravOld.FileNamePath.Substring(0, 3));
            } 
     #endregion

     #region Trav Sw Install
            if (tasklist.IsOn(TasksBitField.Flag.travSwInstall))
            {
                progOverall.statusText.Text = "Done deleting software, now for Traveler software install";
                progOverall.Update();
                Directory.CreateDirectory(TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler"); //TA files copied at the end
                if (File.Exists(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName))    //COPY TC setup program so it is available to start script
                {   //copies TC setup file to traveler so traveler script has it to upgrade
                    progOverall.statusSecond.Text = "Copying TrueCrypt Setup from WIndows\\temp directory";
                    progOverall.statusSecond.Visible = true;
                    progOverall.Update();
                    Thread.Sleep(200);
                    try
                    {
                        File.Copy(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\" + tcSetupProgramName, true);
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
                    progOverall.statusSecond.Text = "Extracting TrueCrypt Setup";
                    progOverall.statusSecond.Visible = true;
                    progOverall.Update();
                    CopyFileFromThisAssembly(tcSetupProgramName, TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler");
                }
                if (tCryptRegEntry != null)
                {
                    progOverall.statusSecond.Text = "Copying TrueCrypt Software to Traveler from hard drive";
                    progOverall.Update();
                    try
                    {
                        File.Copy(tcProgramDirectory + "\\TrueCrypt.exe", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe", true);
                        File.Copy(tcProgramDirectory + "\\TrueCrypt.sys", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.sys", true);
                        File.Copy(tcProgramDirectory + "\\TrueCrypt Format.exe", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt Format.exe", true);
                        File.Copy(tcProgramDirectory + "\\TrueCrypt-x64.sys", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt-x64.sys", true);
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
                    progOverall.statusSecond.Text = "Extracting TrueCrypt Software to Traveler";
                    progOverall.Update();
                    tcProgramFQN = TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\truecrypt.exe";
                    tcProgramDirectory = tcProgramFQN.Substring(0, tcProgramFQN.Length - 14);
                    CopyFileFromThisAssembly(tcSetupProgramName, Environment.GetEnvironmentVariable("temp"));
                    Log.WritSection("Traveler TrueCrypt being upgraded to latest TrueCrypt release by extracting from TC setup");
                    StartUpDriveTC(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, "", "extract", tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4), TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 3) + "Tax-Aide_Traveler", "");
                }
                //TrueCrypt files setup copy Taxaide files here
                progOverall.statusSecond.Text = "Extracting Tax-Aide Scripts to Traveler";
                progOverall.Update();
                CopyTAFilesFromThisAssembly(TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 3) + "Tax-Aide_Traveler");
                Log.WritSection("TrueCrypt Program Path to be used for Traveler purposes = " + tcProgramFQN);
                progOverall.statusSecond.Visible = false;
            } 
     #endregion

     #region Trav tc File Create/Format and OLD File copy
            if (tasklist.IsOn(TasksBitField.Flag.travtcFileFormat))
            {
                progOverall.statusText.Text = "Create and Format Traveler TrueCrypt Volume";
                progOverall.Update();
                Thread.Sleep(200);
                Log.WritSection("Traveler TrueCrypt Volume formatting starting");
                StartUpDriveTC(tcProgramDirectory + "\\TrueCrypt Format.exe", "", "format", "Truecrypt volume creation wizard", TrueCryptFilesNew.tcFilePathTravNew, TrueCryptFilesNew.tcFileTravSizeNew.ToString());
                //copy old files
                    progOverall.Close(); 
                //test for file move due to old file 
                string oldTravFile = (string)Microsoft.Win32.Registry.GetValue(regKeyName, "TFTATravOld", "");
                if (oldTravFile != "")
                {//We have old files to move
                    Log.WritSection("Have to Migrate old Traveler files across to new TrueCrypt Volume");
                    MessageBox.Show("We now have to copy the contents of the old TrueCrypt traveler Volume to the New TrueCrypt traveler Volume. You will be asked first for the password to the new Truecrypt Volume and then the password for the old TrueCrypt Volume. Then the contents of the old volume will be copied to the new Volume",mbCaption,MessageBoxButtons.OK,MessageBoxIcon.Information);

                    //Prepare the user
                    tcDriveOpen(TrueCryptFilesNew.tcFilePathTravNew, "NEW P", "Traveler", tcDriveLetter);
                    tcDriveOpen(oldTravFile, "OLD", "Traveler", "S");
                    try
                    {
                        Log.WritWTime("About to Copy old " + oldTravFile);
                        CopyDirectories("s:\\", tcDriveLetter + "\\");
                    }
                    catch (Exception e)
                    {
                        Log.WritSection("Help = " + e.Message);
                        Environment.Exit(1);
                    }
                    tcDriveClose("s");
                    tcDriveClose(tcDriveLetter);
                    File.Delete(oldTravFile);
                    Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                    rk.DeleteValue("TFTATravOld");
                }
            } 
     #endregion
              
       #endregion
            if (File.Exists(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName))
            {//a little housekeeping neatness
                File.Delete(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName);
            }
            Log.WritSection("Successfully Completed TaskList");
            MessageBox.Show("Tasks Successfully Completed", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Function to  setup and run TC install, uninstall on hard drive
        private void StartUpDriveTC(string prgPath,string prgOptions, string uninTxt, string mainWinTitle, string editBox1, string editBox2)
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
                MessageBox.Show("Problem starting " + uninTxt + ", exception =" + e.Message.ToString(),mbCaption,MessageBoxButtons.OK,MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            TCWin.mainWinTitle = mainWinTitle;
            while (Win32.FindWindow(null, mainWinTitle) == 0) //wait for program startup
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(1000); //first window takes a while to stabalize
            hWnd = Win32.FindWindow("CustomDlg", mainWinTitle);//CustomDlg
            if (hWnd == 0)
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
            TCWin.SetupDoActionList(uninTxt, editBox1, editBox2);   //sets up the table of actions used by winaction to runthrough windows
            int i = 0;
            while (Win32.FindWindow("CustomDlg", TCWin.mainWinTitle) != 0)
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
                i = tcWin.DoAction();   //do the action specified in the List for this window and wait for the window to change
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
        private void RemoveOldTaxAideFiles()
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
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(tcProgramDirectory, "TC*"));
            files.AddRange(Directory.GetFiles(tcProgramDirectory, "*.ico"));
            files.AddRange(Directory.GetFiles(tcProgramDirectory, "Identify*"));
            files.AddRange(Directory.GetFiles(tcProgramDirectory, "uac*"));
            files.AddRange(Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "tc*"));
            if (File.Exists(tcProgramDirectory + "\\EditV32.exe")){files.Add(tcProgramDirectory + "\\EditV32.exe");} 
            files.AddRange(Directory.GetFiles(desktopFolder, "*tc*"));
            if (File.Exists(tcProgramDirectory + "\\ExtTC.exe")){files.Add(tcProgramDirectory + "\\ExtTC.exe");}                
            foreach (var item in files)
            {
                File.Delete(item);
            }
            Log.WritWTime("Count of TaxAide script files removed = " + files.Count.ToString());
        }
        private void RemoveTravelerTCFiles(string drive)
        {
            List<string> files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(drive, "TC*"));
                files.AddRange(Directory.GetFiles(drive, "truecrypt*"));
            }
            catch (System.NullReferenceException )
            {
                Log.WritWTime("null ref in remove files=" + drive);
                return;               
            } 
            foreach (var item in files)
            {
                File.Delete(item);
            }
            File.Delete(drive + "autorun.inf");
            File.Delete(drive + "Configuration.xml");
            File.Delete(drive + "StartTraveler.bat");
            File.Delete(drive + "IdentifyTRAV.vbs");
            File.Delete(drive + "decryption.ico");
            File.Delete(drive + "IdentifyEXT.vbs");
            File.Delete(drive + "StartExternal.bat");
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TC stop");
        }
        private void CopyTAFilesFromThisAssembly(string destDir)
        {
            CopyFileFromThisAssembly("decryption.ico", destDir);
            CopyFileFromThisAssembly("encryption.ico", destDir);
            CopyFileFromThisAssembly("favicon.ico", destDir);
            progOverall.statusSecond.Text = "Start Tax-Aide Drive Script Copying";
            progOverall.statusSecond.Visible = true;
            progOverall.Update();
            CopyFileFromThisAssembly("Start_Tax-Aide_Drive.exe", destDir);
            progOverall.statusSecond.Text = "Stop Tax-Aide Drive Script Copying";
            progOverall.Update();
            CopyFileFromThisAssembly("Stop_Tax-Aide_Drive.exe", destDir);
            if (FileList.travUSBDrv.Exists(delegate(DrvInfo s) { return s.drvName.Equals(destDir.Substring(0, 2).ToUpper()); }))  //tests whether this is a usb connected drive
            {
                Log.WritWTime("Copying unique traveler files");
                CopyFileFromThisAssembly("Tax-AideDelete.bat", destDir);
                CopyFileFromThisAssembly("autorun.inf", destDir.Substring(0, 3));
                CopyFileFromThisAssembly("Start Traveler.bat", destDir.Substring(0, 3));
            }
            else
            {
                ShellLink desktopShortcut = new ShellLink();
                desktopShortcut.ShortCutFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Start Tax-Aide Drive.lnk";
                desktopShortcut.Target = destDir + "\\" + "Start_Tax-Aide_Drive.exe";
                desktopShortcut.IconPath = destDir + "\\" + "encryption.ico";
                desktopShortcut.Save();
                desktopShortcut.Dispose();
                ShellLink desktopShortcut1 = new ShellLink();
                desktopShortcut1.ShortCutFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Stop Tax-Aide Drive.lnk";
                desktopShortcut1.Target = destDir + "\\" + "Stop_Tax-Aide_Drive.exe";
                desktopShortcut1.IconPath = destDir + "\\" + "decryption.ico";
                desktopShortcut1.Save();
                desktopShortcut1.Dispose();
            }
            progOverall.statusSecond.Visible = false;
        }
        private void CopyFileFromThisAssembly(string filename, string destPath) 
        {
            //create a buffer
            byte[] dataBuffer = new byte[2048];
            int n = 2048;
            try
            {
                using (Stream fsSource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TaxAide_TrueCrypt_Utility.Embedded." + filename))
                {
                    using (FileStream fsNew = new FileStream(destPath + "\\" + filename, FileMode.Create, FileAccess.Write))
                    {
                        while (n > 0)
                        {
                            n = fsSource.Read(dataBuffer, 0, 2048);
                            fsNew.Write(dataBuffer, 0, n);
                        }
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
        private void tcDriveOpen(string tcFilePath, string mbExt, string Traveler, string tcDrvLetter)
        {
            MessageBox.Show("TrueCrypt software and Volumes are being upgraded for greater reliability and performance. Please enter the Password for the " + mbExt + " " + Traveler + " Volume next",mbCaption,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            //fire up truecrypt
            Log.WritWTime("Opening = " + tcFilePath + " for moving files");
            tcDrvLetter = tcDrvLetter.Substring(0, 1);  // make sure we have just a letter
            Process proc2 = Process.Start(tcProgramFQN, " /q /v " + tcFilePath + " /l" + tcDrvLetter);
            proc2.WaitForExit();
            if (proc2.ExitCode != 0)
            {
                MessageBox.Show("TrueCrypt volume opening failed in some way. Reboot may be needed, when you start the P drive the copying will be attempted again.\n\tExiting", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
        }
        private void tcDriveClose(string tcDrvLetter)
        {
            Process proc2 = Process.Start(tcProgramFQN, " /q /f /d" + tcDrvLetter);
            proc2.WaitForExit();
            if (proc2.ExitCode != 0)
            {
                MessageBox.Show("TrueCrypt volume Closing failed in some way\n\tExiting", mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
    }
    #endregion

    public struct TrueCryptFilesNew //holds file paths & sizes for new names
    {
        public static string tcFilePathHDNew;
        public static string tcFilePathTravNew;
        public static int tcFileHDSizeNew;
        public static int tcFileTravSizeNew;
    }

    #region Truecrypt File Class or the File that is going to be resized or moved
    public class TrueCryptFile //This class is about the TC file that is going to be resized
    {
        public bool Traveler { get; set; }
        public long size;
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
                if (FileList.travUSBDrv.Exists(delegate(DrvInfo s) {return s.drvName.Equals(fqdName.Substring(0, 2).ToUpper());} ))
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
        public void ReName(string newName)
        {
            try
            {
                File.Move(fqdName, fqdName.Substring(0, fqdName.LastIndexOf("\\") + 1) + newName);
            }
            catch (Exception e)
            {
                Log.WritWTime("There was some kind of problem renaming the existing TrueCrypt Volume. The exception is " + e.ToString());
                MessageBox.Show("There was some kind of problem renaming the existing TrueCrypt Volume. The exception is " + e.ToString() + "Exiting", TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            WriteStrm = new StreamWriter(str);
            logPathFile = str;
            WriteStrm.AutoFlush = true;
            WriteStrm.WriteLine("AARP Tax-Aide TrueCrypt Utilities Log Startup: " + DateTime.Now.ToString());
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
