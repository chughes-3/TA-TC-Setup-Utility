using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace TaxAide_TrueCrypt_Utility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.Normal application run with no arguments. 2 circumstances it is called by "Start Tax-Aide Drive"
        /// 1. No tpdata file exists and need to create it. This is detected in filelist module by detecting that program is running wither from the tax-aide dire (Hard drive) or the tax-aide traveler dir (traveler) in which case the only option offered is formatting
        /// 2. traveler drive detects that host needs to be upgraded in which case it passes the string argument "hostupg" then use filelist form initialize the setup all parameters but never show the form just create the tasklist
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //first check to see if already running
            Regex myPatt = new Regex(@"\((.*)\)"); //extract process friendly name from full process
            Process myProc = Process.GetCurrentProcess();
            Match myMatch = myPatt.Match(myProc.ToString());
            String myProcFriendly = myMatch.Value.Substring(1, myMatch.Length - 2);//get rid of parentheses
            Process[] myProcArray = Process.GetProcessesByName(myProcFriendly);
            if (myProcArray.GetLength(0) > 1)
            {
                MessageBox.Show("This program is already running", TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(1);
            }
            Log tcFileResizerLog = new Log(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TA Utility 4TC TY10.log");
            string hostUpgOrFormatCall;
            TrueCryptFile tcFileHDOld = new TrueCryptFile(); //setup object to hold original file information name path and if traveler plus methods
            TrueCryptFile tcFileTravOld = new TrueCryptFile();
            TasksBitField tasklist = new TasksBitField(); //setup object to hold flags of things to be done1
            TrueCryptSWObj TcSoftware = new TrueCryptSWObj();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
                hostUpgOrFormatCall = "";
            }
            else
            {
                hostUpgOrFormatCall = args[0];
            }
            //hostUpgOrFormatCall = "hostupg"; //for debug purposes format hostupg May also need a change in SetFormatClsStartProcess for debug purposes
            FileList InfoFromUser = new FileList(tasklist, tcFileHDOld, tcFileTravOld,hostUpgOrFormatCall);
            if (hostUpgOrFormatCall != "hostupg")
            {//user usage or call to format so need to actually show dialog box for interaction
                InfoFromUser.ShowDialog();
                if (InfoFromUser.DialogResult != DialogResult.OK)
                {
                    Environment.Exit(1);
                }
            }
            else
            {//have to upgrade host due to traveler insertion
                Log.WritWTime("Host upgrade requested by parameter call");
                InfoFromUser.Check4HostUpgrade(); // sets up necessary flags for upgrade
                if (TrueCryptFilesNew.tcFileHDSizeNew > 0)
                {
                    Log.WritWTime("Host tpdata.tc to be upgraded, new size = " + TrueCryptFilesNew.tcFileHDSizeNew.ToString() + "MB");
                }
            }
            Log.WritSection(string.Format("TaskList = 0x{0:X}", tasklist.taskList));
            tasklist.LogTasks();
            //Next  TC software object does tasklist
            //Environment.Exit(1);  //used to stop program here at the point where can examine Log but before and changes are done
            TcSoftware.TCSWObjDoTasks(tasklist,tcFileHDOld,tcFileTravOld);
        }
    }
}
