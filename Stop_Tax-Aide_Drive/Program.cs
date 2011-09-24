using System;
using System.Collections.Generic;
using System.Text;
using StartTA_TCFile;

namespace Stop_Tax_Aide_Drive
{
    class Program
    {
        static void Main()
        {
#if Log
            Log tcFileResizerLog = new Log(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TA Utility 4TC TY11.log"); 
#endif
            ProgramData thisProg = new ProgramData();
            TrueCryptSWObj tcSoftware = new TrueCryptSWObj(thisProg);
            WMI_Mine.ServerShareConn(thisProg); //does the warning about sessions
            WMI_Mine.DropConn4Share(TC_Data_FileStop.shareName, TC_Data_FileStop.shareNameLegacy);//drop connections to P drive
            TC_Data_FileStop tcFile = new TC_Data_FileStop(thisProg, tcSoftware);
            tcFile.DeleteShares();
            tcFile.CloseTcFile(thisProg.drvLetter);
            tcFile.CleanUp();
        }
    }
}
