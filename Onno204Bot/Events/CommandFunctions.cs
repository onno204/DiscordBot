using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using MediaToolkit;
using MediaToolkit.Model;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace Onno204Bot.Events
{
    class CommandFunctions
    {

        public static async Task Hi(CommandContext ctx)
        {
            await ctx.RespondAsync($"👋 Hi, {ctx.User.Mention}!");
        }

        /// <summary>
        /// Echo something
        /// </summary>
        /// <param name="duser">Args = Message</param>
        /// <returns></returns>
        public static async Task Echo(DUser duser)
        {
            await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.EchoReply, "~2", duser.Args), "~1", duser.Member.Mention), duser);
        }

        /// <summary>
        /// Ping - Pong Test
        /// </summary>
        /// <param name="duser"></param>
        /// <returns></returns>
        public static async Task Ping(DUser duser)
        {
            await DiscordUtils.SendBotMessage("Pong!", duser);
        }
        
        /// <summary>
        /// Purge an x Amount of messages
        /// </summary>
        /// <param name="duser">Args = Amount to remove</param>
        /// <returns></returns>
        public static async Task Purge(DUser duser)
        {
            int Amount = 0; //Just to set an int
            if (!Int32.TryParse(duser.Args, out Amount)) { //Try parse. If error say it
                await DiscordUtils.SendBotMessage(Messages.StringToNumberError, duser);
                return;
            }

            IReadOnlyList<DiscordMessage> List = await duser.TextChannel.GetMessagesAsync(Amount); //Create list with messages
            ulong ChannelID = duser.TextChannel.Id;
            foreach (DiscordMessage DM in List) //Foreach message in list, Delete them
            {
                if (DM.ChannelId == ChannelID)
                {
                    await DM.DeleteAsync();
                }
            }

            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.PurgeMessage, "~1", Amount.ToString()), duser); //Send message everything went well
        }

        /// <summary>
        /// Make the bot join a voice channel :)
        /// </summary>
        /// <param name="duser">Normal Duser from CTX, Make sure Voicechannel is supplied!</param>
        /// <returns></returns>
        public static async Task MusicJoinCh(DUser duser)
        {
            Utils.Debug("Join, " + "Voicechannel: " + duser.VoiceChannel.Name);
            // check whether VNext is enabled
            var vnext = duser.VNClient;
            if (vnext == null)
            {
                // not enabled
                await DiscordUtils.SendBotMessage("VNext is not enabled or configured, contact the admin", duser);
                return;
            }
            // check whether we aren't already connected
            var vnc = duser.VNCon;
            if (vnc != null)
            {
                // already connected
                await DiscordUtils.SendBotMessage(Messages.AudioAlreadyInguild, duser);
                return;
            }
            // connect
            vnc = await vnext.ConnectAsync(duser.VoiceChannel);
            duser.VNCon = vnc;
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioConnectedTo, "~1", duser.VoiceChannel.Name), duser);
        }
        
        /// <summary>
        /// Make the musicbot leave the channel
        /// </summary>
        /// <param name="duser">Args = Amount to remove</param>
        /// <returns></returns>
        public static async Task MusicLeave(DUser duser)
        {
            // check whether VNext is enabled
            var vnext = duser.BuildinVoiceNextClient;
            if (vnext == null)
            {
                // not enabled
                await DiscordUtils.SendBotMessage("VNext is not enabled or configured. Contact Botmaker for help!", duser);
                return;
            }

            // check whether we are connected
            VoiceNextConnection vnc = Program.Voice.GetConnection(duser.Guild);
            Utils.Debug("Guild, " + duser.Guild.Name);
            if (vnc == null)
            {
                // not connected
                await DiscordUtils.SendBotMessage(Messages.AudioNotConnected, duser);
                return;
            }

            // disconnect
            vnc.Disconnect();
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioDisconnected, "~1", vnc.Channel.Name), duser);
        }

        /// <summary>
        /// Start playing music
        /// </summary>
        /// <param name="duser">Args = YoutubeURL</param>
        /// <returns></returns>
        public static async Task MusicPlay(DUser duser) {
            String YoutubeURL = duser.Args;
            if (!YoutubeURL.ToLower().StartsWith("https://")) { YoutubeURL = "https://" + YoutubeURL; }
            string filename = "";
            Video vid = null;
            try
            {
                YouTube youtube = YouTube.Default;
                vid = youtube.GetVideo(YoutubeURL);
                if (!Directory.Exists(Config.VideoDir)) { Directory.CreateDirectory(Config.VideoDir); }

                filename = MusicBot.CreatePathFromVid(vid);
                if (!File.Exists(filename))
                {
                    await DiscordUtils.SendBotMessage(Messages.AudioDownloading, duser);
                    string filenameNoMP3 = @Config.VideoDir+ Utils.RemoveSpecialCharacters(vid.FullName) + ".Temp";
                    Utils.Debug("Vid UrL: " + YoutubeURL);
                    Utils.Debug("Vid Uri: " + await vid.GetUriAsync());
                    File.WriteAllBytes(filenameNoMP3, vid.GetBytes());
                    var inputFile = new MediaFile { Filename = filenameNoMP3 };
                    var outputFile = new MediaFile { Filename = filename };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);
                        engine.Convert(inputFile, outputFile);
                    }
                    File.Delete(filenameNoMP3);
                    DUtils.SetMusicURL(vid, YoutubeURL);
                }
            }
            catch (Exception e)
            {
                Utils.Log(e.Message + " ----- " + e.StackTrace, LogType.Error);
                await DiscordUtils.SendBotMessage("Error: " + e.Message, duser);
                await Task.Delay(1);
                return;
            }
            if (vid != null)
                MusicBot.AddToQueue(vid);
            await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.AudioMusicAddedToQueue, "~2", YoutubeURL), "~1", vid.Title), duser);
            await MusicBot.Play(duser);
        }

        /// <summary>
        /// Simply gets the queue for the current server/guild
        /// </summary>
        /// <param name="duser"></param>
        /// <returns></returns>
        public static async Task MusicQueue(DUser duser)
        {
            await DiscordUtils.SendBotMessage(MusicBot.StringList(duser), duser);
        }

        /// <summary>
        /// Simply Skips to the next song in the Queue
        /// </summary>
        /// <param name="duser"></param>
        /// <returns></returns>
        public static async Task MusicSkip(DUser duser)
        {
            MusicBot.NextSong();
            await Task.Delay(1);
        }
        
        /// <summary>
        /// Simply gets the queue for the current server/guild
        /// </summary>
        /// <param name="duser"></param>
        /// <returns></returns>
        public static async Task MusicContinueCrash(DUser duser)
        {
            await MusicBot.Play(duser);
        }

        /// <summary>
        /// Continues after someone joined if this option is enabled
        /// </summary>
        /// <param name="duser"></param>
        /// <returns></returns>
        public static async Task MusicContinueAfterJoin(DUser duser)
        {
            MusicBot.StopPlayingJoined = false;
            await Task.Delay(1);
        }

        /// <summary>
        /// Removes a song from the music Queue
        /// </summary>
        /// <param name="duser">Args = Queue ID</param>
        /// <returns></returns>
        public static async Task MusicRemoveSong(DUser duser)
        {

            int ID = 0; //Just to set an int
            if (!Int32.TryParse(duser.Args, out ID))
            { //Try parse. If error say it
                await DiscordUtils.SendBotMessage(Messages.StringToNumberError, duser);
                return;
            }
            Video vid = MusicBot.GetVideo(ID);
            bool Good = MusicBot.RemoveVideo(ID);
            if (Good && (vid != null))
            {
                await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicRemove, "~1", vid.FullName), duser);
            }
            else
            {
                await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicRemoveERROR, "~1", ID + ""), duser);
            }
        }

        /// <summary>
        /// Simply gets the queue for the current server/guild
        /// </summary>
        /// <param name="duser"></param>
        /// <returns></returns>
        public static async Task MusicProcentLeft(DUser duser)
        {
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicProcentLeft, "~1", MusicBot.CurrentPlayingProcent()), duser);
        }

        /// <summary>
        /// Simply gets the queue for the current server/guild
        /// </summary>
        /// <param name="duser"> Args = Volume ( 1 = 100%, 0.1 = 10%, 2 = 200%)</param>
        /// <returns></returns>
        public static async Task MusicChangeVolume(DUser duser)
        {
            float V = 0; //Just to set an int
            if (!float.TryParse(duser.Args, out V))
            { //Try parse. If error say it
                await DiscordUtils.SendBotMessage(Messages.StringToNumberError, duser);
                return;
            }
            MusicBot.ChangeVolume(V);
            await DiscordUtils.SendBotMessage(Messages.AudioVolumeChange, duser);
        }

        public static async Task UpdateRemoteInfo(DUser duser) {
            await Remote.RemoteUpdateInfo.Update();
            await Task.Delay(1);
        }


    }
}
