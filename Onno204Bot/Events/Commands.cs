// Decompiled with JetBrains decompiler
// Type: Onno204Bot.Events.Commands
// Assembly: Onno204Bot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 07514260-55DC-4904-AABF-A0148194411D
// Assembly location: A:\Projects\VisualStudio\Discord\bin\Debug\Onno204Bot.exe

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Onno204Bot.Cfg;
using Onno204Bot.Lib;
using Onno204Bot.Remote;
using System;
using System.Threading.Tasks;

namespace Onno204Bot.Events
{
    internal class Commands
    {
        [Command("hi")]
        public async Task Hi(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.Hi(ctx);
        }

        [Command("echo")]
        [Description("Echo's whatever u say.")]
        [Aliases(new string[] { "repeat" })]
        public async Task CMDEcho(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.Echo(new DUser(ctx, true, 0UL)
            {
                Args = ctx.RawArgumentString
            });
        }

        [Command("ping")]
        [Description("Internet Check.")]
        [Aliases(new string[] { "pong", "pingpong" })]
        public async Task CMDPing(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            DUser duser = new DUser(ctx, true, 0UL);
            await CommandFunctions.Ping(duser);
            await DiscordUtils.SendBotMessage("Pong2!", new DUser(ctx, true, 0UL));
        }

        [Command("purge")]
        [Description("Remove X amount of messages.")]
        [Aliases(new string[] { "delete" })]
        public async Task CMDPurge(CommandContext ctx, [Description("Amount of messages to purge")] int amount)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.Purge(new DUser(ctx, true, 0UL)
            {
                Args = amount.ToString()
            });
        }

        [Command("join")]
        [Description("Makes me join a voice channel.")]
        [Aliases(new string[] { "add" })]
        public async Task Join(CommandContext ctx, DiscordChannel chn = null)
        {
            await Commands.TryDelete(ctx);
            Utils.Debug((object)"Join, Voicechannel Joining!");
            await CommandFunctions.MusicJoinCh(new DUser(ctx, true, 0UL));
        }

        [Command("leave")]
        [Description("Makes me leave a voice channel.")]
        [Aliases(new string[] { "exit", "quit" })]
        public async Task Leave(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicLeave(new DUser(ctx, true, 0UL));
        }

        [Command("play")]
        [Description("Plays an youtube Video(Only youtube links, No search!).")]
        [Aliases(new string[] { "song", "music" })]
        public async Task Play(CommandContext ctx, [RemainingText, Description("Full youtube URL.")] string YoutubeURL)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicPlay(new DUser(ctx, true, 0UL) {
                Args = YoutubeURL
            });
        }

        [Command("test2")]
        [Description("TestCommand!")]
        public async Task CMDTest(CommandContext ctx)
        {
            DUser duser = new DUser(ctx, true, 0UL);
            await DiscordUtils.SendBotMessage(ctx.Member.VoiceState.ToString() + "hey", duser);
            await Onno204Bot.Remote.Remote.Start();
        }

        [Command("remote")]
        [Description("Start remote Control!")]
        [Aliases(new string[] { "webcommands", "startremote" })]
        public async Task CMDRemote(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await Onno204Bot.Remote.Remote.Start();
        }

        [Command("remoteupdate")]
        [Description("Update Remote info!")]
        public async Task CMDUpdateRemote(CommandContext ctx)
        {
            if (GetSetItems.LastDUser == null)
            {
                DUser duser = new DUser(ctx, true, 0UL);
            }
            await Commands.TryDelete(ctx);
            await RemoteUpdateInfo.Update();
        }

        [Command("queue")]
        [Description("Shows the current queue.")]
        [Aliases(new string[] { "songs", "playlist" })]
        public async Task CMDqueue(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicQueue(new DUser(ctx, true, 0UL));
        }

        [Command("next")]
        [Description("Skip the current song and go the next song in the queue")]
        [Aliases(new string[] { "skip", "nextsong" })]
        public async Task CMDNext(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicSkip(new DUser(ctx, true, 0UL));
        }

        [Command("continue")]
        [Description("Continue playing if the bot crashed")]
        [Aliases(new string[] { "replay" })]
        public async Task CMDContinue(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicContinueCrash(new DUser(ctx, true, 0UL));
        }

        [Command("itsoke")]
        [Description("Continue playing if the bot paused because someone joined")]
        public async Task CMDitsoke(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicContinueAfterJoin(new DUser(ctx, true, 0UL));
        }

        [Command("Remove")]
        [Description("Shows the current queue.")]
        [Aliases(new string[] { "deleteSong", "Removesong" })]
        public async Task CMDRemove(CommandContext ctx, [RemainingText, Description("Full youtube URL.")] int ID)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicRemoveSong(new DUser(ctx, true, 0UL)
            {
                Args = ID.ToString()
            });
        }

        [Command("howlong")]
        [Description("See howlong the current song is.")]
        [Aliases(new string[] { "time", "boring" })]
        public async Task CMDHowLong(CommandContext ctx)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicProcentLeft(new DUser(ctx, true, 0UL));
        }

        [Command("volume")]
        [Description("Change the Volume of a new song(1 = 100%)")]
        public async Task CMDVolume(CommandContext ctx, [RemainingText, Description("Volume. (1 = 100%)")] float V)
        {
            await Commands.TryDelete(ctx);
            await CommandFunctions.MusicContinueAfterJoin(new DUser(ctx, true, 0UL)
            {
                Args = V.ToString()
            });
        }

        public static async Task TryDelete(CommandContext ctx)
        {
            try
            {
                await ctx.Message.DeleteAsync((string)null);
            }
            catch (Exception ex)
            {
                Utils.Log(ex.StackTrace + "\n" + ex.Message, LogType.Error);
            }
        }
    }
}
