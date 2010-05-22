using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
//using System.Diagnostics;

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
            Log tcFileResizerLog = new Log(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\TA Utility 4TC.log");
            TrueCryptFile tcFileHDOld = new TrueCryptFile(); //setup object to hold original file information name path and if traveler plus methods
            TrueCryptFile tcFileTravOld = new TrueCryptFile();
            TasksBitField tasklist = new TasksBitField(); //setup object to hold flags of things to be done1
            TrueCryptSWObj TcSoftware = new TrueCryptSWObj();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FileList());
            FileList InfoFromUser = new FileList(tasklist,tcFileHDOld,tcFileTravOld);
            InfoFromUser.ShowDialog();
            tasklist.LogTasks();
            //Next  TC software object does tasklist
            TcSoftware.TCSWObjDoTasks(tasklist,tcFileHDOld,tcFileTravOld);
        }
    }
}
