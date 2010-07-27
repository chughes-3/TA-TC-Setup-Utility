using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace TaxAide_TrueCrypt_Utility
{
    public class NextWinAction
    {
        public string uniqueWinText;
        public string actionToBeDone;
        public string captionText;
        public string variableText;
        public TcAction.Action activeFunction;
    }
    //New TrueCrypot Version
    //Delete old form embedded and edd new version display properites and change "build action" to "embedded resource' 
    //change dialog box title. Do a search through entire project for old version and check all references
    //Classes4objects setup program name, sw version and data version to be upgraded  
    // Change Assembly versioning
    //
    public class TCWin //this class is all about the TrueCrypt winodws that are open when TC is running install uninstall or formatting
    {
         public static List<string> dlgBoxTitles = new List<string>()    // when have new TrueCrypt version must check dialog box titles
         {
             "TrueCrypt Setup",
             "Truecrypt Volume Creation Wizard",
             "TrueCrypt Setup 7.0",
         };
        public StringBuilder winText = new StringBuilder(1024); //for all window text as one long string
        public StringBuilder staticText = new StringBuilder(1024); //for all static window text as one long string
        public static string mainWinTitle;
        public bool winChanged;
        public int comboBoxCount;
        public List<WinCtrls> winCtrlList = new List<WinCtrls>();
        public struct WinCtrls
        {
            public string cntrlClass;
            public string caption;
            public int hCtrl;
        }
        public int hWndHandle;//handle for window associated with this object
        public int hWndDialog; // to track case of dialog box
        public static bool uninstallFailContinue;   //a boolean to remember passing through a dialog box when back at main window recursion loop
        public delegate bool Callback(int hWnd, int lParam); //Used to enum child windows in initialisation and check if windows changed
        public static object actionsList; //used to record which format,extract etc doing
        public TcAction.Action activeFunction;
        protected delegate void DoActDel(TCWin tcwin);//setup byinitial SetupDoActionList
        protected DoActDel DoAction;

   #region TC window Class object initializer to get all Window data Delegate and CallBack,plus setup action list
        public TCWin(int hWnd) //initial constructor
        {
            hWndHandle = hWnd;
            if (actionsList is TcActionExtract)
            {
                DoAction = ((TcActionExtract)actionsList).DoAction;
            }
            if (actionsList is TcActionUninstall)
            {
                DoAction = ((TcActionUninstall)actionsList).DoAction;
            }
            if (actionsList is TcActionInstall)
            {
                DoAction = ((TcActionInstall)actionsList).DoAction;
            }
            if (actionsList is TcActionFormat)
            {
                DoAction = ((TcActionFormat)actionsList).DoAction;
            }
            StringBuilder wintxt = new StringBuilder(256); //first entry on cntrl list is entry for main window
            StringBuilder wintxt1 = new StringBuilder(256);
            Win32.GetWindowText(hWnd, wintxt, 256);
            Win32.GetClassName(hWnd, wintxt1, 256);
            winCtrlList.Add(new WinCtrls { caption = wintxt.ToString(), hCtrl = hWnd, cntrlClass = wintxt1.ToString() });
            Callback myCallBack = new Callback(EnumChildGetValue);
            Win32.EnumChildWindows(hWnd, myCallBack, 0);
            //next lines sometimes useful for debugging
            //Log.WriteStrm.WriteLine(staticText.ToString()); // for those occasions when want to write all text that will be searched
            //Log.WriteStrm.WriteLine(winText.ToString());
        }
        public bool EnumChildGetValue(int hWnd, int lParam)
        {
            StringBuilder wintxt = new StringBuilder(256);
            int txtLen;
            StringBuilder wintxt1 = new StringBuilder(256);
            Win32.GetClassName(hWnd, wintxt1, 256);
            txtLen = Win32.GetWindowText(hWnd, wintxt, 256);
            if (wintxt1.ToString() == "ComboBox")
            {
                wintxt.Remove(0, wintxt.Length);
                //Win32.SendMessage(hWnd, (int)win32Message.WM_GETTEXT, (IntPtr)256, wintxt); //text may vary
                wintxt.Append("ComboBox" + comboBoxCount.ToString());
                comboBoxCount++;
            }
            winCtrlList.Add(new WinCtrls { caption = wintxt.ToString(), hCtrl = hWnd, cntrlClass = wintxt1.ToString() });
            //Next line logs all child window data only necessary for debugging
            //Log.WriteStrm.WriteLine("caption=" + wintxt.ToString() + "hcrtl=" + hWnd.ToString() + "cntrlClass=" + wintxt1.ToString());
            if (wintxt1.ToString() == "Static")
            {
                staticText.Append(wintxt.ToString()); 
            }
            if ((txtLen > 128))
            {
                winText.Append("\n");
            }
            winText.Append(wintxt.ToString() + "+");//Build string of all text in dialog box to search
            return true;
        }
   #endregion 

        public int DoActionWait()
        {
            DoAction(this);// goes to tcaction and does appropriate thing
// Wait for next window region
                int i = 0;
                while ((Win32.FindWindow("CustomDlg", mainWinTitle) != 0) & (winChanged != true))
                {//Main Wait loop
                    Thread.Sleep(200);
                    int j = TestWinUpdate(hWndHandle);
                    if (j == 1)
                    {
                        //we have a dialog box This may be recursive since one dialog boox may call another
                        Log.WritWTime("  " + "Dialog Box Found. Next Line shows which one");
                        TCWin dlgWin = new TCWin(hWndDialog);
                        i=dlgWin.DoActionWait(); //does action waits for change if change is another dlgbox recursively comes here
                        if (i != 1)
                        {
                            return 0; //break out of loop this indicates that we have a chnage and it is not a dialog box
                        }
                        hWndDialog = 0;
                       return 1;
                    }
                }
                Thread.Sleep(200); //to let window show before continue
            //Come out of action and wait loops here so test for exit conditions
            if (Win32.FindWindow("CustomDlg", mainWinTitle) == 0)   // normal exit when TrueCrypt process has disappeared
            {
                if (actionsList is TcActionExtract)
                {
                    i = ((TcActionExtract)actionsList).TestExit();
                }
                if (actionsList is TcActionUninstall)
                {
                    i = ((TcActionUninstall)actionsList).TestExit();
                }
                if (actionsList is TcActionFormat)
                {
                    i = ((TcActionFormat)actionsList).TestExit();
                }
                if (actionsList is TcActionInstall)
                {
                    i = ((TcActionInstall)actionsList).TestExit();
                }
                if (i == 0)  //we have normal exit
                {
                    Log.WritWTime("TrueCrypt Normal Exit");
                    return 0;
                }
                //WE WILL HAVE TO PUT THIS TEST IN IN tcaction where have access to r
                //if (winaction[r].captionText == "Exit" | winaction[r].captionText == "&Finish" )
                //{   //covers format (exit) and TC setup (&Finish is clicked)
                //    Log.WritWTime("TrueCrypt Normal Exit");
                //    return 0; //ends the loop
                //}
                foreach (WinCtrls item in winCtrlList)
                {
                    if (item.cntrlClass == "Button" & item.caption == "&Finish")
                    {   //Uninstall
                        Log.WritWTime("TrueCrypt Setup uninstall Normal Exit");
                        return 0; //ends the loop
                    }
                }
                if (uninstallFailContinue == true)
                {
                    Log.WritWTime("Uninstall failed due to Directory not empty - non Fatal - so Continue");
                    return 0;
                }
                Log.WriteStrm.WriteLine(DateTime.Now.ToString() + mainWinTitle + " Process has disappeared");
                MessageBox.Show(string.Format("The " + mainWinTitle + " Process has unexpectedly disappeared.\n A log file is located at {0}", Log.logPathFile), TrueCryptSWObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
            return 1;
        }

#region Test if window has changed Function or generated a dialog box
		public int TestWinUpdate(int hWnd)
        {
            hWndHandle = hWnd;
            // attempt to get top windows from thread but generated protection fault and picks up default ime window first 
            //Callback ThreadWinCallBack = new Callback(EnumThreadWndProc); //by testing Windows off a Thread we find dialog boxes
            //long threadID = Win32.GetWindowThreadProcessId(hWnd, (IntPtr)0);
            //Win32.EnumThreadWindows(threadID, ThreadWinCallBack, 0);
            foreach (string str in dlgBoxTitles)
            {
                int tmphWnd = Win32.FindWindow("#32770", str); //this is class code for dialog box
                if (tmphWnd != 0)
                {
                    if (winCtrlList[0].cntrlClass != "#32770")
                    {
                        hWndDialog = tmphWnd;// only set for dialog box if we are not in a dialog box
                        Thread.Sleep(200); //makes sure dialog box has all static text setup
                        return 1; //indicating a dialog box
                    }
                    else //we detect a dialog box and this object is for a dialog box
                    {
                        if (tmphWnd != hWndHandle)  //then we have another dialog window
                        {
                            Log.WriteStrm.WriteLine("Detected DlgBox while in Dlgbox tmphWND=" + tmphWnd.ToString() + " hWnddialog=" + hWndDialog.ToString() + " hWndHandle=" + hWndHandle.ToString());
                            hWndDialog = tmphWnd;// only set for dialog box
                            return 1; //indicating another dialog box while we are in a dialog box
                        }
                        else
                        {
                            return 0; //We are in a dialog box and do not have a new one return 0 to avoid uncontrolled recursion
                        }
                    }
                }                
            }
            //if dialog box there are no child windows once dialog gone away so cntllist will not change so change window will not be detected by logic below
            if (winCtrlList[0].cntrlClass == "#32770")
            {
                winChanged = true; //done to force exit from time loop when dealing with dialog box
                return 0; //ie not a new dialog box
            }
            //At this point any dialog box should be dealt with so now back to regular child windows
            Callback ChildWinCallBack = new Callback(EnumChildGethWnds);
            Win32.EnumChildWindows(hWnd, ChildWinCallBack, 0);
            return 0;
        }
        public bool EnumChildGethWnds(int ahWnd, int lParam)
        {
            StringBuilder wintxt = new StringBuilder(256);
            int txtLen = Win32.GetWindowText(ahWnd, wintxt, 256);
            foreach (WinCtrls item in winCtrlList)
            {
                if (item.hCtrl == ahWnd & item.cntrlClass != "Button")  //win not changed if hwnd known and control is not a button
                {  
                    return true;
                }
                else if (item.hCtrl == ahWnd & item.cntrlClass == "Button" & item.caption == wintxt.ToString()) //win not changed if hwnd known and control is a button and the button's text has not changed
                {
                    return true;
                }
            }
            //Log.WriteStrm.WriteLine("ahwnd=" + ahWnd.ToString() + " wintxt =" + wintxt.ToString());
            winChanged = true;
            return true;
        }
        //public bool EnumThreadWndProc(int ahWnd, int lParam) //Callback for windows thread process
        //{
        //    if (ahWnd != hWndHandle)
        //    {
        //        //We have a dialog box separate from main window setup hwnd
        //        hWndDialog = ahWnd;
        //    }
        //    return true;
        //}
        public string GetDlgCtrl(ref int i)
        {
            string str = winCtrlList[i].cntrlClass +" "+ ConvertToHex(winCtrlList[i].hCtrl) + winCtrlList[i].caption;
            i += 1;
            if (i==(winCtrlList.Count))
            {
                i = -1; 
            }
            return str;
        }

        private string ConvertToHex(int num)
        {
            byte[] mbytes = BitConverter.GetBytes(num);
            Array.Reverse(mbytes);
            return BitConverter.ToString(mbytes).Replace("-", null);
        }
 #endregion

    }
}