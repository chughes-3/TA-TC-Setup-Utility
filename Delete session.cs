
//>>"pnp" <pnp.@.softlab.ntua.gr> wrote in message
//>>news:ut9vH6TCFHA.1292@TK2MSFTNGP10.phx.gbl...
//>>[color=darkred]
//>>>Hi all,
//>>> from computer management|shared folders|sessions I can see all the
//>>>users in a network that are connected to my PC and how many open files
//>>>they have. Is there a way to monitor this through C# and drop the users
//>>>that I want?
//>>>
//>>>thanks in advance,
//>>>Peter[/color]
//>>
//>>[/color]
//>
//>[/color]

 

//Willy Denoyette [MVP]    Posts: n/a 
//#6: Nov 16 '05  
 
//re: Drop users connected to my PC?

//--------------------------------------------------------------------------------


//"pnp" <pnp.@.softlab.ntua.gr> wrote in message
//news:OJAO49bCFHA.2032@tk2msftngp13.phx.gbl...[color=blue]
//> Yes that is what I want to do. How can I accomplish that?
//>[/color]


//One possibility is to use System.Management classes and the ServerSession
//and ServerConnection WMI classes.
//Note that:
//- the client cannot be stopped to re-open another session, this can happen
//automatically, for instance office applications will re-initiate and re-open
//the files it has currently open.
//- all resources associated with this client session will be closed (files
//are close), this guarantess physical consistency, but not logical
//consistancy.

//Herewith a small sample:

using System;
using System.Management;
class App
{
    public static void Main()
    {
        // remove session for user 'SomeUser'
        DropThisSession(@"SomeUser");
    }

    static void DropThisSession(string objectQry)
    {
        SelectQuery query = new SelectQuery("select ComputerName, UserName, ResourcesOpened from win32_serversession where username ='" + objectQry + "'");
        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
        {
            foreach (ManagementObject mo in searcher.Get())
            {
                Console.WriteLine("Session opened by: {0} from {1} with {2} resources opened", mo.Properties["UserName"].Value, mo.Properties["ComputerName"].Value, mo.Properties["ResourcesOpened"].Value);
                // Optionaly - Get associated serverconnection instance
                foreach (ManagementBaseObject b in mo.GetRelated("Win32_ServerConnection"))
                { 
                    ShowServerConnectionProperties(b.ToString()); 
                }
                // Delete the session, this will close all opened resources (files etc..)for this session
                mo.Delete();
            }
        }
    }
    static void ShowServerConnectionProperties(string objectClass)
    {
        using (ManagementObject process = new ManagementObject(objectClass))
        {
            process.Get();
            PropertyDataCollection processProperties = process.Properties;
            Console.WriteLine("ConnectionID: {0,6} \tShareName: {1}", processProperties["ConnectionID"].Value, processProperties["ShareName"].Value);
        }
    }
}


 
 
 
