using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;


namespace TaxAide_TrueCrypt_Utility
{
    public abstract class TcAction
    {
        protected int r;// index into table of windows and actions
        protected abstract List<NextWinAction> winAction { get; } //minimum required to define a property even though do not ever use get to pull complete list
        public delegate void Action();
        TCWin tcWin;
        internal int nFlag = 0; //diagnostic for arg out of range on xp
        #region DoAction and consequent Action Functions
        public void DoAction(TCWin tcWinParam)
        {
            tcWin = tcWinParam;
            //Log.WritWTime(tcWin.winText.ToString());
            r = winAction.FindIndex(delegate(NextWinAction s) { return tcWin.winText.ToString().Contains(s.uniqueWinText); });
            if (r >= 0)
            {
                Log.WritWTime("  " + winAction[r].uniqueWinText + ", r=" + r.ToString());
                Action A = winAction[r].activeFunction;
                A();
            }
            else
            {
                Log.WriteStrm.WriteLine("NEW WINDOW FOUND Static Text=" + tcWin.staticText.ToString());
                foreach (TCWin.WinCtrls item in tcWin.winCtrlList)
                {
                    Log.WriteStrm.WriteLine(item.cntrlClass + ", " + item.caption);
                }
                MessageBox.Show(string.Format("The " + TCWin.mainWinTitle + " Process has displayed an unexpected window.\n Exiting, a log file is located at {0}", Log.logPathFile), DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);//new window

            }
        }
        protected void Nothing()    //setup for user data entry
        {
            Win32.SetForegroundWindow(tcWin.hWndHandle);
        }
        protected void ClickButton()
        {
            //find button then send first letter
            NextButton(winAction[r].captionText, string.Empty);
            if (winAction[r].variableText != string.Empty)
            {   //then we have another character to send, Note this logic is for 1 additional chareacter to 1st button
                if (winAction[r].variableText != "\r")
                {
                    NextButton(winAction[r].captionText, winAction[r].variableText);
                }
                else //special processing for enter key
                {
                    Win32.PostMessage(tcWin.hWndHandle, (int)win32Message.WM_KEYDOWN, (IntPtr)'\r', (IntPtr)0x1C0001);
                }
            }
            return;
        }
        protected void SpecialKey() //Assumes single character
        {
            int wCtrLstIdx = tcWin.winCtrlList.FindIndex(delegate(TCWin.WinCtrls w) { return (w.cntrlClass.Equals("Button") & w.caption.Contains(winAction[r].captionText)); });
            int ch = Char.Parse(winAction[r].variableText);
            char c1 = char.Parse(winAction[r].variableText);
            int scanCode1 = VKeytoScanCodeCls.Lookup(c1);
            int LParam1 = Win32.MakeLParam(0x0001, scanCode1);
            Win32.SetFocus(tcWin.winCtrlList[wCtrLstIdx].hCtrl);
            Win32.PostMessage(tcWin.winCtrlList[wCtrLstIdx].hCtrl, (int)win32Message.WM_KEYDOWN, (IntPtr)ch, (IntPtr)LParam1);
        }

