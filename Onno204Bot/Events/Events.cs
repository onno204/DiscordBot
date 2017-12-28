using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Onno204Bot.Lib;
using DSharpPlus.Entities;

namespace Onno204Bot.Events
{
    class Events
    {
        public static async Task RegisterEvent() {
            return;


            ///////////////////
            /////////////////////
            ///MUSIC BOT CONTAINS THIS FUCKING CODE

            IReadOnlyDictionary<ulong, DiscordGuild> Guilds = Program.discord.Guilds;
            for (int i=0; i < Guilds.Count(); i++) {
                ulong GuildID = Guilds.ElementAt(i).Key;
                //Here starts the voice part
                Program.Voice.GetConnection(await Program.discord.GetGuildAsync(GuildID)).VoiceReceived += async e =>
                {
                    try
                    {
                        if (Config.StopPlayingIfANYsoundIsReceived) {
                            MusicBot.StopPlayingJoined = true;
                            return;
                        }
                        Utils.Debug("Musicbot, " + "Received sounds!!!" + e.User.Username);
                    }
                    catch (Exception ee)
                    {
                        if (Config.StopPlayingWithNewPlayer) {
                            MusicBot.StopPlayingJoined = true;
                        }
                        /*
                        Utils.Debug("VoiceEvent ID, " + ee.);
                        if (ee.Message.Contains("De objectverwijzing is niet op een exemplaar van een object ingesteld."))
                        {
                            if (Config.StopPlayingWithNewPlayer) {
                                MusicBot.StopPlayingJoined = true;
                            }
                        }
                        */
                        Utils.Log("Error(" + ee.Message + "), " + ee.StackTrace, LogType.Error);
                    }
                };
            }

            Program.discord.MessageCreated += async e => {
                try
                {
                    if (e.Message.Content.StartsWith(Config.CommandString)) {
                        string te = Utils.ReplaceFirstOccurrence(e.Message.Content, Config.CommandString, "");
                        string[] Splitted = te.Split(' ');
                        string Command = (Splitted[0]).ToLower();
                        string Args = Utils.ReplaceFirstOccurrence(te, Splitted[0], "");
                        string[] Arg = new string[Splitted.Length - 1];
                        for (int i = 1; i < Splitted.Length; i++) {
                            Arg[i - 1] = Splitted[i];
                        }

                        DUser user = new DUser(e.Message.ChannelId, 0, e.Message.Channel.GuildId, e.Author.Id, command: Command, Arg: Arg, Args: Args);
                        
                        return;
                    }
                }
                catch (Exception ee)
                {
                    Utils.Log("Error," + ee.Message + "<:>" + ee.StackTrace, LogType.Error);
                }
            };

        }


    }
}
