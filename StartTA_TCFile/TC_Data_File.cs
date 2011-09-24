using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace StartTA_TCFile
{
    class TC_Data_File
    {
        const string dataFileName = "TPDATA.TC";
        public const string tCryptDriveLetter = "P";
        const string tCryptDriveName = tCryptDriveLetter + ":\\";
        const string shareNameLegacy = "TWSRVR_" + tCryptDriveLetter;
        public const string shareName = "TaxWiseServer_" + tCryptDriveLetter;
        internal string tpdataPath = string.Empty;
        string regKeyName = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; //used to store old file names
        string regsubKeyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
        string migration;
        ProgramData thisProginst;
        TrueCryptSWObj tcSWobj;
        enum shareCreateErrorCodes { Success = 0, AccessDenied = 2, UnknownFailure = 8, InvalidName = 9, InvalidLevel = 10, InvalidParameter = 21, DuplicateShare = 22, RedirectedPath = 23, UnknownDeviceorDirectory = 24, NetNameNotFound = 25 }
    #region TC_Data_File initialization
        internal TC_Data_File(ProgramData thisProg,TrueCryptSWObj tcswobjpassed)
        {
            thisProginst = thisProg;
            tcSWobj = tcswobjpassed;
            if (Directory.Exists(tCryptDriveName))
            {
                CloseTcFile(); //attempt a file close
                if (Directory.Exists(tCryptDriveName))
                {// test again after trial close
                    MessageBox.Show("The P Drive exists. Please close it and restart this program", thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1); 
                }
            }
            if (!Environment.GetEnvironmentVariable("HomeDrive").StartsWith(thisProginst.drvLetter, StringComparison.CurrentCultureIgnoreCase) | Environment.OSVersion.Version.Major < 6)
            {//traveler or xp
                tpdataPath = thisProginst.drvLetter + ":\\" + dataFileName;
            }
            else
            {//not traveler, not xp
                tpdataPath = Environment.GetEnvironmentVariable("Public") + "\\" + dataFileName;
            }
            //Find out if we have errors conditions
            // First see if migration registry key is set
            if (thisProginst.removable == false)
            {
                migration = (string)Microsoft.Win32.Registry.GetValue(regKeyName, "TFTAOld", "");
            }
            else
            {
                migration = (string)Microsoft.Win32.Registry.GetValue(regKeyName, "TFTATravOld", "");
            }
            if (migration == "")
            {
                if (File.Exists(tpdataPath))
                {
                    return; //non error path is return here error path below here 
                }

            //ERROR PATH BELOW HERE
#region Deal with Migration errors
		                else
                {
                    DialogResult mbResult = MessageBox.Show("The Tax-Aide TrueCypt data file does not exist, Create it?", thisProginst.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (mbResult == DialogResult.OK)
                    {
                        StartTAUtility("format"); // go create the file Use Format argument
                    }
                    else
                    {
                        Environment.Exit(1);
                    }
                }
            }
            // below this point have a registry entry when we should not have one means something went wrong in migration
            string migrationS = string.Empty;
            if (migration.Contains(","))
            {// we have old P & old S
                migrationS = migration.Substring(migration.IndexOf(",") + 1);
                migration = migration.Remove(migration.IndexOf(",")); // gets the P string
            }
            if (File.Exists(migration))
            {//We have reg entry plus old P
                if (File.Exists(tpdataPath))
                {
                    migrationFileActionForm migForm = new migrationFileActionForm();
                    if (thisProg.removable == true)
                    {//Setup migration form for traveler
                        DriveInfo drv = new DriveInfo(thisProginst.drvLetter);
                        string drvName = thisProginst.drvLetter + " (" + drv.VolumeLabel + ") ";
                        migForm.delInitialExplain.Text = "A TrueCrypt TPDATA file (the P drive) exists on the Traveler Drive\n" + drvName + "In addition an old Traveler TrueCrypt file exists \non the Hard Drive.This looks like a failed migration of user data from \nan old Traveler based Truecrypt file to a new Traveler TrueCrypt file.";
                        migForm.delNewPStartTAutil.Text = "Delete the current traveler TPDATA file, Start the Tax-Aide Utility \nto create a new Traveler TPDATA file and complete the migration \nthat failed. Effectively start the whole process over.";
                        migForm.delOldTCFiles.Text = "Delete the old Traveler TrueCrypt file, Assumes data migration is \ndone correctly, and the old file is not needed. Warning - it will be \npermanently deleted. Then open the traveler file as the P drive.";
                        migForm.radBtGrpBox.Text += " for " + drvName;
                    }
                    DialogResult mbResult = migForm.ShowDialog();
                    if (mbResult == DialogResult.OK)
                        return; // have to simply open p drive ignore state of old stuff
                    else if (mbResult == DialogResult.Retry)
                    {//Delete new tp File and start over
                        File.Delete(tpdataPath);
                        StartTAUtility("");
                    }
                    else if (mbResult == DialogResult.Yes)
                    {// delete old TC file(s) and open P drive
                        File.Delete(migration);
                        if (migrationS != string.Empty)
                            File.Delete(migrationS);
                        DeleteMigrationRegEntries();
                        return;
                    }
                }
                else
                {// have registry entry and old tpdataa no new tpdata
                    DialogResult mbResult = MessageBox.Show("An old TrueCrypt TPDATA file exists, with a migration flag set.\rNo new TrueCrypt TPDATA file exists, the Tax-Aide utility will be started to create a new data file, migration will happen automatically?", thisProginst.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (mbResult == DialogResult.OK)
                    {
                        StartTAUtility("");
                    }
                    else
                    {
                        Environment.Exit(1);
                    }
                }
            }
            else
            {// reg entry no old file
                if (File.Exists(tpdataPath))
                {
                    DialogResult mbResult = MessageBox.Show("There is a migration data flag set, but no old TPDATA file, \r\nDelete the Migration Flag?", thisProginst.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error); //Delete reg entries and return on OK, exit on cancel
                    if (mbResult == DialogResult.OK)
                    {
                        DeleteMigrationRegEntries();
                        return;
                    }
                    else
                        Environment.Exit(1);
                }
                else
                {
                    DialogResult mbResult = MessageBox.Show("There is a migration data flag set, but no old TPDATA file, \r\nDelete the Migration Flag?", thisProginst.mbCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (mbResult == DialogResult.OK)
                    {
                        DeleteMigrationRegEntries();
                        StartTAUtility("");
                    }
                    else
                        Environment.Exit(1);
                }
            }
	#endregion
 
        }
        private void DeleteMigrationRegEntries()
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(regsubKeyName);
            if (thisProginst.removable == false)
            {
                rk.DeleteValue("TFTAOld");
            }
            else
            {
                rk.DeleteValue("TFTATravOld");
            }
        }
    #endregion
        internal void OpenTcFile()
        {
            Process tcproc = new Process();
            tcproc.StartInfo.FileName = tcSWobj.tcProgramFQN;
            tcproc.StartInfo.Arguments = " /q /v " + tpdataPath + " /l" + tCryptDriveLetter;
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
                MessageBox.Show("Drive Opening failed in some way, Exiting", thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        internal void CloseTcFile()
        {
            Process tcproc = new Process();
            tcproc.StartInfo.FileName = tcSWobj.tcProgramFQN;
            tcproc.StartInfo.Arguments = " /q /d /f";
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
        internal void PDrivePerms()
        {
            DirectorySecurity dsec = Directory.GetAccessControl(tCryptDriveName);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            //get existing rule and check to see if everyong full is set
            foreach (FileSystemAccessRule ace in rules)
            {
                if (ace.IdentityReference.Value == "Everyone" && ace.FileSystemRights == FileSystemRights.FullControl)
                {
                    return; //Everyone full control already set
                }
            }
            try
            {
                SecurityIdentifier everyoneID = new SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
                dsec.AddAccessRule(new FileSystemAccessRule(everyoneID,FileSystemRights.FullControl,AccessControlType.Allow));
                dsec.AddAccessRule(new FileSystemAccessRule(everyoneID, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
                Directory.SetAccessControl(tCryptDriveName, dsec);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error changing the File permissions on the P Drive. The error is \r" + e.ToString());
            }
        }
        void StartTAUtility(string arg)
        {
            if (File.Exists(thisProginst.scriptExePath + "\\Tax-Aide TrueCrypt Utility.exe"))
            {
                try
                {
                    Process proc = Process.Start(thisProginst.scriptExePath + "\\" + TrueCryptSWObj.TAtcSetupProgramName, arg);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception on starting Tax-Aide TrueCrypt Utility\r\n" + e.ToString(), thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
            else
            {
                MessageBox.Show("Tax-Aide TrueCrypt Utility File does not exist", thisProginst.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Environment.Exit(1);
        }
        #region Sharing Drive Method(s)
        internal int SharePDrive()  //0 is share down ok non zero is bad news
        {
            ShareCreate(shareName, tCryptDriveName, "Tax-AideShare");
            ShareCreate(shareNameLegacy, tCryptDriveName, "Tax-AideShareLegacy");
            return 0;   // if there were errors the program will have exited before getting here.

        }

        private void ShareCreate(string ShareName, string FolderPath, string Description)
        {
            ManagementClass mgmtClass = new ManagementClass("Win32_Share");
            ManagementBaseObject inParams = mgmtClass.GetMethodParameters("Create");
            ManagementBaseObject outParams;
            inParams["Description"] = Description;
            inParams["Name"] = ShareName;
            inParams["Path"] = FolderPath;
            inParams["Type"] = 0x0; //disk drive
            inParams["Access"] = SecurityDescriptor();  //for Everyone full perms
            outParams = mgmtClass.InvokeMethod("Create", inParams, null);
            if ((uint)(outParams.Properties["ReturnValue"].Value) != 0)
            {
                string errCode = Enum.GetName(typeof(shareCreateErrorCodes), outParams.Properties["ReturnValue"].Value);
                MessageBox.Show(String.Format("Unable to share P Drive. The error message was {0}\n\nShareName = {1}\nFolderPath = {2}", errCode, ShareName, FolderPath), thisProginst.mbCaption);
                Environment.Exit(1);
            }
        }
        private ManagementObject SecurityDescriptor()
        {
            SecurityIdentifier sec = new SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            byte[] sidArray = new byte[sec.BinaryLength];
            sec.GetBinaryForm(sidArray, 0);
            ManagementObject Trustee = new ManagementClass(new ManagementPath("Win32_Trustee"), null);
            Trustee["Domain"] = "NT Authority";
            Trustee["Name"] = "Everyone";
            Trustee["SID"] = sidArray;
            ManagementObject ACE = new ManagementClass(new ManagementPath("Win32_Ace"), null);
            ACE["AccessMask"] = 2032127; // 0x1f01ff Full Access
            ACE["AceFlags"] = 3;    //Non-container and container child objects to inherit ace
            ACE["AceType"] = 0;     //defines access allowed (1 would be defining access denied
            ACE["Trustee"] = Trustee;
            ManagementObject SecDesc = new ManagementClass(new ManagementPath("Win32_SecurityDescriptor"), null);
            SecDesc["ControlFlags"] = 4;        //SE_DACL_present
            SecDesc["DACL"] = new object[] { ACE };
            return SecDesc;
        }

        #endregion
    }

}
