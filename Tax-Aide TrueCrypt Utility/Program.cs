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
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //first check to see if already running
            Regex myPatt = new Regex(@"\((.*)\)"); //extract process friendly name from full process
            Process myProc = Process.GetCurrentProcess();
            Match myMatch = myPatt.Match(myProc.ToString());
            String myProcFriendly = myMatch.Value.Substring(1, myMatch.Length - 2);
            Process[] myProcArray = Process.GetProcessesByName(myProcFriendly);
            if (myProcArray.GetLength(0) > 1)
            {
                MessageBox.Show("This program is already running", TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(1);
            }
            Log tcFileResizerLog = new Log(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TA Utility 4TC TY11.log");
            TrueCryptFile tcFileHDOld = new TrueCryptFile(); //setup object to hold original file information name path and if traveler plus methods
            TrueCryptFile tcFileTravOld = new TrueCryptFile();
            TasksBitField tasklist = new TasksBitField(); //setup object to hold flags of things to be done1
            TrueCryptSWObj TcSoftware = new TrueCryptSWObj();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FileList());
            FileList InfoFromUser = new FileList(tasklist,tcFileHDOld,tcFileTravOld);
            InfoFromUser.ShowDialog();
            if (InfoFromUser.DialogResult != DialogResult.OK)
            {
                Environment.Exit(1);
            }
            Log.WritSection(string.Format("TaskList = 0x{0:X}", tasklist.taskList));
            tasklist.LogTasks();
            //Next  TC software object does tasklist
            //Environment.Exit(1);
            TcSoftware.TCSWObjDoTasks(tasklist,tcFileHDOld,tcFileTravOld);
        }
    }
}
