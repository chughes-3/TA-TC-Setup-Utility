using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;   // used to find all users or public desktops

using shelllink; //the shelllink namespace for creating shortcuts

namespace TaxAide_TrueCrypt_Utility
{
    #region TrueCryptSWObj Class
    
    class TrueCryptSWObj
    {
        readonly string tcSetupProgramName = "TrueCrypt Setup 6.3a.exe";    // these are strings MUST change when change the release of TC
        public static readonly string tcSetupVersion = "6.3"; // this is version below which upgrade will happen
        public static string tcProgramFQN = string.Empty;
        public static string tcProgramDirectory;
        public static int osVer;    //5=xp 6=vista or win7
        public static string tCryptRegEntry;
        TasksBitField tasklist;
        int hWnd;   //window handle for TrueCrypt main window (both setup an setup /u and format)
        string regKeyName = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; //used to store old file names

        #region TrueCryptSWobj constructor Static string initialization
        public TrueCryptSWObj() // initialize all static entries
        {
            //do initial analysis of state of system
            Log.WriteStrm.Write("OS Version = " + System.Environment.OSVersion.ToString() + " or ");
            if (System.Environment.OSVersion.ToString().Contains("6.1"))
            {
                Log.WriteStrm.WriteLine("Windows 7");
            }
            else if (System.Environment.OSVersion.ToString().Contains("6.0"))
            {
                Log.WriteStrm.WriteLine("Vista");
            }
            else if (System.Environment.OSVersion.ToString().Contains("5.1"))
            {
                Log.WriteStrm.WriteLine("Win XP");
            }
            else
            {
                MessageBox.Show("Unknown OS, Exiting", "TrueCrypt Utilities", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            osVer = Environment.OSVersion.Version.Major;
            tCryptRegEntry = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\TrueCryptVolume\Shell\open\command", "", "");
            Log.WriteStrm.WriteLine("TrueCrypt Registry Entry = " + tCryptRegEntry);
            if (tCryptRegEntry != null)
            {
                tcProgramFQN = tCryptRegEntry.Substring(1, tCryptRegEntry.Length - 10); //registry entry has a leading quote that needs to go
                tcProgramDirectory = tcProgramFQN.Substring(0, tcProgramFQN.Length - 14);
            }
        }
                        
        #endregion

        public void TCSWObjDoTasks(TasksBitField tasklistPassed,TrueCryptFile tcFileHDOld, TrueCryptFile tcFileTravOld)
        {
            tasklist = tasklistPassed;

     #region Do tasks in task list

     #region HD Rename old TC File(s)
            if (tasklist.IsOn(TasksBitField.Flag.hdTcfileOldRename))
            {   //registry stuff to be added
                tcFileHDOld.ReName("oldtpdata.tc");
                string fileNames = tcFileHDOld.FileNamePath;
                //check for existence of s file
                if (File.Exists(Path.GetFullPath(tcFileHDOld.FileNamePath + "\\tsdata.tc")))
                {//create a new tcfile obkject
                    TrueCryptFile tcFileHDoldS = new TrueCryptFile();
                    tcFileHDoldS.FileNamePath = Path.GetFullPath(tcFileHDOld.FileNamePath + "\\tsdata.tc");
                    tcFileHDoldS.ReName("oldtsdata.tc");
                    fileNames += tcFileHDoldS.FileNamePath;
                }
                Microsoft.Win32.Registry.SetValue(regKeyName, "TC4TAhdOld", fileNames);
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
                //Close any TrueCrypt Drives
                Process proc1 = Process.Start(tcProgramFQN, " /q /d /f");
                while (proc1.HasExited == false)
                {
                    Thread.Sleep(100);
                }
                if (proc1.ExitCode != 0)
                {
                    MessageBox.Show(" TrueCrypt Drive Closing failed in some way before uninstall\nPlease close the TrueCrypt Drives manually and restart\n   Exiting", "TrueCrypt Uninstall Driver");
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
                CopyFileFromThisAssembly(tcSetupProgramName, Environment.GetEnvironmentVariable("temp"));
                Log.WritSection("Installing  on Hard Drive " + tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4));
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
                CopyTAFilesFromThisAssembly(tcProgramDirectory);
            } 
     #endregion

     #region HD TC File Create/Format
            if (tasklist.IsOn(TasksBitField.Flag.hdTCFileFormat))
            {
                Log.WritWTime("Har drive TC formatting to be done here");
            } 
     #endregion

     #region Trav TC file move to HD
            if (tasklist.IsOn(TasksBitField.Flag.travTcfileOldCopy))
            {
                Log.WritWTime("Copy Old TC File to hard drive");
                string moveLoc = TrueCryptFilesNew.tcFilePathHDNew.Substring(0, TrueCryptFilesNew.tcFilePathHDNew.Length - FileList.tcFilename.Length) + "old" + FileList.tcFilename; // picks up path we are using on this system for tcfiles and standard new file name modified by old
                tcFileTravOld.TCFileMove(moveLoc);    // copies to root or public depending on xp or vista
                //Now let's set registry key for old file note traveler old file object path not updated by move
                Microsoft.Win32.Registry.SetValue(regKeyName, "TC4TATravOld", moveLoc);    // record in registry in case of reboot
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
                Directory.CreateDirectory(TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler"); //TA files copied at the end
                if (File.Exists(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName))    //COPY TC setup program so it is available to start script
                {   //copies TC setup file to traveler so traveler script has it to upgrade
                    File.Copy(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\" + tcSetupProgramName);
                }
                else
                {
                    CopyFileFromThisAssembly(tcSetupProgramName, TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler");
                }
                if (tCryptRegEntry != null)
                {
                    File.Copy(tcProgramDirectory + "\\TrueCrypt.exe", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.exe");
                    File.Copy(tcProgramDirectory + "\\TrueCrypt.sys", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt.sys");
                    File.Copy(tcProgramDirectory + "\\TrueCrypt Format.exe", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt Format.exe");
                    File.Copy(tcProgramDirectory + "\\TrueCrypt-x64.sys", TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\TrueCrypt-x64.sys");
                    Log.WriteStrm.WriteLine("Traveler TrueCrypt upgraded to latest TrueCrypt release by copying host\'s files");
                }
                else  // no TC on host so must use Traveler TC
                {   //have to extract TC files from setup since no TC on host have to cpy setup to temp
                    tcProgramFQN = TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 2) + "\\Tax-Aide_Traveler\\truecrypt.exe";
                    tcProgramDirectory = tcProgramFQN.Substring(0, tcProgramFQN.Length - 14);
                    CopyFileFromThisAssembly(tcSetupProgramName, Environment.GetEnvironmentVariable("temp"));
                    Log.WritSection("Traveler TrueCrypt being upgraded to latest TrueCrypt release by extracting from TC setup");
                    StartUpDriveTC(Environment.GetEnvironmentVariable("temp") + "\\" + tcSetupProgramName, "", "extract", tcSetupProgramName.Substring(0, tcSetupProgramName.Length - 4), TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 3) + "Tax-Aide_Traveler", "");
                }
                //TrueCrypt files setup copy Taxaide files here4♦
                CopyTAFilesFromThisAssembly(TrueCryptFilesNew.tcFilePathTravNew.Substring(0, 3) + "Tax-Aide_Traveler");
                Log.WritSection("Traveler TrueCrypt Program Path = " + tcProgramFQN);
            } 
     #endregion

     #region Trav tc File Create/Format
            if (tasklist.IsOn(TasksBitField.Flag.travtcFileFormat))
            {
                Log.WritWTime("Traveler TC formatting to be done here");
            } 
     #endregion
              
       #endregion
            
        }
        // Function to  setup and run TC install, uninstall on hard drive
        private void StartUpDriveTC(string prgPath,string prgOptions, string uninTxt, string mainWinTitle, string editBox1, string editBox2)
        {   //edit boxes for Volume location and size strings
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
                MessageBox.Show("Problem starting " + uninTxt + ", exception =" + e.Message.ToString(),"TrueCrypt Utilities",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            TCWin.mainWinTitle = mainWinTitle;
            while (Win32.FindWindow(null, mainWinTitle) == 0) //wait for program startup
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(500); //first window takes a while to stabalize
            hWnd = Win32.FindWindow("CustomDlg", mainWinTitle);//CustomDlg
            if (hWnd == 0)
            {
                Log.WritWTime("Automated TrueCrypt " + uninTxt + " failed.\nPlease " + uninTxt + " TrueCrypt manually then restart this program");
                MessageBox.Show("Automated TrueCrypt " + uninTxt + " failed.\nPlease " + uninTxt + " TrueCrypt manually then restart this program", "TrueCrypt Utilities", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
            TCWin.SetupDoActionList(uninTxt, editBox1, editBox2);   //sets up the table of actions used by winaction to runthrough windows
            int i = 0;
            while (Win32.FindWindow("CustomDlg", TCWin.mainWinTitle) != 0)
            {
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
            CopyFileFromThisAssembly("Start_Tax-Aide_Drive.exe", destDir);
            CopyFileFromThisAssembly("Stop_Tax-Aide_Drive.exe", destDir);
            DriveInfo drv = new DriveInfo(destDir.Substring(0, 2));
            if (drv.DriveType == DriveType.Removable)
            {
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
                desktopShortcut1.ShortCutFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\St0p Tax-Aide Drive.lnk";
                desktopShortcut1.Target = destDir + "\\" + "Stop_Tax-Aide_Drive.exe";
                desktopShortcut1.IconPath = destDir + "\\" + "decryption.ico";
                desktopShortcut1.Save();
                desktopShortcut1.Dispose();
            }

        }
        private void CopyFileFromThisAssembly(string filename, string destPath) 
        {
            //create a buffer
            byte[] dataBuffer = new byte[2048];
            int n = 2048;
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
    }
    #endregion

    public struct TrueCryptFilesNew //holds file paths for new names
    {
        public static string tcFilePathHDNew;
        public static string tcFilePathTravNew;
    }

    #region Truecrypt File Class or the File that is going to be resized or moved
    public class TrueCryptFile //This class is about the TC file that is going to be resized
    {
        public bool Traveler { get; set; }
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
                DriveInfo drv = new DriveInfo(fqdName.Substring(0, 2));
                if (drv.DriveType == DriveType.Removable)
                {
                    Traveler = true;
                }
                else
                {
                    Traveler = false;
                }
            }
        }
        public void TCFileMove(string newPathName)
        {
            //Debug.WriteLine(newPath);
            ProgressForm pBar1 = new ProgressForm("Copying File " + fqdName + " to " + newPathName);
//            try
            {
                //create a buffer
                byte[] dataBuffer = new byte[2048];
                int n = 2048;
                using (FileStream fsSource = new FileStream(fqdName, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream fsNew = new FileStream(newPathName, FileMode.Create, FileAccess.Write))
                    {
                        int stepSize = (int) fsSource.Length / 204800; //buffer size and 100 steps
                        int step = 0;
                        if (stepSize > 100) //only show progress bar for > 400MB
                        {
                            pBar1.Show();
                        } 
                        while (n > 0)
                        {
                            n = fsSource.Read(dataBuffer, 0, 2048);
                            fsNew.Write(dataBuffer, 0, n);
                            step++;
                            if (step >= stepSize)
                            {
                                pBar1.DoProgressStep();
                                step = 0;
                            }
                        }
                    }
                }
                File.Delete(fqdName);     //deletes old file
                //fqdName = newPathName;  //Do not update this object because too many things use original name moved name in registry
            }
//            catch (Exception e)
            {
 //               Console.WriteLine(e.ToString());
            }
        }
        public void ReName(string newName)
        {
            File.Move(fqdName, fqdName.Substring(0, fqdName.LastIndexOf("\\") + 1) + newName);
            fqdName = fqdName.Substring(0, fqdName.LastIndexOf("\\") + 1) + newName; // updated because used to load registry
        }
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
