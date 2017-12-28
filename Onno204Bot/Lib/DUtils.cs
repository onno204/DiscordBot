using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VideoLibrary;

namespace Onno204Bot.Lib
{
    internal class DUtils
    {
        public static List<DUser> DUserList = new List<DUser>();

        public static async Task<DiscordUser> GetMemberFromID(ulong id)
        {
            DiscordUser userAsync = await Program.discord.GetUserAsync(id);
            return userAsync;
        }

        public static async Task<DiscordChannel> GetChannelFromID(ulong id)
        {
            DiscordChannel channelAsync = await Program.discord.GetChannelAsync(id);
            return channelAsync;
        }

        public static void Debug(object obj)
        {
            Console.WriteLine("Debug: " + obj.ToString());
        }

        public static string GetMusicURL(Video video)
        {
            string path = Config.VideoDir + "VideoURL.json";
            if (!File.Exists(path))
                File.Create(path).Close();
            JObject jobject = JObject.Parse(File.ReadAllText(path));
            string str = jobject[video.Title].ToString();
            File.WriteAllText(path, jobject.ToString());
            return str;
        }

        public static void SetMusicURL(Video video, string URL)
        {
            string path = Config.VideoDir + "VideoURL.json";
            if (!File.Exists(path))
                File.Create(path).Close();
            JObject jobject = (JObject)null;
            try
            {
                jobject = JObject.Parse(File.ReadAllText(path));
            }
            catch (Exception ex) { Utils.Log(ex.Message + ":" + ex.StackTrace, LogType.Error); }
            if (jobject == null)
                jobject = new JObject();
            jobject[video.Title] = (JToken)URL;
            File.WriteAllText(path, jobject.ToString());
        }

        public static int GetAmountInVoice(DiscordChannel VoiceChn)
        {
            int num = 0;
            foreach (DiscordVoiceState voiceState in (IEnumerable<DiscordVoiceState>)VoiceChn.Guild.VoiceStates)
            {
                if ((long)voiceState.Channel.Id == (long)VoiceChn.Id)
                    ++num;
            }
            return num;
        }
    }
}
