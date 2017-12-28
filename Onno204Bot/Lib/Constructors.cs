using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onno204Bot.Lib
{
    class Constructors
    {
    }
    class VoiceChannelInfo {

        public int Count { get; set; }
        public int Deaf { get; set; }
        public ulong ID { get; set; }
        public String Name { get; set; }
        public String AvatarURL { get; set; }
        public int Muted { get; set; }
        public List<DiscordUser> Users { get; set; }
        public String UsersString { get; set; }
        public DiscordChannel Channel { get; set; }
        public DiscordGuild Guild { get; set; }

        public VoiceChannelInfo(DiscordChannel VoiceChnl) {
            try
            {
                Count = GetAmountInVoice(VoiceChnl);
                Channel = VoiceChnl;
                Guild = VoiceChnl.Guild;
                Name = VoiceChnl.Name;
                ID = VoiceChnl.Id;
                Users = new List<DiscordUser>();
                foreach (DiscordVoiceState dvs in VoiceChnl.Guild.VoiceStates)
                {
                    if (dvs.Channel.Id == VoiceChnl.Id)
                    {
                        try
                        {
                            if (dvs.SelfDeaf) { Deaf++; }
                            if (dvs.SelfMute) { Muted++; }
                            UsersString += ", " + dvs.User.Username;
                            AvatarURL += ", " + dvs.User.AvatarUrl;
                            Users.Add(dvs.User);
                        }catch (Exception e) {
                            Utils.Log(e.Message + "//\n" + e.StackTrace + "\n"+dvs.ToString(), LogType.Error);
                        }
                    }
                }
            } catch (Exception e) {
                Utils.Log(e.Message + "//\n" + e.StackTrace, LogType.Error);
            }

        }

        public int GetAmountInVoice(DiscordChannel VoiceChn) {
            int Amount = 0;
            foreach (DiscordVoiceState dvs in VoiceChn.Guild.VoiceStates) {
                if (dvs.Channel.Id == VoiceChn.Id) { Amount++; }
            }
            return Amount;
        }
    }
}
