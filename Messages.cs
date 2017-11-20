using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onno204Bot
{
    class Messages
    {
        //Leave a message empty and it won't be send!

            //Markup and Style
        //! = Send it with markup style (ONLY at the Start of the message!)
        //@ = Send it with Title (ONLY at the Start of the message!)
            //First the '!' Than the '@' !!

        //~1 - ~2 - ~3 - ~4 --- e.t.c.
        public static String BotTitle = "***#Onno204 bot at your service!***";

        public static String CommandNotFound = "!@Het commando '~1' hebben we niet herkent in ons systeem."; // ~1 = Command
        public static String EchoReply = "@Een speciaal berichtje van '~1'. \n\n~2"; // ~1 = User --- ~2 = Message
        public static String MentionReply = "!Ik kom zo snel mogelijk bij je terug! ~1"; // ~1 = User --- ~2 = Message
        public static String HelpReply = "!@Nog geen Hulp pagina Beschrikbaar!"; // ~1 = User
        
    }
}
