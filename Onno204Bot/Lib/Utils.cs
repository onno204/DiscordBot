using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onno204Bot
{
    public enum LogType { Chat, Console, Log, SendedMessages, Error, DeletedMessages, ActionLog, Misc };
    class Utils
    {

        /// <summary>
        /// LogType's at the bottom
        /// </summary>
        /// <param name="Text">Text to write</param>
        /// <param name="LT">Log Type</param>
        public static void Log(String Text, LogType LT)
        {
            if (!Directory.Exists(Config.LogDir)) { Directory.CreateDirectory(Config.LogDir); }
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            String MethodName = stackTrace.GetFrame(1).GetMethod().Name;
            String FileName = stackTrace.GetFrame(1).GetFileName();
            Text = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt") + "("+FileName+"//"+MethodName+")-> " + Text;
            if (LT == LogType.Chat) {//Chat Log
                Log("Chat ||| " + Text, LogType.Console);
                WriteToFile(Config.LogDir + "ChatLog.txt", Text);
            }
            else if (LT == LogType.Error) { // Error Log
                Log("ERROR Log ||| " + Text, LogType.Console);
                WriteToFile(Config.LogDir + "ErrorLog.txt", Text);
            }
            else if (LT == LogType.ActionLog) { //ActionLog
                Log("Action Log ||| " + Text, LogType.Console);
                WriteToFile(Config.LogDir + "ActionLog.txt", Text);
            }
            else if (LT == LogType.SendedMessages) { //Sended messages Log
                Log("SendMessage ||| " + Text, LogType.Console);
                WriteToFile(Config.LogDir + "SendedMessages.txt", Text);
            }
            else if (LT == LogType.DeletedMessages) { //Deleted messages Log
                Log("Deleted Messages ||| " + Text, LogType.Console);
                WriteToFile(Config.LogDir + "DeletedMessages.txt", Text);
            }
            else if (LT == LogType.Log) { //Log?
                Log("Unknow Log ||| " + Text, LogType.Console);
                WriteToFile(Config.LogDir + "DerpLog.txt", Text);
            }
            else if (LT == LogType.Console) {//Console Output
                Console.WriteLine(Text);
            }else { // Unspecified
                Log("Not specified Logg ||| " + Text, LogType.Console);
                WriteToFile(Config.LogDir + "Log.txt", Text);
            }
        }

        public static void Debug(object obj)
        {
            Console.WriteLine("Debug: " + obj.ToString());
        }

        public static void WriteToFile(String path, String Text)
        {
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(Text);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(Text);
            }
        }

        public static void ReadFile(String path)
        {
            // Open the file to read from.
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
        }


        /// <summary>
        /// Replace the first Occurence in a string
        /// </summary>
        /// <param name="Source">The full string</param>
        /// <param name="Find">What to replace</param>
        /// <param name="Replace">Replace with</param>
        /// <returns></returns>
        public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            try {
                int Place = Source.IndexOf(Find);
                string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
                return result;
            }catch (Exception e) { Log("Error: " + e.Message, LogType.Error); }
            return "";
        }

        /// <summary>
        /// Replace the Last Occurence in a string
        /// </summary>
        /// <param name="Source">The full string</param>
        /// <param name="Find">What to replace</param>
        /// <param name="Replace">Replace with</param>
        /// <returns></returns>
        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            try
            {
                int Place = Source.LastIndexOf(Find);
                string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
                return result;
            }catch (Exception e) { Log("Error: " + e.Message, LogType.Error); }
            return "";
        }

        /// <summary>
        /// Replace all the Occurences in a string
        /// </summary>
        /// <param name="Source">The full string</param>
        /// <param name="Find">What to replace</param>
        /// <param name="Replace">Replace with</param>
        /// <returns></returns>
        public static string Replace(string Source, string Find, string Replace)
        {
            return Source.Replace(Find, Replace);
        }

        /// <summary>
        /// Get the word at a specific count
        /// </summary>
        /// <param name="Text">Full Text</param>
        /// <param name="At">Wordt to find(First word = 1)</param>
        /// <returns>Returns one word</returns>
        public static string GetWordAt(String Text, int At) {
            return Text.Split(' ')[At-1];
        }


        /// <summary>
        /// Removes Special chracaters from a string
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Output String</returns>
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }




    }
}
