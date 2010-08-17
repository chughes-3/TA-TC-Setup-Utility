using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TaxAide_TrueCrypt_Utility
{
    #region WM Message Constants
    enum win32Message : int    //goto http://msdn.microsoft.com/en-us/library/ms649011(VS.85).aspx and search for meaning
    {                           // locally in winuser.h in windows 7 sdk include directory
        WM_NULL =   0x00,
        WM_CREATE = 0x01,
        WM_DESTROY = 0x02,
        WM_MOVE =     0x03,
        WM_SIZE = 0x05,
        WM_ACTIVATE = 0x06,
        WM_SETFOCUS = 0x07,
        WM_KILLFOCUS = 0x08,
        WM_ENABLE = 0x0A,
        WM_SETREDRAW = 0x0B,
        WM_SETTEXT = 0x0C,
        WM_GETTEXT = 0x0D,
        WM_GETTEXTLENGTH = 0x0E,
        WM_PAINT = 0x0F,
        WM_CLOSE = 0x10,
        WM_QUERYENDSESSION = 0x11,
        WM_QUIT = 0x12,
        WM_QUERYOPEN = 0x13,
        WM_ERASEBKGND = 0x14,
        WM_SYSCOLORCHANGE = 0x15,
        WM_ENDSESSION = 0x16,
        WM_SYSTEMERROR = 0x17,
        WM_SHOWWINDOW = 0x18,
        WM_CTLCOLOR = 0x19,
        WM_WININICHANGE = 0x1A,
        WM_SETTINGCHANGE = 0x1A,
        WM_DEVMODECHANGE = 0x1B,
        WM_ACTIVATEAPP = 0x1C,
        WM_FONTCHANGE = 0x1D,
        WM_TIMECHANGE = 0x1E,
        WM_CANCELMODE = 0x1F,
        WM_SETCURSOR = 0x20,
        WM_MOUSEACTIVATE = 0x21,
        WM_CHILDACTIVATE = 0x22,
        WM_QUEUESYNC = 0x23,
        WM_GETMINMAXINFO = 0x24,
        WM_PAINTICON = 0x26,
        WM_ICONERASEBKGND = 0x27,
        WM_NEXTDLGCTL = 0x28,
        WM_SPOOLERSTATUS = 0x2A,
        WM_DRAWITEM = 0x2B,
        WM_MEASUREITEM = 0x2C,
        WM_DELETEITEM = 0x2D,
        WM_VKEYTOITEM = 0x2E,
        WM_CHARTOITEM = 0x2F,

        WM_SETFONT = 0x30,
        WM_GETFONT = 0x31,
        WM_SETHOTKEY = 0x32,
        WM_GETHOTKEY = 0x33,
        WM_QUERYDRAGICON = 0x37,
        WM_COMPAREITEM = 0x39,
        WM_COMPACTING = 0x41,
        WM_WINDOWPOSCHANGING = 0x46,
        WM_WINDOWPOSCHANGED = 0x47,
        WM_POWER = 0x48,
        WM_COPYDATA = 0x4A,
        WM_CANCELJOURNAL = 0x4B,
        WM_NOTIFY = 0x4E,
        WM_INPUTLANGCHANGEREQUEST = 0x50,
        WM_INPUTLANGCHANGE = 0x51,
        WM_TCARD = 0x52,
        WM_HELP = 0x53,
        WM_USERCHANGED = 0x54,
        WM_NOTIFYFORMAT = 0x55,
        WM_CONTEXTMENU = 0x7B,
        WM_STYLECHANGING = 0x7C,
        WM_STYLECHANGED = 0x7D,
        WM_DISPLAYCHANGE = 0x7E,
        WM_GETICON = 0x7F,
        WM_SETICON = 0x80,

        WM_NCCREATE = 0x81,
        WM_NCDESTROY = 0x82,
        WM_NCCALCSIZE = 0x83,
        WM_NCHITTEST = 0x84,
        WM_NCPAINT = 0x85,
        WM_NCACTIVATE = 0x86,
        WM_GETDLGCODE = 0x87,
        WM_NCMOUSEMOVE = 0xA0,
        WM_NCLBUTTONDOWN = 0xA1,
        WM_NCLBUTTONUP = 0xA2,
        WM_NCLBUTTONDBLCLK = 0xA3,
        WM_NCRBUTTONDOWN = 0xA4,
        WM_NCRBUTTONUP = 0xA5,
        WM_NCRBUTTONDBLCLK = 0xA6,
        WM_NCMBUTTONDOWN = 0xA7,
        WM_NCMBUTTONUP = 0xA8,
        WM_NCMBUTTONDBLCLK = 0xA9,

        WM_KEYFIRST = 0x100,
        WM_KEYDOWN = 0x100,
        WM_KEYUP = 0x101,
        WM_CHAR = 0x102,
        WM_DEADCHAR = 0x103,
        WM_SYSKEYDOWN = 0x104,
        WM_SYSKEYUP = 0x105,
        WM_SYSCHAR = 0x106,
        WM_SYSDEADCHAR = 0x107,
        WM_KEYLAST = 0x108,

        WM_IME_STARTCOMPOSITION = 0x10D,
        WM_IME_ENDCOMPOSITION = 0x10E,
        WM_IME_COMPOSITION = 0x10F,
        WM_IME_KEYLAST = 0x10F,

        WM_INITDIALOG = 0x110,
        WM_COMMAND = 0x111,
        WM_SYSCOMMAND = 0x112,
        WM_TIMER = 0x113,
        WM_HSCROLL = 0x114,
        WM_VSCROLL = 0x115,
        WM_INITMENU = 0x116,
        WM_INITMENUPOPUP = 0x117,
        WM_MENUSELECT = 0x11F,
        WM_MENUCHAR = 0x120,
        WM_ENTERIDLE = 0x121,

        WM_CTLCOLORMSGBOX = 0x132,
        WM_CTLCOLOREDIT = 0x133,
        WM_CTLCOLORLISTBOX = 0x134,
        WM_CTLCOLORBTN = 0x135,
        WM_CTLCOLORDLG = 0x136,
        WM_CTLCOLORSCROLLBAR = 0x137,
        WM_CTLCOLORSTATIC = 0x138,

        WM_MOUSEFIRST = 0x200,
        WM_MOUSEMOVE = 0x200,
        WM_LBUTTONDOWN = 0x201,
        WM_LBUTTONUP = 0x202,
        WM_LBUTTONDBLCLK = 0x203,
        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_RBUTTONDBLCLK = 0x206,
        WM_MBUTTONDOWN = 0x207,
        WM_MBUTTONUP = 0x208,
        WM_MBUTTONDBLCLK = 0x209,
        WM_MOUSELAST = 0x20A,
        WM_MOUSEWHEEL = 0x20A,

        WM_PARENTNOTIFY = 0x210,
        WM_ENTERMENULOOP = 0x211,
        WM_EXITMENULOOP = 0x212,
        WM_NEXTMENU = 0x213,
        WM_SIZING = 0x214,
        WM_CAPTURECHANGED = 0x215,
        WM_MOVING = 0x216,
        WM_POWERBROADCAST = 0x218,
        WM_DEVICECHANGE = 0x219,

        WM_MDICREATE = 0x220,
        WM_MDIDESTROY = 0x221,
        WM_MDIACTIVATE = 0x222,
        WM_MDIRESTORE = 0x223,
        WM_MDINEXT = 0x224,
        WM_MDIMAXIMIZE = 0x225,
        WM_MDITILE = 0x226,
        WM_MDICASCADE = 0x227,
        WM_MDIICONARRANGE = 0x228,
        WM_MDIGETACTIVE = 0x229,
        WM_MDISETMENU = 0x230,
        WM_ENTERSIZEMOVE = 0x231,
        WM_EXITSIZEMOVE = 0x232,
        WM_DROPFILES = 0x233,
        WM_MDIREFRESHMENU = 0x234,

        WM_IME_SETCONTEXT = 0x281,
        WM_IME_NOTIFY = 0x282,
        WM_IME_CONTROL = 0x283,
        WM_IME_COMPOSITIONFULL = 0x284,
        WM_IME_SELECT = 0x285,
        WM_IME_CHAR = 0x286,
        WM_IME_KEYDOWN = 0x290,
        WM_IME_KEYUP = 0x291,

        WM_MOUSEHOVER = 0x2A1,
        WM_NCMOUSELEAVE = 0x2A2,
        WM_MOUSELEAVE = 0x2A3,

        WM_CUT = 0x300,
        WM_COPY = 0x301,
        WM_PASTE = 0x302,
        WM_CLEAR = 0x303,
        WM_UNDO = 0x304,

        WM_RENDERFORMAT = 0x305,
        WM_RENDERALLFORMATS = 0x306,
        WM_DESTROYCLIPBOARD = 0x307,
        WM_DRAWCLIPBOARD = 0x308,
        WM_PAINTCLIPBOARD = 0x309,
        WM_VSCROLLCLIPBOARD = 0x30A,
        WM_SIZECLIPBOARD = 0x30B,
        WM_ASKCBFORMATNAME = 0x30C,
        WM_CHANGECBCHAIN = 0x30D,
        WM_HSCROLLCLIPBOARD = 0x30E,
        WM_QUERYNEWPALETTE = 0x30F,
        WM_PALETTEISCHANGING = 0x310,
        WM_PALETTECHANGED = 0x311,

        WM_HOTKEY = 0x312,
        WM_PRINT = 0x317,
        WM_PRINTCLIENT = 0x318,

        WM_HANDHELDFIRST = 0x358,
        WM_HANDHELDLAST = 0x35F,
        WM_PENWINFIRST = 0x380,
        WM_PENWINLAST = 0x38F,
        WM_COALESCE_FIRST = 0x390,
        WM_COALESCE_LAST = 0x39F,
        WM_DDE_FIRST = 0x3E0,
        WM_DDE_INITIATE = 0x3E0,
        WM_DDE_TERMINATE = 0x3E1,
        WM_DDE_ADVISE = 0x3E2,
        WM_DDE_UNADVISE = 0x3E3,
        WM_DDE_ACK = 0x3E4,
        WM_DDE_DATA = 0x3E5,
        WM_DDE_REQUEST = 0x3E6,
        WM_DDE_POKE = 0x3E7,
        WM_DDE_EXECUTE = 0x3E8,
        WM_DDE_LAST = 0x3E8,

        WM_USER = 0x400,
        WM_APP = 0x8000,
        //Combo Box messages
        CB_GETEDITSEL = 0x0140,//Gets the starting and ending character positions of the current selection in the edit control of a combo box
        CB_LIMITTEXT = 0x0141,
        CB_SETEDITSEL = 0x0142,
        CB_ADDSTRING = 0x0143,
        CB_DELETESTRING = 0x0144,
        CB_DIR = 0x0145,
        CB_GETCOUNT = 0x0146,
        CB_GETCURSEL = 0x0147,
        CB_GETLBTEXT = 0x0148,
        CB_GETLBTEXTLEN = 0x0149,
        CB_INSERTSTRING = 0x014A,
        CB_RESETCONTENT = 0x014B,
        CB_FINDSTRING = 0x014C,
        CB_SELECTSTRING = 0x014D,
        CB_SETCURSEL = 0x014E,
        CB_SHOWDROPDOWN = 0x014F,
        CB_GETITEMDATA = 0x0150,
        CB_SETITEMDATA = 0x0151,
        CB_GETDROPPEDCONTROLRECT = 0x0152,
        CB_SETITEMHEIGHT = 0x0153,
        CB_GETITEMHEIGHT = 0x0154,
        CB_SETEXTENDEDUI = 0x0155,
        CB_GETEXTENDEDUI = 0x0156,
        CB_GETDROPPEDSTATE = 0x0157,
        CB_FINDSTRINGEXACT = 0x0158,
        CB_SETLOCALE = 0x0159,
        CB_GETLOCALE = 0x015A,
        //Edit control messages
        EM_GETSEL = 0x00B0,
        EM_SETSEL = 0x00B1,
        EM_GETRECT = 0x00B2,
        EM_SETRECT = 0x00B3,
        EM_SETRECTNP = 0x00B4,
        EM_SCROLL = 0x00B5,
        EM_LINESCROLL = 0x00B6,
        EM_SCROLLCARET = 0x00B7,
        EM_GETMODIFY = 0x00B8,
        EM_SETMODIFY = 0x00B9,
        EM_GETLINECOUNT = 0x00BA,
        EM_LINEINDEX = 0x00BB,
        EM_SETHANDLE = 0x00BC,
        EM_GETHANDLE = 0x00BD,
        EM_GETTHUMB = 0x00BE,
        EM_LINELENGTH = 0x00C1,
        EM_REPLACESEL = 0x00C2,
        EM_GETLINE = 0x00C4,
        EM_LIMITTEXT = 0x00C5,
        EM_CANUNDO = 0x00C6,
        EM_UNDO = 0x00C7,
        EM_FMTLINES = 0x00C8,
        EM_LINEFROMCHAR = 0x00C9,
        EM_SETTABSTOPS = 0x00CB,
        EM_SETPASSWORDCHAR = 0x00CC,
        EM_EMPTYUNDOBUFFER = 0x00CD,
        EM_GETFIRSTVISIBLELINE = 0x00CE,
        EM_SETREADONLY = 0x00CF,
        EM_SETWORDBREAKPROC = 0x00D0,
        EM_GETWORDBREAKPROC = 0x00D1,
        EM_GETPASSWORDCHAR = 0x00D2

}
    #endregion

    #region Virtual keys enum
    /// <summary> 
    /// Virtual Keys 
    /// </summary> 
    enum VKeys : int
    {
        VK_LBUTTON = 0x01,   //Left mouse button 
        VK_RBUTTON = 0x02,   //Right mouse button 
        VK_CANCEL = 0x03,   //Control-break processing 
        VK_MBUTTON = 0x04,   //Middle mouse button (three-button mouse) 
        VK_BACK = 0x08,   //BACKSPACE key 
        VK_TAB = 0x09,   //TAB key 
        VK_CLEAR = 0x0C,   //CLEAR key 
        VK_RETURN = 0x0D,   //ENTER key 
        VK_SHIFT = 0x10,   //SHIFT key 
        VK_CONTROL = 0x11,   //CTRL key 
        VK_MENU = 0x12,   //ALT key 
        VK_PAUSE = 0x13,   //PAUSE key 
        VK_CAPITAL = 0x14,   //CAPS LOCK key 
        VK_ESCAPE = 0x1B,   //ESC key 
        VK_SPACE = 0x20,   //SPACEBAR 
        VK_PRIOR = 0x21,   //PAGE UP key 
        VK_NEXT = 0x22,   //PAGE DOWN key 
        VK_END = 0x23,   //END key 
        VK_HOME = 0x24,   //HOME key 
        VK_LEFT = 0x25,   //LEFT ARROW key 
        VK_UP = 0x26,   //UP ARROW key 
        VK_RIGHT = 0x27,   //RIGHT ARROW key 
        VK_DOWN = 0x28,   //DOWN ARROW key 
        VK_SELECT = 0x29,   //SELECT key 
        VK_PRINT = 0x2A,   //PRINT key 
        VK_EXECUTE = 0x2B,   //EXECUTE key 
        VK_SNAPSHOT = 0x2C,   //PRINT SCREEN key 
        VK_INSERT = 0x2D,   //INS key 
        VK_DELETE = 0x2E,   //DEL key 
        VK_HELP = 0x2F,   //HELP key 
        VK_0 = 0x30,   //0 key 
        VK_1 = 0x31,   //1 key 
        VK_2 = 0x32,   //2 key 
        VK_3 = 0x33,   //3 key 
        VK_4 = 0x34,   //4 key 
        VK_5 = 0x35,   //5 key 
        VK_6 = 0x36,    //6 key 
        VK_7 = 0x37,    //7 key 
        VK_8 = 0x38,   //8 key 
        VK_9 = 0x39,    //9 key 
        VK_A = 0x41,   //A key 
        VK_B = 0x42,   //B key 
        VK_C = 0x43,   //C key 
        VK_D = 0x44,   //D key 
        VK_E = 0x45,   //E key 
        VK_F = 0x46,   //F key 
        VK_G = 0x47,   //G key 
        VK_H = 0x48,   //H key 
        VK_I = 0x49,    //I key 
        VK_J = 0x4A,   //J key 
        VK_K = 0x4B,   //K key 
        VK_L = 0x4C,   //L key 
        VK_M = 0x4D,   //M key 
        VK_N = 0x4E,    //N key 
        VK_O = 0x4F,   //O key 
        VK_P = 0x50,    //P key 
        VK_Q = 0x51,   //Q key 
        VK_R = 0x52,   //R key 
        VK_S = 0x53,   //S key 
        VK_T = 0x54,   //T key 
        VK_U = 0x55,   //U key 
        VK_V = 0x56,   //V key 
        VK_W = 0x57,   //W key 
        VK_X = 0x58,   //X key 
        VK_Y = 0x59,   //Y key 
        VK_Z = 0x5A,    //Z key 
        VK_NUMPAD0 = 0x60,   //Numeric keypad 0 key 
        VK_NUMPAD1 = 0x61,   //Numeric keypad 1 key 
        VK_NUMPAD2 = 0x62,   //Numeric keypad 2 key 
        VK_NUMPAD3 = 0x63,   //Numeric keypad 3 key 
        VK_NUMPAD4 = 0x64,   //Numeric keypad 4 key 
        VK_NUMPAD5 = 0x65,   //Numeric keypad 5 key 
        VK_NUMPAD6 = 0x66,   //Numeric keypad 6 key 
        VK_NUMPAD7 = 0x67,   //Numeric keypad 7 key 
        VK_NUMPAD8 = 0x68,   //Numeric keypad 8 key 
        VK_NUMPAD9 = 0x69,   //Numeric keypad 9 key 
        VK_SEPARATOR = 0x6C,   //Separator key 
        VK_SUBTRACT = 0x6D,   //Subtract key 
        VK_DECIMAL = 0x6E,   //Decimal key 
        VK_DIVIDE = 0x6F,   //Divide key 
        VK_F1 = 0x70,   //F1 key 
        VK_F2 = 0x71,   //F2 key 
        VK_F3 = 0x72,   //F3 key 
        VK_F4 = 0x73,   //F4 key 
        VK_F5 = 0x74,   //F5 key 
        VK_F6 = 0x75,   //F6 key 
        VK_F7 = 0x76,   //F7 key 
        VK_F8 = 0x77,   //F8 key 
        VK_F9 = 0x78,   //F9 key 
        VK_F10 = 0x79,   //F10 key 
        VK_F11 = 0x7A,   //F11 key 
        VK_F12 = 0x7B,   //F12 key 
        VK_SCROLL = 0x91,   //SCROLL LOCK key 
        VK_LSHIFT = 0xA0,   //Left SHIFT key 
        VK_RSHIFT = 0xA1,   //Right SHIFT key 
        VK_LCONTROL = 0xA2,   //Left CONTROL key 
        VK_RCONTROL = 0xA3,    //Right CONTROL key 
        VK_LMENU = 0xA4,      //Left MENU key 
        VK_RMENU = 0xA5,   //Right MENU key 
        VK_PLAY = 0xFA,   //Play key 
        VK_ZOOM = 0xFB, //Zoom key 
    }
    
    #endregion

    public class Win32
    {
        /// <summary> 
        /// MakeLParam Macro 
        /// </summary> 
        public static int MakeLParam(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | (LoWord & 0xffff));
        } 

        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);
        [DllImport("User32.dll")]
        public static extern Boolean EnumChildWindows(int hWndParent, Delegate lpEnumFunc, int lParam);
        [DllImport("User32.dll")]
        public static extern Int32 GetWindowText(int hWnd, StringBuilder s, int nMaxCount);
        [DllImport("User32.dll")]
        public static extern Int32 GetWindowTextLength(int hwnd);
        [DllImport("user32.dll")]
        public static extern int GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern int GetClassName(int hWnd, StringBuilder s, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern int RealGetWindowClass(int hWnd, StringBuilder s, int nMaxCount);
        //SetFocus function sets the keyboard focus to the specified window
        [DllImport("User32.dll")]
        public static extern Int32 SetFocus(int hwnd);
        //GetNextDlgGroupItem function retrieves a handle to the first control in a group of controls that precedes (or follows) the specified control in a dialog box
        [DllImport("User32.dll")]
        public static extern int GetNextDlgGroupItem(int hDlg, int hCtl, bool bPrev);
        //GetDlgCtrlID function retrieves the identifier of the specified control
        [DllImport("User32.dll")]
        public static extern int GetDlgCtrlID(int hwndCtrl);
        //Reverse of above GetDlgItem function retrieves a handle to a control in the specified dialog box. hDlg handle to dailog box, IDCtrl is control id
        [DllImport("User32.dll")]
        public static extern int GetDlgItem(int hDlg, int IDCntl);
        //GetDlgItemText function retrieves the title or text associated with a control in a dialog box
        [DllImport("User32.dll")]
        public static extern int GetDlgItemText(int hDlg, int hwndCtrl, StringBuilder s, int nMaxCount);
        //SendMessage to a window function Hwnd window handle msg as an integer and last 2 params are variable. Here spec's as intptr for lenght and s fro string for getset text. Seems as though compiler figures out correct sub passed on params
        [DllImport("User32.dll")]
        public static extern int SendMessage(int hWnd, int Msg, IntPtr wParam, StringBuilder lParam);
        //SendMessage to a window function Hwnd window handle msg as an integer and last 2 params are variable. Here spec's as intptr for getting length
        [DllImport("User32.dll")]
        public static extern int SendMessage(int hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        //SendMessage to a window function FOr control focus. Hnd window handle msg as an integer and last 2 params are variable. Here spec's as wparam is control handle and lparam is true (it would be false for tabstop control movement
        [DllImport("User32.dll")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, bool lParam);
        [DllImport("User32.dll")]
        public static extern int PostMessage(int hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        //GetWindowThreadProcessId function retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        [DllImport("User32.dll")]
        public static extern long GetWindowThreadProcessId(int hWnd, IntPtr lpdwProcessID);
        //The EnumThreadWindows function enumerates all nonchild windows associated with a thread by passing the handle to each window, in turn, to an application-defined callback function. EnumThreadWindows continues until the last window is enumerated or the callback function returns FALSE. To enumerate child windows of a particular window, use the EnumChildWindows function.
        [DllImport("User32.dll")]
        public static extern bool EnumThreadWindows(long thread, Delegate lpEnumFunc, int lParam);
        //Below is setup to get a window state (minimized etc)
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
        private static List<string> winState = new List<string>() { "SW_HIDE", "SW_SHOWNORMAL", "SW_SHOWMINIMIZED", "SW_SHOWMAXIMIZED", "SW_SHOWNOACTIVATE", "SW_SHOW", "SW_MINIMIZE", "SW_SHOWMINNOACTIVE", "SW_SHOWNA", "SW_RESTORE", "SW_SHOWDEFAULT" };
        [DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        public static int WindowGetState(IntPtr hWnd)
        {
            WINDOWPLACEMENT lpwndpl = new WINDOWPLACEMENT();
            lpwndpl.length = Marshal.SizeOf(lpwndpl);
            GetWindowPlacement(hWnd,ref lpwndpl);
            return lpwndpl.showCmd;
        }
        public static string WindowGetStateS(IntPtr hWnd)
        {
            return winState[WindowGetState(hWnd)];
        }
        //End of get window state//start of set window state
        public static void WindowSetState(int hWnd, string wState)
        {
            ShowWindow(hWnd, winState.IndexOf(wState));
        }
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(int hWnd, int nCmdShow);
    }
}
