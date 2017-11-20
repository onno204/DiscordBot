using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onno204Bot.Lib
{
    class DiscordUtils
    {


        public static async Task SendBotMessage(String message, DiscordChannel channel, DiscordUser user = null)
        {
            if (Config.BlaclistedServers.Contains(channel.Parent.Name)) { return; } //Check if server is blacklisted
            if (message == "") { return; } //Check if message = Empty(Rule to not send message)
            string[] Sentances = message.Split('$');
            int i = 0;
            for (i = 0; i < Sentances.Length; i++) {
                String Sentance = Sentances[i];
                //Construct Sentance
                String Msg = Sentance;
                DiscordEmbedBuilder.EmbedAuthor DEFA = null;
                DiscordEmbedBuilder.EmbedFooter DEF = null;
                if (user != null) {
                    DEF = new DiscordEmbedBuilder.EmbedFooter {
                        IconUrl = user.AvatarUrl,
                        Text = "Executed by: " + user.Username + "#" + user.Discriminator
                    };
                    DEFA = new DiscordEmbedBuilder.EmbedAuthor {
                        IconUrl = Program.discord.CurrentUser.AvatarUrl,
                        Name = Program.discord.CurrentUser.Username + "#" + Program.discord.CurrentUser.Discriminator
                    };
                }

                DiscordEmbed demd = new DiscordEmbedBuilder {
                    Color = DiscordColor.Orange,
                    Description = Msg,
                    //ImageUrl = "http://onno204vps.nl.eu.org/Site1/AddOns/Pictures/Image.png",
                    Url = Config.ReplyLink,
                    Footer = DEF,
                    Author = DEFA
                };

                Utils.Log(Msg, LogType.SendedMessages);
                if (Config.NoChatOutput) { return; }
                await Program.discord.SendMessageAsync(channel, "", embed: demd).ConfigureAwait(false);
            }
        }
        

        public static void RemoveMessageAfterAWhile(ulong MessageID, int TimeS) {
            new Thread(() => RemoveMessageAfterAWhileT(MessageID, TimeS)).Start();
        }
        public static void RemoveMessageAfterAWhileT(ulong MessageID, int TimeS){
            //new Thread(() => ConsoleBeep()).Start();
        }

    }
}
