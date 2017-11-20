using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using NAudio.Wave;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoLibrary;

namespace Onno204Bot.Events
{
    class MusicBot
    {

        public static Dictionary<int, Video> Queue = new Dictionary<int, Video>();
        public static Thread ThreadID = null;
        public static int ThreadIDD = 0;

        //Current song stats
        public static int TotalSendBytes = 0; //Self Addes
        public static int IntPlayout = 0; //Self Addes
        public static float Volume = 1F;

        public static bool CurrentPlaying(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();
            var vnc = vnext.GetConnection(ctx.Guild);
            return vnc.IsPlaying;
        }
        public static String CurrentPlayingProcent() {
            double deel = TotalSendBytes;
            double geheel = IntPlayout;
            int tot = (int)Math.Round((deel / geheel) * 100);
            Console.WriteLine(deel + "/" + geheel + "=" + tot);
            return tot + "%";
        }
        public static bool AddToQueue(Video vid)
        {
            if (Queue.Values.Contains(vid)) { return false; }
            Queue.Add(Queue.Count, vid);
            return true;
        }
        public static void ChangeVolume(float V)
        {
            Volume = V;
        }
        public static Video NextQueue() {
            Queue.Remove(0);
            if (!Queue.ContainsKey(1)) { return null; }
            Video RVid = Queue[1];
            for (int i = 1; i <= (Queue.Count+ 3); i++) {
                if (!Queue.ContainsKey(i)) { break; }
                try {
                    Queue[i - 1] = Queue[i];
                    Queue.Remove(i);
                }catch (Exception ee) { Utils.Log(ee.StackTrace + "\n" + ee.Message, LogType.Error); }
            }
            return RVid;
        }
        public static Video FirstPlay() {
            return Queue[0];
        }
        public static bool RemoveVideo(int ID) {
            if (!Queue.ContainsKey(ID)) { return false; }
            Queue.Remove(ID);
            for (int i = (ID+1); i <= (Queue.Count + 3); i++)
            {
                if (!Queue.ContainsKey(i)) { break; }
                try
                {
                    Queue[i - 1] = Queue[i];
                    Queue.Remove(i);
                }
                catch (Exception ee) { Utils.Log(ee.StackTrace + "\n" + ee.Message, LogType.Error); }
            }
            return true;
        }
        public static void NextSong() { ThreadIDD = 0; }
        public static Video GetVideo(int ID)
        {
            if (!Queue.ContainsKey(ID)) { return null; }
            return Queue[ID];
        }
        public static void ClearQueue() {
            Queue.Clear();
        }
        public static String StringList(CommandContext ctx)
        {
            String EndS = "";
            for (int i = 0; i <= (Queue.Count + 3); i++) {
                try {
                    if (!Queue.ContainsKey(i)) { break; }
                    EndS = EndS + i + ". " + Queue[i].Title + "\n";
                }
                catch (Exception ee) { Utils.Log(ee.StackTrace + "\n" + ee.Message, LogType.Error); }
            }
            if (EndS == "") { EndS = "No queued Music!"; }
            return EndS;
        }
        public static String CreatePathFromVid(Video vid) {
            return @Directory.GetCurrentDirectory() + "\\Bot\\" + vid.Title + ".mp3";
        }
        public static async Task Play(CommandContext ctx, bool Next = false) {

            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNextClient();
            if (vnext == null)
            {
                // not enabled
                await ctx.RespondAsync("VNext is not enabled or configured.");
                return;
            }

            // check whether we aren't already connected
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                // already connected
                await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioNotconnectedToServer, "~1", ctx.RawArgumentString), ctx.Message.Channel, user: ctx.User);
                await Commands.JoinCh(ctx);
                await Play(ctx);
                return;
                //await ctx.RespondAsync("Not connected in this guild.");
                //return;
            }
            if (Next) {
                ThreadID.Abort();
                await Task.Delay(1000);
                ThreadID = new Thread(() => { StartPlay(ctx, vnc); });
                ThreadID.Start();
                return;
            }
            if (CurrentPlaying(ctx)) {
                await DiscordUtils.SendBotMessage(Messages.AudioMusicAlreadyPlaying, ctx.Message.Channel, user: ctx.User);
                return;
            }
            ThreadID = new Thread(() => { StartPlay(ctx, vnc); }) ;
            ThreadID.Start();
        }
        private static async Task StartPlay(CommandContext ctx, VoiceNextConnection vnc, bool Next = false) {
            
            // play
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioStartedPlaying, "~1", vnc.Channel.Name), ctx.Message.Channel, user: ctx.User);
            await vnc.SendSpeakingAsync(true);
            Video v = FirstPlay();
            if (Next == true) { v = NextQueue(); }

            Random rd = new Random();
            int SpecialID = rd.Next(12, 123123);
            await VoiceStream(vnc, v, SpecialID);
            while (Queue.ContainsKey(1)) {
                Console.WriteLine("Playing next song!");
                await Task.Delay(1500);
                await VoiceStream(vnc, NextQueue(), SpecialID);
            }
            ClearQueue();
            await DiscordUtils.SendBotMessage(Messages.AudioMusicQueueEnded, ctx.Message.Channel, user: ctx.User);


        }
        public static async Task VoiceStream(VoiceNextConnection vnc, Video vid, int SpecialID)
        {
            ThreadIDD = SpecialID;
            Exception exc = null;
            String filename = CreatePathFromVid(FirstPlay());
            // check if file exists
            if (!File.Exists(filename))
            {
                // file does not exist
                Utils.Log($"File `{filename}` does not exist.", LogType.Error);
                //await ctx.RespondAsync($"File `{filename}` does not exist.");
                return;
            }
            try
            {
                MediaFoundationResampler resampler;
                WaveStream mediaStream;
                int SampleRate = 48000;
                //float Volume = 1F;
                int channelCount = 2; // Get the number of AudioChannels our AudioService has been configured to use.
                WaveFormat OutFormat = new WaveFormat(SampleRate, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
                try
                {
                    mediaStream = new WaveChannel32(new MediaFoundationReader(filename), Volume, 0F);
                    using (mediaStream)
                    using (resampler = new MediaFoundationResampler(mediaStream, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
                    {
                        int m = Int32.Parse(mediaStream.Length + "");
                        IntPlayout = (m/2) + (m/35);
                        TotalSendBytes = 0;
                        resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                        int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                        byte[] buffer = new byte[blockSize];
                        int byteCount;
                        while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) { // Read audio into our buffer, and keep a loop open while data is present
                            if (byteCount < blockSize) {
                                // Incomplete Frame
                                for (int i = byteCount; i < blockSize; i++)
                                    buffer[i] = 0;
                            }
                            TotalSendBytes += buffer.Length;
                            if (SpecialID != ThreadIDD) { return; }
                            await vnc.SendAsync(buffer, 20); // we're sending 20ms of data
                            if (IntPlayout <= TotalSendBytes)
                            {
                                Console.WriteLine("I AM B REAKING UP MY BONES AGAIN, YOU MADE MY SYSTEM BROKE!");
                                break;
                            }
                        }
                    }
                } catch (Exception ee) { Utils.Log(ee.StackTrace + "\n" + ee.Message, LogType.Error); }
            }
            catch (Exception ex) { exc = ex; }
            finally
            {
                await vnc.SendSpeakingAsync(false);
            }
            if (exc != null)
                Utils.Log($"An exception occured during playback: `{exc.GetType()}: {exc.Message}`", LogType.Error);
                //await ctx.RespondAsync($"An exception occured during playback: `{exc.GetType()}: {exc.Message}`");
        }





    }
}
