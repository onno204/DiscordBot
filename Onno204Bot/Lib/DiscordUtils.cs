using DSharpPlus.Entities;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Onno204Bot.Lib
{
    internal class DiscordUtils
    {
        public static async Task SendBotMessage(string message, DUser duser = null)
        {
            if (Config.BlacklistedServers.Contains(duser.TextChannel.Parent.Name) || message == "" || message.StartsWith("#") && !Config.AlotOfChatOutput)
                return;
            string[] Sentances = message.Split('$');
            int i = 0;
            for (i = 0; i < Sentances.Length; ++i)
            {
                string Sentance = Sentances[i];
                ConfiguredTaskAwaitable<DiscordMessage> configuredTaskAwaitable;
                if (Sentance.StartsWith("^"))
                {
                    Sentance = Utils.ReplaceFirstOccurrence(Sentance, "^", "");
                    Utils.Log(Sentance, LogType.SendedMessages);
                    if (Config.NoChatOutput)
                        break;
                    if ((uint)duser.RemoteInt > 0U)
                    {
                        Console.WriteLine("Returning: " + (object)duser.RemoteInt);
                        Onno204Bot.Remote.Remote.SendDone(duser.RemoteInt, Sentance);
                    }
                    else
                    {
                        configuredTaskAwaitable = Program.discord.SendMessageAsync(duser.TextChannel, Sentance, false, (DiscordEmbed)null).ConfigureAwait(false);
                        DiscordMessage discordMessage = await configuredTaskAwaitable;
                    }
                }
                else
                {
                    string Msg = Sentance;
                    DiscordEmbedBuilder.EmbedAuthor DEFA = (DiscordEmbedBuilder.EmbedAuthor)null;
                    DiscordEmbedBuilder.EmbedFooter DEF = (DiscordEmbedBuilder.EmbedFooter)null;
                    if (duser != null)
                    {
                        DEFA = new DiscordEmbedBuilder.EmbedAuthor()
                        {
                            IconUrl = Program.discord.CurrentUser.AvatarUrl,
                            Name = Program.discord.CurrentUser.Username + "#" + Program.discord.CurrentUser.Discriminator
                        };
                        DEF = new DiscordEmbedBuilder.EmbedFooter()
                        {
                            IconUrl = duser.Member.AvatarUrl,
                            Text = "Executed by: " + duser.Member.Username + "#" + duser.Member.Discriminator
                        };
                    }
                    DiscordEmbed demd = (DiscordEmbed)new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Orange,
                        Description = Msg,
                        Url = Config.ReplyLink,
                        Footer = DEF,
                        Author = DEFA
                    };
                    Utils.Log(Msg, LogType.SendedMessages);
                    if ((uint)duser.RemoteInt > 0U)
                    {
                        Console.WriteLine("Returning: " + (object)duser.RemoteInt);
                        Onno204Bot.Remote.Remote.SendDone(duser.RemoteInt, Msg);
                        break;
                    }
                    if (Config.NoChatOutput)
                        break;
                    configuredTaskAwaitable = Program.discord.SendMessageAsync(duser.TextChannel, "", false, demd).ConfigureAwait(false);
                    DiscordMessage discordMessage = await configuredTaskAwaitable;
                    Sentance = (string)null;
                    Msg = (string)null;
                    DEFA = (DiscordEmbedBuilder.EmbedAuthor)null;
                    DEF = (DiscordEmbedBuilder.EmbedFooter)null;
                    demd = (DiscordEmbed)null;
                }
            }
        }

        public static void RemoveMessageAfterAWhile(ulong MessageID, int TimeS)
        {
            new Thread((ThreadStart)(() => DiscordUtils.RemoveMessageAfterAWhileT(MessageID, TimeS))).Start();
        }

        public static void RemoveMessageAfterAWhileT(ulong MessageID, int TimeS)
        {
        }
    }
}
