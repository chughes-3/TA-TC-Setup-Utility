using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Stop_Tax_Aide_Drive
{
#if Log
    class Log
    {
        public static StreamWriter WriteStrm; //declared as static so only 1 instance so get at it via class not object
        public static string logPathFile;
        public Log(string str)//by having class NOT static can declare constructor with an argument to pass string through
        {
            WriteStrm = new StreamWriter(str, true);
            logPathFile = str;
            WriteStrm.AutoFlush = true;
            WriteStrm.WriteLine("***");
            WriteStrm.WriteLine("***");
            WriteStrm.WriteLine("***");
            WriteStrm.WriteLine("            AARP Tax-Aide TrueCrypt Utilities Log Startup: " + DateTime.Now.ToString());
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
#endif
}
