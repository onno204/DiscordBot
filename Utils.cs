using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onno204Bot
{
    class Utils
    {

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
            }catch (Exception e) { Program.Log("Error: " + e.Message, LogType.Error); }
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
            }catch (Exception e) { Program.Log("Error: " + e.Message, LogType.Error); }
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


    }
}