        protected void EnterText()  //This logic assumes there is only one edit box in the Win and the NEXT button is the same as previous (true for TC)
        // GetDlgCtrl handle of edit box enter text Exception is Password which has 2 edit boxes separated by tab processing
        {
            int wCtrLstIndex = tcWin.winCtrlList.FindIndex(delegate(TCWin.WinCtrls w) { return (w.cntrlClass.Equals("Edit")); });
            Win32.SendMessage(tcWin.winCtrlList[wCtrLstIndex].hCtrl, (int)win32Message.EM_SETSEL, (IntPtr)0, (IntPtr)(-1)); //selects all existing text
            char[] values = winAction[r].variableText.ToCharArray();
            foreach (char c in values)
            {
                if (c != '\t')
                {
                    int scanCode = VKeytoScanCodeCls.Lookup(c);
                    int LParam = Win32.MakeLParam(0x0001, scanCode);
                    int t = Win32.PostMessage(tcWin.winCtrlList[wCtrLstIndex].hCtrl, (int)win32Message.WM_CHAR, (IntPtr)Convert.ToInt32(c), (IntPtr)LParam);
                }
                else
                    wCtrLstIndex += 1; // this is password setup with confirm box immediately after password entry box in window control list
            }   
            // Next section allows multiple buttons (including radio buttons) to be clicked
            string[] buttonStrings = winAction[r].captionText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in buttonStrings)
            {// special processing to convert Next button to enter key to whole window caused by vol loc screen change in TC7.0 (never save history checkbox went from an s to an n same as next
                if (string.Compare(item,"Next",true) == 0)
                {
                    Win32.PostMessage(tcWin.hWndHandle, (int)win32Message.WM_KEYDOWN, (IntPtr)'\r', (IntPtr)0x1C0001);
                }
                else
                {
                    NextButton(item, string.Empty); 
                }
            }
        }
        protected void ComboSelect()    //assumes unique win text is picked up by win contrl list
        {
            int wCtrLstIndex = tcWin.winCtrlList.FindIndex(delegate(TCWin.WinCtrls w) { return (w.cntrlClass.Equals("ComboBox") & w.caption.Contains(winAction[r].captionText)); });
            StringBuilder txt = new StringBuilder(20);
            int variableTextCommaIndex = winAction[r].variableText.IndexOf(',');
            txt.Append(winAction[r].variableText.Substring(0, variableTextCommaIndex));
            nFlag = 1; //diagnostic for arg out of range on xp
            int ret = Win32.SendMessage(tcWin.winCtrlList[wCtrLstIndex].hCtrl, (uint)win32Message.CB_SELECTSTRING, (IntPtr)(-1), txt);
            NextButton(winAction[r].variableText.Substring(variableTextCommaIndex + 1), string.Empty);//Assumes this is always the next button when combo box involved otherwise have to do something complex with data like sharing the variable string. Could do a return keydown message to window
            nFlag = 0; //diagnostic for arg out of range on xp
        }
        protected void RestartExit()    //TC installation
        {
            MessageBox.Show("TrueCrypt requires a System Restart\rPlease do that and then restart this program", DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            Log.WritWTime("System restart required due to TrueCrypt installation or Problem");
            Environment.Exit(1);
        }
        protected void UninstallFail()
        {
            if (System.IO.File.Exists(DoTasksObj.tcProgramFQN))
            {   // uninstall failed
                Log.WritWTime("Automated TrueCrypt Uninstall failed");
                MessageBox.Show("Automated TrueCrypt Uninstall failed.\nPlease uninstall TrueCrypt manually then restart this program", DoTasksObj.mbCaption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
            }
            else
            {   //a fail due to non empty directory so can do install
                Win32.PostMessage(tcWin.hWndHandle, (int)win32Message.WM_KEYDOWN, (IntPtr)'\r', (IntPtr)0x1C0001); //sends enter to dialog box
                Thread.Sleep(1000);
                IntPtr hWnd = Win32.FindWindow("CustomDlg", TCWin.mainWinTitle); //now send escape to mainW window
                Win32.PostMessage(hWnd, (int)win32Message.WM_KEYDOWN, (IntPtr)'\u001B', (IntPtr)0x10001); //sends enter to dialog box
                TCWin.uninstallFailContinue = true;
            }
        }
        public int TestExit()
        {
            if (winAction[r].captionText == "Exit" | winAction[r].captionText == "&Finish")
            {   //covers format (exit) and TC setup (&Finish is clicked)
                return 0; //ends the loop
            }
            else return 1;
        }
        void NextButton(string caption, string sendLetter)
        {
            int wCtrLstIndex = 0;
            int ch = 0;
            wCtrLstIndex = tcWin.winCtrlList.FindIndex(delegate(TCWin.WinCtrls w) { return (w.cntrlClass.Equals("Button") & w.caption.Contains(caption)); });
            if (nFlag == 1)  //diagnostic for arg out of range on xp
            {
                Log.WritWTime("NextButton = " + wCtrLstIndex.ToString());  //diagnostic for arg out of range on xp
            }
            if (sendLetter == string.Empty)
            {
                if (nFlag == 1)  //diagnostic for arg out of range on xp
                {
                    Log.WritWTime("caption = " + tcWin.winCtrlList[wCtrLstIndex].caption); //Ampersand marks letter used to operate button
                }
                int posAmp = tcWin.winCtrlList[wCtrLstIndex].caption.IndexOf('&') + 1; //Ampersand marks letter used to operate button
                if (nFlag == 1)  //diagnostic for arg out of range on xp
                {
                    Log.WritWTime("poAmp = " + posAmp.ToString()); //Ampersand marks letter used to operate button
                }
                ch = Char.Parse(tcWin.winCtrlList[wCtrLstIndex].caption.Substring(posAmp, 1));//  Convert character after & in the string to int32                
            }
            else
            {
                ch = Char.Parse(sendLetter);
            }
            int scanCode = VKeytoScanCodeCls.Lookup(Char.Parse(tcWin.winCtrlList[wCtrLstIndex].caption.Substring(1, 1)));
            int LParam = Win32.MakeLParam(0x0001, scanCode);
            Win32.PostMessage(tcWin.winCtrlList[wCtrLstIndex].hCtrl, (int)win32Message.WM_CHAR, (IntPtr)ch, (IntPtr)LParam);
            //Log.WritWTime("hctrl = " + tcWin.hWndHandle.ToString() + tcWin.winCtrlList[wCtrLstIndex].cntrlClass.ToString() + " " + tcWin.winCtrlList[wCtrLstIndex].caption + " char ch = " + ch);
        }
        #endregion
    }
    public class TcActionInstall : TcAction
    {
        private List<NextWinAction> winActionP = new List<NextWinAction>();
        protected override List<NextWinAction> winAction { get { return winActionP; } }
        public TcActionInstall()
        {
            winActionP.AddRange(new NextWinAction[] // have to do this as an initailization cannot do it when defining with abstract/overirde situation
            {
            new NextWinAction{uniqueWinText="accept these license terms",actionToBeDone="ClickButton",captionText="I a&ccept and agree to be bound by the license terms", variableText="a",activeFunction=ClickButton},      //2 entries required in window c to 1st button then followed by a which moves this window c is implied a is the extra entry 
            new NextWinAction{uniqueWinText="Wizard Mode",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="Installing",actionToBeDone="Nothing",captionText="",variableText=String.Empty,activeFunction=Nothing},
            new NextWinAction{uniqueWinText="Setup Options",actionToBeDone="ClickButton",captionText="Create System &Restore point", variableText="\r",activeFunction=ClickButton},     //2 entries required in window click to 1st button then followed by variable text entry either letter for clickbutton or \r which moves this window 
            new NextWinAction{uniqueWinText="TrueCrypt Installed",actionToBeDone="SpecialKey",captionText="&Finish",variableText="\r",activeFunction=SpecialKey},
            new NextWinAction{uniqueWinText="successfully installed",actionToBeDone="SpecialKey",captionText="OK",variableText="\r",activeFunction=SpecialKey},
            new NextWinAction{uniqueWinText="successfully updated",actionToBeDone="SpecialKey",captionText="OK",variableText="\r",activeFunction=SpecialKey},
            new NextWinAction{uniqueWinText="never used TrueCrypt before",actionToBeDone="ClickButton",captionText="&No",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="computer must be restarted",actionToBeDone="RestartExit",captionText="",variableText=String.Empty,activeFunction=RestartExit},
            new NextWinAction{uniqueWinText="service has been marked for deletion" ,actionToBeDone="RestartExit" ,captionText="",variableText=String.Empty,activeFunction=RestartExit},
            new NextWinAction{uniqueWinText="system cannot find the file specified",actionToBeDone="RestartExit",captionText="",variableText=String.Empty,activeFunction=RestartExit}, 
            new NextWinAction{uniqueWinText="device driver has failed",actionToBeDone="RestartExit",captionText="",variableText=String.Empty,activeFunction=RestartExit}         
            });
        }
    }
    public class TcActionExtract : TcAction
    {
        private List<NextWinAction> winActionP = new List<NextWinAction>();
        protected override List<NextWinAction> winAction { get { return winActionP; } }
        public TcActionExtract()
        {
            winActionP.AddRange(new NextWinAction[] // have to do this as an initailization cannot do it when defining with abstract/overirde situation
            {
            new NextWinAction{uniqueWinText="accept these license terms",actionToBeDone="ClickButton",captionText="I a&ccept and agree to be bound by the license terms", variableText="a", activeFunction =ClickButton},      //2 entries required in window c to 1st button then followed by a which moves this window c is implied a is the extra entry
            new NextWinAction{uniqueWinText="Wizard Mode",actionToBeDone="ClickButton",captionText="Extract",variableText="\r",activeFunction =ClickButton},
            new NextWinAction{uniqueWinText="Note that if you decide",actionToBeDone="ClickButton",captionText="&Yes",variableText=String.Empty,activeFunction = ClickButton},
            new NextWinAction{uniqueWinText="Extraction Options",actionToBeDone="EnterText",captionText="&Open the destination location when finished,E&xtract",variableText=String.Empty, activeFunction =EnterText}, //variable text to be filled in before use NOTE have 2 buttins in this one logic is in entertext action       
            new NextWinAction{uniqueWinText="Extracting",actionToBeDone="Nothing",captionText="",variableText=String.Empty, activeFunction =Nothing},
            new NextWinAction{uniqueWinText="Extraction Complete",actionToBeDone="SpecialKey",captionText="&Finish",variableText="\r", activeFunction =SpecialKey},
            new NextWinAction{uniqueWinText="All files have been successfully extracted",actionToBeDone="SpecialKey",captionText="OK",variableText="\r", activeFunction =SpecialKey}         
            });
            Log.WritWTime("Extract List setup = " + winActionP[1].captionText);
        }
        public void SetPath(string path)
        {
            int i = winActionP.FindIndex(delegate(NextWinAction s) { return s.uniqueWinText.Equals("Extraction Options"); });
            winActionP[i].variableText = path;
        }
    }
    public class TcActionUninstall : TcAction
    {
        private List<NextWinAction> winActionP = new List<NextWinAction>();
        protected override List<NextWinAction> winAction { get { return winActionP; } }
        public TcActionUninstall()
        {
            winActionP.AddRange(new NextWinAction[] // have to do this as an initailization cannot do it when defining with abstract/overirde situation
            {
            new NextWinAction{uniqueWinText="Click Uninstall to remove TrueCrypt from this system.++&Uninstall",actionToBeDone="ClickButton",captionText="Create System &Restore point", variableText="\r",activeFunction=ClickButton},     //2 entries required in window click to 1st button then followed by variable text entry either letter for clickbutton or \r to move this window 
            new NextWinAction{uniqueWinText="Click Uninstall to remove TrueCrypt from this system.++&Finish",actionToBeDone="ClickButton",captionText="&Finish", variableText=String.Empty,activeFunction=ClickButton},     //
            new NextWinAction{uniqueWinText="TrueCrypt has been successfully uninstalled",actionToBeDone="SpecialKey",captionText="OK",variableText="\r",activeFunction=SpecialKey}, //sends enter key
            new NextWinAction{uniqueWinText="directory is not empty",actionToBeDone="SpecialKey",captionText="OK",variableText="\r",activeFunction=SpecialKey}, 
            new NextWinAction{uniqueWinText="Uninstallation failed",actionToBeDone="UninstallFail",captionText="OK",variableText="\r",activeFunction=UninstallFail}, 
            new NextWinAction{uniqueWinText="Exit?",actionToBeDone="ClickButton",captionText="&Yes",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="Uninstall TrueCrypt from:",actionToBeDone="SpecialKey",captionText="&Uninstall", variableText="\r",activeFunction=SpecialKey}     //For TC 4.2 version only - unique uninstall window
            });
        }
    }
    public class TcActionFormat : TcAction
    {
        private List<NextWinAction> winActionP = new List<NextWinAction>();
        protected override List<NextWinAction> winAction { get { return winActionP; } }
        public TcActionFormat()
        {
            winActionP.AddRange(new NextWinAction[] // have to do this as an initailization cannot do it when defining with abstract/overirde situation
            {
            new NextWinAction{uniqueWinText="Volume Location",actionToBeDone= "EnterText",captionText="Next",variableText=String.Empty,activeFunction=EnterText},
            new NextWinAction{uniqueWinText="Create an encrypted file container",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="Volume Type",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="Volume Size",actionToBeDone="EnterText",captionText="Next",variableText=String.Empty,activeFunction=EnterText},
            new NextWinAction{uniqueWinText="Volume Format",actionToBeDone="ComboSelect",captionText="ComboBox0",variableText="NTFS,Format",activeFunction=ComboSelect},//Selection to be made in Combo Box and button to be pressed to continue
            new NextWinAction{uniqueWinText="Volume Password",actionToBeDone="EnterText",captionText="Next",variableText=String.Empty,activeFunction=EnterText},
            new NextWinAction{uniqueWinText="Encryption Options",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="Short passwords are easy",actionToBeDone="ClickButton",captionText="Yes",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="been successfully created",actionToBeDone="SpecialKey",captionText="OK",variableText="\r",activeFunction=SpecialKey}, //sends enter key
            new NextWinAction{uniqueWinText="Caps Lock is on",actionToBeDone="SpecialKey",captionText="OK",variableText="\r",activeFunction=SpecialKey}, //sends enter key
            new NextWinAction{uniqueWinText="volume has been created and",actionToBeDone="SpecialKey",captionText="Exit",variableText="\u001B",activeFunction=SpecialKey}, //sends escape key
            new NextWinAction{uniqueWinText="administrator privileges",actionToBeDone="ClickButton",captionText="&No",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="NOT ENCRYPT THE FILE, BUT IT WILL DELETE IT",actionToBeDone="Nothing",captionText="",variableText=String.Empty,activeFunction=Nothing},
            new NextWinAction{uniqueWinText="Force dismount?",actionToBeDone="ClickButton",captionText="Yes",variableText=String.Empty,activeFunction=ClickButton},
            new NextWinAction{uniqueWinText="Large Files",actionToBeDone="ClickButton",captionText="Next",variableText=String.Empty,activeFunction=ClickButton}
            });
        }
        public void SetEditBoxes(string path, string size, string password)
        {
            int i = winActionP.FindIndex(delegate(NextWinAction s) { return s.uniqueWinText.Equals("Volume Location"); });
            winActionP[i].variableText = path;
            i = winActionP.FindIndex(delegate(NextWinAction s) { return s.uniqueWinText.Equals("Volume Size"); });
            winActionP[i].variableText = size;
            i = winActionP.FindIndex(delegate(NextWinAction s) { return s.uniqueWinText.Equals("Volume Password"); });
            winActionP[i].variableText = password;
        }
    }
}
