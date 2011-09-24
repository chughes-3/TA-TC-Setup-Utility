using System.Reflection;
using System.Text.RegularExpressions;

namespace Start_Traveler
{
    class Program
    {
        static string pattern = "(?<=//)[a-zA-Z]:";    //matches // followed by a letter followed by :
        static void Main(string[] args)
        {
            string scriptExePath = Assembly.GetEntryAssembly().CodeBase;    // format is, file:///D:/blah/blah.exe so 3 slashes then next 2 This gete eg e:
            Match m = new Regex(pattern).Match(scriptExePath);
            System.Diagnostics.Process.Start(m.Value + "\\Tax-Aide_Traveler\\Start_Tax-Aide_Drive.exe");
        }
    }
}
