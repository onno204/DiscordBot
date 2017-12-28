using DSharpPlus;
using DSharpPlus.Entities;
using Onno204Bot.Cfg;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Onno204Bot.Remote
{
    class RemoteUpdateInfo
    {
        public static async Task Update() {

            WebClient wc = new WebClient();
            NameValueCollection vals = new NameValueCollection();
            DUser duser = GetSetItems.LastDUser;
            DiscordChannel TextChannel = duser.TextChannel;
            DiscordChannel VoiceChannel = Program.Voice.GetConnection(duser.Guild).Channel;
            if (GetSetItems.RUTextID != 0) {
                TextChannel = await Program.discord.GetChannelAsync(GetSetItems.RUTextID);
                GetSetItems.RUTextID = 0;
            }
            if (GetSetItems.RUVoiceID != 0) {
                VoiceChannel= await Program.discord.GetChannelAsync(GetSetItems.RUVoiceID);
                GetSetItems.RUVoiceID = 0;
            }

            //Text channel info
            vals.Add("CurrentText", TextChannel.Name);
            vals.Add("CurrentTextID", TextChannel.Id + "");
            vals.Add("CurrentTextCount", "Not available yet!");
            vals.Add("CurrentTextMembers", "Not available yet!");
            vals.Add("CurrentTextGuildName", TextChannel.Guild.Name);
            vals.Add("CurrentTextGuildIcon", TextChannel.Guild.IconUrl);
            //Voice channel info
            VoiceChannelInfo VCI = new VoiceChannelInfo(VoiceChannel);
            vals.Add("CurrentVoice", VCI.Name);
            vals.Add("CurrentVoiceID", VCI.ID + "");
            vals.Add("CurrentVoiceCount", VCI.Count+"");
            vals.Add("CurrentVoiceDeaf", VCI.Deaf+"");
            vals.Add("CurrentVoiceMuted", VCI.Muted+"");
            vals.Add("CurrentVoiceMembers", VCI.UsersString);
            vals.Add("CurrentVoiceAvatarURLS", VCI.AvatarURL);
            vals.Add("CurrentVoiceGuildName", VCI.Guild.Name);
            vals.Add("CurrentVoiceGuildIcon", VCI.Guild.IconUrl);
            try { 
            vals.Add("CurrentVoiceSelf", Program.Voice.GetConnection(duser.Guild).Channel.Name);
            }catch(Exception e) {
                Utils.Log(e.Message + ":" + e.StackTrace, LogType.Error);
                vals.Add("CurrentVoiceSelf", "Voicechannel Error!");
            }
            //Self info
            DiscordClient Self = Program.discord;
            vals.Add("SelfAvatarURL", Self.CurrentUser.AvatarUrl);
            vals.Add("SelfDMChannelsCount", Self.PrivateChannels.Count + "");
            String SelfDmChannelsString = "";
            foreach (DiscordChannel chnl in Self.PrivateChannels) { SelfDmChannelsString += ", " + chnl.Name; }
            vals.Add("SelfDmChannelsMembers", SelfDmChannelsString);
            String SelfguildsString = "";
            foreach (DiscordGuild guild in Self.Guilds.Values) { SelfguildsString += ", " + guild.Name; }
            vals.Add("SelfGuildsNames", SelfguildsString);
            vals.Add("SelfGuildsCount", Self.Guilds.Count+"");
            vals.Add("SelfMFA", Self.CurrentUser.MfaEnabled + "");
            vals.Add("SelfPing", Self.Ping + "");
            vals.Add("SelfName", Self.CurrentUser.Username);
            String voiceState = "";
            foreach (DiscordVoiceState dvs in duser.Guild.VoiceStates) {
                if (dvs.User.Id == Self.CurrentUser.Id) {
                    voiceState = "Deaf:" + dvs.Deaf + "," + "Mute:" + dvs.Mute;
                }
            }
            vals.Add("SelfVoiceState", voiceState);
            String GuildNames = "";
            foreach (DiscordGuild guilds in Program.discord.Guilds.Values) {
                GuildNames += ", " + guilds.Name;
            }
            vals.Add("SelfVoiceState", voiceState);

            //Upload to the DB
            wc.UploadValues(RemoteConf.URL + "UpdateInfo.php", vals);
            await Task.Delay(1);
        }



    }


}
