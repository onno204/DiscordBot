using DSharpPlus.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onno204Bot
{
    class Config
    {

        //new Thread(() => ConsoleBeep()).Start();
        //Inlog
        //Reading Password and username from file since this is on discord
        //To login with a bot token go to Program -> 251, Enter bot token -> 252, Set token type to bot!
        public static String Email = File.ReadAllText("A:\\Projects\\VisualStudio\\Discord\\Email.txt");
        //You can remove the decrypt string but it's for my own security reason
        public static String Password = Program.DecryptString(File.ReadAllText("A:\\Projects\\VisualStudio\\Discord\\Password.txt"), "Ecrypted");

        //Chat
        public static String CommandString = "::";
        public static String ReplyLink = "http://prio1gaming.nl.eu.org";
        public static bool NoChatOutput = true;

        //Log
        public static string LogDir = Directory.GetCurrentDirectory() + "\\Logs\\";

        //Misc
        //public static String BlaclistedServers = "Prio1Gaming,Prio2Gaming";
        public static String BlaclistedServers = "none";
        public static bool Exiting = false;
    }
}
