using System;
using System.Collections.Generic;
using System.Text;

namespace TaxAide_TrueCrypt_Utility
{
    public class TasksBitField
    {
        [FlagsAttribute]
        public enum Flag : uint
        {
            Clear    = 0x00,                            //FOR LIST BELOW USE BEGIN OF COLUMN NAMES NOT RIGHT OF ==s
            hdTcfileOldRename = 0x01,                   //1
            hdTASwOldDelete = hdTcfileOldRename << 1,   //2
            hdTcSwUninstall = hdTASwOldDelete << 1,     //4
            hdTcSwInstall = hdTcSwUninstall << 1,       //8
            hdTaxaideSwInstall = hdTcSwInstall << 1,    //10
            hdTCFileFormat = hdTaxaideSwInstall << 1,   //20
            travTcfileOldCopy = hdTCFileFormat << 1,    //40
            travTASwOldDelete = travTcfileOldCopy << 1, //80    
            travSwInstall = travTASwOldDelete << 1,     //100
            travtcFileFormat = travSwInstall << 1,      //200
            travTASwOldIsver6_2 = travtcFileFormat <<1  //400
        }
        private uint taskBits;
        public uint taskList
        {
            get { return taskBits; }
            set { taskBits = value; }
        }
        public void SetFlag(Flag flg)
        {
            taskList |= (uint)flg;
        }
        public bool IsOn(Flag flg)
        {
            return (taskList & (uint)flg) == (uint)flg;
        }
        public bool TestTrav()
        {
            return (taskList & (uint)0x3c0) != 0;
        }
        public void LogTasks()
        {
            if (this.IsOn(Flag.hdTcfileOldRename))
            {
                Log.WriteStrm.WriteLine("HD OLD TC File(s) Rename");
            }
            if (this.IsOn(Flag.hdTASwOldDelete))
            {
                Log.WriteStrm.WriteLine("HD old Tax-Aide SW delete");
            }
            if (this.IsOn(Flag.hdTcSwUninstall))
            {
                Log.WriteStrm.WriteLine("HD TC Uninstall");
            }
            if (this.IsOn(Flag.hdTcSwInstall))
            {
                Log.WriteStrm.WriteLine("HD TC Install");
            }
            if (this.IsOn(Flag.hdTaxaideSwInstall))
            {
                Log.WriteStrm.WriteLine("HD Tax-Aide SW Install");
            }
            if (this.IsOn(Flag.hdTCFileFormat))
            {
                Log.WriteStrm.WriteLine("HD TC File Create/Format");
            }
            if (this.IsOn(Flag.travTcfileOldCopy))
            {
                Log.WriteStrm.WriteLine("Trav Old TC file Copy to hard Drive");
            }
            if (this.IsOn(Flag.travTASwOldDelete))
            {
                Log.WriteStrm.WriteLine("Trav Delete Old Tax-Aide Software");
            }
            if (this.IsOn(Flag.travSwInstall))
            {
                Log.WriteStrm.WriteLine("Trav Install Software");
            }
            if (this.IsOn(Flag.travtcFileFormat))
            {
                Log.WriteStrm.WriteLine("Trav tc file Create/Format");
            }
        }
    }
}
