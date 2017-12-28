using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using MediaToolkit;
using MediaToolkit.Model;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Onno204Bot.Cfg;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoLibrary;

namespace Onno204Bot.Events
{
    class CommandsEvents {
        public static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            Utils.Log(DateTime.Now + " Command: " + e.Context.Member.Username + ", " + e.Context.Message.Content, LogType.ActionLog);
            return Task.CompletedTask;
        }

    }

    [Group("admin")] // let's mark this class as a command group
    [Description("Administrative commands.")] // give it a description for help purposes
    [Hidden] // let's hide this from the eyes of curious users
    [RequirePermissions(Permissions.ManageGuild)] // and restrict this to users who have appropriate permissions
    public class AdminCommands
    {
        // all the commands will need to be executed as <prefix>admin <command> <arguments>

        // this command will be only executable by the bot's owner
        [Command("sudo"), Description("Executes a command as another user."), Hidden, RequireOwner]
        public async Task Sudo(CommandContext ctx, [Description("Member to execute as.")] DiscordMember member, [RemainingText, Description("Command text to execute.")] string command)
        {
            // note the [RemainingText] attribute on the argument.
            await ctx.TriggerTypingAsync();
            var cmds = ctx.CommandsNext;
            await cmds.SudoAsync(member, ctx.Channel, command);
        }
    }
    class Commands
    {
        [Command("hi")]
        public async Task Hi(CommandContext ctx)
        {
            await TryDelete(ctx);
            await ctx.RespondAsync($"👋 Hi, {ctx.User.Mention}!");
        }

        //Aliases("?", "h", "hulp", "kill")
        [Command("echo"), Description("Echo's whatever u say."), Aliases("repeat")]
        public async Task CMDEcho(CommandContext ctx)
        {
            await TryDelete(ctx);
            await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.EchoReply, "~2", ctx.RawArgumentString), "~1", ctx.Message.Author.Mention), ctx.Message.Channel, user: ctx.User);
        }

        [Command("ping"), Description("Internet Check."), Aliases("pong", "pingpong")]
        public async Task CMDPing(CommandContext ctx)
        {
            await TryDelete(ctx);
            await DiscordUtils.SendBotMessage("Pong!", ctx.Message.Channel, user: ctx.User);
        }

        [Command("purge"), Description("Remove X amount of messages."), Aliases("delete")]
        public async Task CMDPurge(CommandContext ctx, [Description("Amount of messages to purge")] int amount)
        {
            IReadOnlyList<DiscordMessage> List = await ctx.Channel.GetMessagesAsync(amount);
            ulong ChannelID = ctx.Channel.Id;
            foreach (DiscordMessage DM in List) {
                if (DM.ChannelId == ChannelID) {
                    await DM.DeleteAsync();
                }
            }
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.PurgeMessage, "~1", amount.ToString()), ctx.Message.Channel, user: ctx.User);
            await TryDelete(ctx);
        }

        [Command("join"), Description("Makes me join a voice channel."), Aliases("add")]
        public async Task Join(CommandContext ctx, DiscordChannel chn = null)
        {
            await TryDelete(ctx);
            await JoinCh(ctx, chn);
        }

        public static async Task JoinCh(CommandContext ctx, DiscordChannel chn = null) {
            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNextClient();
            if (vnext == null)
            {
                // not enabled
                await DiscordUtils.SendBotMessage("VNext is not enabled or configured, contact the admin", ctx.Message.Channel, user: ctx.User);
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
            {
                // already connected
                await DiscordUtils.SendBotMessage(Messages.AudioAlreadyInguild, ctx.Message.Channel, user: ctx.User);
                return;
            }

            // get member's voice state
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel == null && chn == null)
            {
                // they did not specify a channel and are not in one
                await DiscordUtils.SendBotMessage(Messages.AudioUserNotInChannel, ctx.Message.Channel, user: ctx.User);
                return;
            }

            // channel not specified, use user's
            if (chn == null)
                chn = vstat.Channel;

            // connect
            vnc = await vnext.ConnectAsync(chn);
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioConnectedTo, "~1", chn.Name), ctx.Message.Channel, user: ctx.User);
        }

        [Command("leave"), Description("Makes me leave a voice channel."), Aliases("exit", "quit")]
        public async Task Leave(CommandContext ctx)
        {
            await TryDelete(ctx);
            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNextClient();
            if (vnext == null)
            {
                // not enabled
                await ctx.RespondAsync("VNext is not enabled or configured.");
                return;
            }

            // check whether we are connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                // not connected
                await DiscordUtils.SendBotMessage(Messages.AudioNotConnected, ctx.Message.Channel, user: ctx.User);
                return;
            }

            // disconnect
            vnc.Disconnect();
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioDisconnected, "~1", vnc.Channel.Name), ctx.Message.Channel, user: ctx.User);
        }
        
        [Command("play"), Description("Plays an youtube Video(Only youtube links, No search!)."), Aliases("song", "music")]
        public async Task Play(CommandContext ctx, [RemainingText, Description("Full youtube URL.")] string YoutubeURL)
        {
            if(!YoutubeURL.ToLower().StartsWith("http://")){ YoutubeURL = "https://"+YoutubeURL; }

<<<<<<< HEAD
        [Command("test2"), Description("TestCommand!")]
        public async Task CMDTest(CommandContext ctx)
        {
            DUser duser = new DUser(ctx, true);
            await DiscordUtils.SendBotMessage(ctx.Member.VoiceState.ToString() + "hey", new DUser(ctx, true));
            await Remote.Remote.Start();
        }

        [Command("remote"), Description("Start remote Control!"), Aliases("webcommands", "startremote")]
        public async Task CMDRemote(CommandContext ctx) {
=======
>>>>>>> parent of 0992202... Now support for Custom Webinterface(Code not public YET)
            await TryDelete(ctx);
            string filename = "";
            Video vid = null;
            try
            {
                YouTube youtube = YouTube.Default;
                vid = youtube.GetVideo(YoutubeURL);
                if (!Directory.Exists("Bot")) { Directory.CreateDirectory("Bot"); }
                
                filename = MusicBot.CreatePathFromVid(vid);
                if (!File.Exists(filename)) {
                    await DiscordUtils.SendBotMessage(Messages.AudioDownloading, ctx.Message.Channel, user: ctx.User);
                    string filenameNoMP3 = @Directory.GetCurrentDirectory() + "\\Bot\\" + vid.FullName + ".Temp";
                    File.WriteAllBytes(filenameNoMP3, vid.GetBytes());
                    var inputFile = new MediaFile { Filename = filenameNoMP3 };
                    var outputFile = new MediaFile { Filename = filename };

                    using (var engine = new Engine()) {
                        engine.GetMetadata(inputFile);
                        engine.Convert(inputFile, outputFile);
                    }
                    File.Delete(filenameNoMP3);
                }
            }
            catch (Exception e)
            {
                await ctx.RespondAsync("Error: " + e.Message);
                await Task.Delay(1);
                return;
            }
            if(vid !=null)
                MusicBot.AddToQueue(vid);
                await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicAddedToQueue, "~1", vid.Title), ctx.Message.Channel, user: ctx.User);
            await MusicBot.Play(ctx);
        }
        [Command("remoteupdate"), Description("Update Remote info!")]
        public async Task CMDUpdateRemote(CommandContext ctx) {
            if (GetSetItems.LastDUser == null) { new DUser(ctx, true); }
            await TryDelete(ctx);
            await Remote.RemoteUpdateInfo.Update();
        }

        [Command("queue"), Description("Shows the current queue."), Aliases("songs", "playlist")]
        public async Task CMDqueue(CommandContext ctx)
        {
            await TryDelete(ctx);
            await DiscordUtils.SendBotMessage(MusicBot.StringList(ctx), ctx.Message.Channel, user: ctx.User);
        }
        [Command("next"), Description("Skip the current song and go the next song in the queue"), Aliases("skip", "nextsong")]
        public async Task CMDNext(CommandContext ctx)
        {
            await TryDelete(ctx);
            MusicBot.NextSong();
        }
        [Command("continue"), Description("Continue playing if the bot crashed"), Aliases("replay")]
        public async Task CMDContinue(CommandContext ctx) {
            await TryDelete(ctx);
            await MusicBot.Play(ctx);
        }
        [Command("Remove"), Description("Shows the current queue."), Aliases("deleteSong", "Removesong")]
        public async Task CMDRemove(CommandContext ctx, [RemainingText, Description("Full youtube URL.")] int ID)
        {
            await TryDelete(ctx);
            Video vid = MusicBot.GetVideo(ID);
            bool Good = MusicBot.RemoveVideo(ID);
            if (Good && (vid != null))
            {
                await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicRemove, "~1", vid.FullName), ctx.Message.Channel, user: ctx.User);
            }
            else
            {
                await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicRemoveERROR, "~1", ID + ""), ctx.Message.Channel, user: ctx.User);
            }
        }
        [Command("howlong"), Description("See howlong the current song is."), Aliases("time", "boring")]
        public async Task CMDHowLong(CommandContext ctx)
        {
            await TryDelete(ctx);
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicProcentLeft, "~1", MusicBot.CurrentPlayingProcent()), ctx.Message.Channel, user: ctx.User);
        }
        [Command("volume"), Description("Change the Volume of a new song(1 = 100%)")]
        public async Task CMDVolume(CommandContext ctx, [RemainingText, Description("Volume. (1 = 100%)")] float V)
        {
            await TryDelete(ctx);
            MusicBot.ChangeVolume(V);
            await DiscordUtils.SendBotMessage(Messages.AudioVolumeChange, ctx.Message.Channel, user: ctx.User);
        }


        public static async Task TryDelete(CommandContext ctx) {
            try
            {
                await ctx.Message.DeleteAsync();
            }
            catch (Exception e) { Utils.Log(e.StackTrace + "\n" + e.Message, LogType.Error);  }
        }



    }
}
