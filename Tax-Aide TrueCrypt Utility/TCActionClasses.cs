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
    }
    class TCWin //this class is all about the TrueCrypt winodws that are open when TC is running install uninstall or formatting
    {
        static List<NextWinAction> winaction;  //variable to hold which list of actions we are using
         static List<NextWinAction> winActionFormat = new List<NextWinAction>()
        {
            //First block are TV Volume Format items
            new NextWinAction{uniqueWinText="Volume Location",actionToBeDone= "EnterText",captionText="Next",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Create an encrypted file container",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Volume Type",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Volume Size",actionToBeDone="EnterText",captionText="Next",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Volume Format",actionToBeDone="ComboSelect",captionText="ComboBox0",variableText="NTFS,Format"},//Selection to be made in Combo Box and button to be pressed to continue
            new NextWinAction{uniqueWinText="Volume Password",actionToBeDone="Nothing",captionText="",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Encryption Options",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Short passwords are easy",actionToBeDone="ClickButton",captionText="Yes",variableText=String.Empty},
            new NextWinAction{uniqueWinText="been successfully created",actionToBeDone="SpecialKey",captionText="OK",variableText="\r"}, //sends enter key
            new NextWinAction{uniqueWinText="volume has been created and",actionToBeDone="SpecialKey",captionText="Exit",variableText="\u001B"}, //sends escape key
            new NextWinAction{uniqueWinText="administrator privileges",actionToBeDone="ClickButton",captionText="&No",variableText=String.Empty},
            new NextWinAction{uniqueWinText="NOT ENCRYPT THE FILE, BUT IT WILL DELETE IT",actionToBeDone="Nothing",captionText="",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Large Files",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty}
        };
            //End of TC Volume Format entries
            //Next Block is TC installer items
         static List<NextWinAction> winActionInstall = new List<NextWinAction>()
         {
            new NextWinAction{uniqueWinText="accept these license terms",actionToBeDone="ClickButton",captionText="I a&ccept and agree to be bound by the license terms", variableText="a"},      //2 entries required in window c to 1st button then followed by a which moves this window c is implied a is the extra entry 
            new NextWinAction{uniqueWinText="Wizard Mode",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Installing",actionToBeDone="Nothing",captionText="",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Setup Options",actionToBeDone="ClickButton",captionText="Create System &Restore point", variableText="\r"},     //2 entries required in window click to 1st button then followed by variable text entry either letter for clickbutton or \r which moves this window 
            new NextWinAction{uniqueWinText="TrueCrypt Installed",actionToBeDone="SpecialKey",captionText="&Finish",variableText="\r"},
            new NextWinAction{uniqueWinText="successfully installed",actionToBeDone="SpecialKey",captionText="OK",variableText="\r"},
            new NextWinAction{uniqueWinText="successfully updated",actionToBeDone="SpecialKey",captionText="OK",variableText="\r"},
            new NextWinAction{uniqueWinText="never used TrueCrypt before",actionToBeDone="ClickButton",captionText="&No",variableText=String.Empty},
            new NextWinAction{uniqueWinText="computer must be restarted",actionToBeDone="RestartExit",captionText="",variableText=String.Empty},
            new NextWinAction{uniqueWinText="system cannot find the file specified",actionToBeDone="RestartExit",captionText="",variableText=String.Empty}         
         };
            //End of TC setup install block
            //Next Block is TC uninstall
         static List<NextWinAction> winActionUninstall = new List<NextWinAction>()
         {
            new NextWinAction{uniqueWinText="Click Uninstall to remove TrueCrypt from this system.++&Uninstall",actionToBeDone="ClickButton",captionText="Create System &Restore point", variableText="\r"},     //2 entries required in window click to 1st button then followed by variable text entry either letter for clickbutton or \r to move this window 
            new NextWinAction{uniqueWinText="Click Uninstall to remove TrueCrypt from this system.++&Finish",actionToBeDone="ClickButton",captionText="&Finish", variableText=String.Empty},     //
            new NextWinAction{uniqueWinText="TrueCrypt has been successfully uninstalled",actionToBeDone="SpecialKey",captionText="OK",variableText="\r"}, //sends enter key
            new NextWinAction{uniqueWinText="directory is not empty",actionToBeDone="SpecialKey",captionText="OK",variableText="\r"}, 
            new NextWinAction{uniqueWinText="Uninstallation failed",actionToBeDone="UninstallFail",captionText="OK",variableText="\r"}, 
            new NextWinAction{uniqueWinText="Exit?",actionToBeDone="ClickButton",captionText="&Yes",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Uninstall TrueCrypt from:",actionToBeDone="SpecialKey",captionText="&Uninstall", variableText="\r"}     //For TC 4.2 version only - unique uninstall window
        };
         //Next Block is TC installer extract files items
         static List<NextWinAction> winActionExtract = new List<NextWinAction>()
         {
            new NextWinAction{uniqueWinText="accept these license terms",actionToBeDone="ClickButton",captionText="I a&ccept and agree to be bound by the license terms", variableText="a"},      //2 entries required in window c to 1st button then followed by a which moves this window c is implied a is the extra entry 
            new NextWinAction{uniqueWinText="Wizard Mode",actionToBeDone="ClickButton",captionText="Extract",variableText="\r"},
            new NextWinAction{uniqueWinText="Note that if you decide",actionToBeDone="ClickButton",captionText="&Yes",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Extraction Options",actionToBeDone="EnterText",captionText="&Open the destination location when finished,E&xtract",variableText=String.Empty}, //variable text to be filled in before use NOTE have 2 buttins in this one logic is in entertext action       
            new NextWinAction{uniqueWinText="Extracting",actionToBeDone="Nothing",captionText="",variableText=String.Empty},
            new NextWinAction{uniqueWinText="Extraction Complete",actionToBeDone="SpecialKey",captionText="&Finish",variableText="\r"},
            new NextWinAction{uniqueWinText="All files have been successfully extracted",actionToBeDone="SpecialKey",captionText="OK",variableText="\r"},         
            new NextWinAction{uniqueWinText="system cannot find the file specified",actionToBeDone="RestartExit",captionText="",variableText=String.Empty}         
         };
         //End of TC setup installer extract block

         public static List<string> dlgBoxTitles = new List<string>()
         {
             "TrueCrypt Setup",
             "Truecrypt Volume Creation Wizard",
             "TrueCrypt Setup 6.3a",
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
        int hWndHandle;//handle for window associated with this object
        int hWndDialog; // to track case of dialog box
        public static bool uninstallFailContinue;   //a boolean to remember passing through a dialog box when back at main window recursion loop
        public delegate bool Callback(int hWnd, int lParam); //Used to enum child windows in initialisation and chekc if windows changed


   #region TC window Class object initializer to get all Window data Delegate and CallBack
        public TCWin(int hWnd) //initial constructor
        {
            hWndHandle = hWnd;
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

        public static void SetupDoActionList(string procedure, string path, string size)   //procedure we are doing to setup up winaction, path for locations and vol size for formating
        {
            int i;
            switch (procedure)
            {
                case "install":
                    winaction = winActionInstall;
                    break;
                case "uninstall":
                    winaction = winActionUninstall;
                    break;
                case "format":
                    winaction = winActionFormat;
                    i = winActionFormat.FindIndex(delegate(NextWinAction s) { return s.uniqueWinText.Equals("Volume Location"); });
                    winActionFormat[i].variableText = path;
                    i = winActionFormat.FindIndex(delegate(NextWinAction s) { return s.uniqueWinText.Equals("Volume Size"); });
                    winActionFormat[i].variableText = size;
                    break;
                case "extract":
                    winaction = winActionExtract;
                    i = winActionExtract.FindIndex(delegate(NextWinAction s) { return s.uniqueWinText.Equals("Extraction Options"); });
                    winActionExtract[i].variableText = path;
                    break;
            }
        }
                
        public int DoAction()
        {
            int r = winaction.FindIndex(delegate(NextWinAction s) { return winText.ToString().Contains(s.uniqueWinText); });
            if (r >= 0)
            {
                Log.WritWTime("  "+winaction[r].uniqueWinText + ", r="+r.ToString());
                //.Write.WriteLine("  "+winaction[r].uniqueWinText + ", r="+r.ToString());
                int wCtrLstIndex = 0;
                switch (winaction[r].actionToBeDone)
                {

   #region Action Case Selection
                    case "Nothing":
                        Win32.SetForegroundWindow(hWndHandle); //setup for user data entry
                        break;
                    case "ClickButton":
                        //find button then send first letter
                        NextButton(winaction[r].captionText, string.Empty);
                        if (winaction[r].variableText != string.Empty)
                        {   //then we have another character to send, Note this logic is for 1 additional chareacter to 1st button
                            if (winaction[r].variableText != "\r")
                            {
                                NextButton(winaction[r].captionText, winaction[r].variableText);
                            }
                            else //special processing for enter key
                            {
                                Win32.PostMessage(hWndHandle, (int)win32Message.WM_KEYDOWN, (IntPtr)'\r', (IntPtr)0x1C0001);
                            }
                        }
                        break;
                    case "SpecialKey":  //Assumes single character

                        int wCtrLstIdx = winCtrlList.FindIndex(delegate(WinCtrls w) { return (w.cntrlClass.Equals("Button") & w.caption.Contains(winaction[r].captionText)); });
                        int ch = Char.Parse(winaction[r].variableText);
                        char c1 = char.Parse(winaction[r].variableText);
                        int scanCode1 = VKeytoScanCodeCls.Lookup(c1);
                        int LParam1 = Win32.MakeLParam(0x0001, scanCode1);
                        Win32.SetFocus(winCtrlList[wCtrLstIdx].hCtrl);
                        Win32.PostMessage(winCtrlList[wCtrLstIdx].hCtrl, (int)win32Message.WM_KEYDOWN, (IntPtr)ch, (IntPtr)LParam1);
                        break;
                    case "EnterText":   //This logic assumes there is only one edit box in the Win and the NEXT button is the same as previous (true for TC)
                        // GetDlgCtrl handle of edit box enter text
                        wCtrLstIndex = winCtrlList.FindIndex(delegate(WinCtrls w) { return (w.cntrlClass.Equals("Edit")); });
                        Win32.SendMessage(winCtrlList[wCtrLstIndex].hCtrl, (int)win32Message.EM_SETSEL, (IntPtr)0, (IntPtr)(-1)); //selects all existing text
                        char[] values = winaction[r].variableText.ToCharArray();
                        foreach (char c in values)
                        {
                            int scanCode = VKeytoScanCodeCls.Lookup(c);
                            int LParam = Win32.MakeLParam(0x0001, scanCode);
                            int t = Win32.SendMessage(winCtrlList[wCtrLstIndex].hCtrl, (int)win32Message.WM_CHAR, (IntPtr)Convert.ToInt32(c), (IntPtr)LParam);
                        }   // Next section allows multiple buttons (including radio buttons) to be clicked
                        string[] buttonStrings = winaction[r].captionText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in buttonStrings)
                        {
                            NextButton(item, string.Empty);                            
                        }
                        break;
                    case "ComboSelect": //assumes unique win text is picked up by win contrl list 
                        wCtrLstIndex = winCtrlList.FindIndex(delegate(WinCtrls w) { return (w.cntrlClass.Equals("ComboBox") & w.caption.Contains(winaction[r].captionText)); });
                        StringBuilder txt = new StringBuilder(10);
                        int variableTextCommaIndex = winaction[r].variableText.IndexOf(',');
                        txt.Append(winaction[r].variableText.Substring(0, variableTextCommaIndex));
                        int ret = Win32.SendMessage(winCtrlList[wCtrLstIndex].hCtrl, (int)win32Message.CB_SELECTSTRING, (IntPtr)(-1), txt);
                        NextButton(winaction[r].variableText.Substring(variableTextCommaIndex + 1),string.Empty);//Assumes this is always the next button when combo box involved otherwise have to do something complex with data like sharing the variable string. Could do a return keydown message to wondow
                        break;
                    case "RestartExit": //TC installation
                        MessageBox.Show("TrueCrypt requires a System Restart\rPlease do that and then restart this program", "TrueCrypt Utilities",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                        Log.WritWTime("System restart required due to TrueCrypt installation or Problem");
                        Application.Exit();
                        break;
                    case "UninstallFail": //
                        if (File.Exists(TrueCryptSWObj.tcProgramFQN))
                        {   // uninstall failed
                            Log.WritWTime("Automated TrueCrypt Uninstall failed");
                            MessageBox.Show("Automated TrueCrypt Uninstall failed.\nPlease uninstall TrueCrypt manually then restart this program", "TrueCrypt Utilities", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            Application.Exit();
                        }
                        else
                        {   //a fail due to non empty directory so can do install
                            Win32.PostMessage(hWndHandle, (int)win32Message.WM_KEYDOWN, (IntPtr)'\r', (IntPtr)0x1C0001); //sends enter to dialog box
                            Thread.Sleep(1000);
                            int hWnd = Win32.FindWindow("CustomDlg", mainWinTitle); //now send escape to mainW window
                            Win32.PostMessage(hWnd, (int)win32Message.WM_KEYDOWN, (IntPtr)'\u001B', (IntPtr)0x10001); //sends enter to dialog box
                            uninstallFailContinue = true;
                        }
                        break;
                    #endregion
                }

   #region Wait for next window region
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
                        i=dlgWin.DoAction(); //does action waits for change if change is another dlgbox recursively comes here
                        if (i != 1)
                        {
                            return 0; //break out of loop this indicates that we have a chnage and it is not a dialog box
                        }
                        hWndDialog = 0;
                       return 1;
                    }
                }

                #endregion
            }
            else //r=0 ie this wondow not in the list
            {
                Log.WriteStrm.WriteLine("NEW WINDOW FOUND Static Text=" + staticText.ToString());
                foreach (WinCtrls item in winCtrlList)
                {
                    Log.WriteStrm.WriteLine(item.cntrlClass + ", " + item.caption);
                }
                MessageBox.Show(string.Format("The " + mainWinTitle + " Process has displayed an unexpected window.\n Exiting, a log file is located at {0}", Log.logPathFile), "TrueCrypt Utilities", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 0;//new window
            }
            //Come out of action and wait loops here so test for exit conditions
            if (Win32.FindWindow("CustomDlg", mainWinTitle) == 0)   // normal exit when TrueCrypt process has disappeared
            {
                if (winaction[r].captionText == "Exit" | winaction[r].captionText == "&Finish" )
                {   //covers format (exit) and TC setup (&Finish is clicked)
                    Log.WritWTime("TrueCrypt Normal Exit");
                    return 0; //ends the loop
                }
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
                MessageBox.Show(string.Format("The " + mainWinTitle + " Process has unexpectedly disappeared.\n A log file is located at {0}", Log.logPathFile), "TrueCrypt Utilities", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 0;
            }
            return 1;
        }
        void NextButton(string caption,string sendLetter)
        {
            int wCtrLstIndex = 0;
            int ch = 0;
            wCtrLstIndex = winCtrlList.FindIndex(delegate(WinCtrls w) { return (w.cntrlClass.Equals("Button") & w.caption.Contains(caption)); });
            if (sendLetter == string.Empty)
            {
                int posAmp = winCtrlList[wCtrLstIndex].caption.IndexOf('&') + 1; //Ampersand marks letter used to operate button
                ch = Char.Parse(winCtrlList[wCtrLstIndex].caption.Substring(posAmp, 1));//  Convert character after & in the string to int32                
            }
            else
            {
                ch = Char.Parse(sendLetter);
            }
            int scanCode = VKeytoScanCodeCls.Lookup(Char.Parse(winCtrlList[wCtrLstIndex].caption.Substring(1, 1)));
            int LParam = Win32.MakeLParam(0x0001, scanCode);
            Win32.PostMessage(winCtrlList[wCtrLstIndex].hCtrl, (int)win32Message.WM_CHAR, (IntPtr)ch, (IntPtr)LParam);
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