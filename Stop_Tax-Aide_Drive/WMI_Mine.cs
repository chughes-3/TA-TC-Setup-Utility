using System;
using System.Text;
using System.Management;
using StartTA_TCFile;
using System.Windows.Forms;

namespace Stop_Tax_Aide_Drive
{
    class WMI_Mine
    {
        public static void ServerShareConn(ProgramData thisProg)
        {
            string messBoxStr1 = " Sessions from other computers to the shared drives still exist\r\n";
            string messBoxStr2 = " Session from another computer to the shared drives still exists\r\n";
            string messBoxStr3 = "This likely means that workstations are still powered on\r\nIf everyone is logged out of Taxwise continuing will do no harm\r\n\r\nDo you want to continue anyway?";
            StringBuilder messBoxStr = new StringBuilder();
            int conn = 0;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ConnectionShare");
                ManagementObjectCollection connShares = searcher.Get();
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj.GetPropertyValue("Antecedent").ToString().Contains(TC_Data_FileStop.shareName))
                    {
                        string str = queryObj.GetPropertyValue("Dependent").ToString();
                        messBoxStr.AppendLine("Connection to " + TC_Data_FileStop.shareName + " from " + str.Substring(str.IndexOf("ComputerName"),str.IndexOf("ShareName")-str.IndexOf("ComputerName")));
                        conn++;
                    }
                    if (queryObj.GetPropertyValue("Antecedent").ToString().Contains(TC_Data_FileStop.shareNameLegacy))
                    {
                        string str = queryObj.GetPropertyValue("Dependent").ToString();
                        messBoxStr.AppendLine("Connection to " + TC_Data_FileStop.shareNameLegacy + " from " + str.Substring(str.IndexOf("ComputerName"), str.IndexOf("ShareName") - str.IndexOf("ComputerName")));
                        conn++;
                    }
                }
                if (conn > 0)
                {
                    if (conn == 1)
                        messBoxStr.Append(Environment.NewLine + conn.ToString() + messBoxStr2);
                    else
                        messBoxStr.Append(Environment.NewLine + conn.ToString() + messBoxStr1);
                    messBoxStr.Append(messBoxStr3);
                    DialogResult dr = MessageBox.Show(messBoxStr.ToString(), thisProg.mbCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == DialogResult.No)
                        Environment.Exit(1);
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
            }
        }
        public static void DropConn4Share(string shName1, string shName2)
        {
            SelectQuery query = new SelectQuery("Select ComputerName, UserName from win32_ServerSession");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject srvSess in searcher.Get())
                {
                    foreach (ManagementBaseObject servConn in srvSess.GetRelated("Win32_ServerConnection"))
                    {
                        if (servConn.GetPropertyValue("ShareName").ToString() == shName1 | servConn.GetPropertyValue("ShareName").ToString() == shName2)
                        {
                            srvSess.Delete();
                        }
                    }
                }
            }
        }
    }
}
