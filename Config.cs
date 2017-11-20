using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onno204Bot
{
    class Config
    {
        //Inlog
        public static String Email = "restafval@hotmail.nl";
        public static String Password = Program.DecryptString("fkC5+RA67v0evNDBMEhbXQ==", "Ecrypted");

        //Chat
        public static String CommandString = "::";

        //Misc
        public static String BlaclistedServers = "Prio1Gaming,Prio2Gaming";
        public static bool Exiting = false;
        public static CancellationToken cancelToken;
    }
}
