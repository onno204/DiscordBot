using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using NAudio.Wave;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using VideoLibrary;

namespace Onno204Bot.Events
{
    internal class MusicBot
    {
        public static Dictionary<int, Video> Queue = new Dictionary<int, Video>();
        public static Thread ThreadID = (Thread)null;
        public static int ThreadIDD = 0;
        public static bool StopPlayingJoined = false;
        public static int UsersInChannel = 0;
        public static int TotalSendBytes = 0;
        public static int IntPlayout = 0;
        public static float Volume = 1f;

        public static bool CurrentPlaying(DUser duser)
        {
            return duser.VNCon.IsPlaying;
        }

        public static string CurrentPlayingProcent()
        {
            double totalSendBytes = (double)MusicBot.TotalSendBytes;
            double intPlayout = (double)MusicBot.IntPlayout;
            int num = (int)Math.Round(totalSendBytes / intPlayout * 100.0);
            Utils.Debug((object)("MusicBot, " + (object)totalSendBytes + "/" + (object)intPlayout + "=" + (object)num));
            return num.ToString() + "%";
        }

        public static bool AddToQueue(Video vid)
        {
            if (MusicBot.Queue.Values.Contains<Video>(vid))
                return false;
            MusicBot.Queue.Add(MusicBot.Queue.Count, vid);
            return true;
        }

        public static void ChangeVolume(float V)
        {
            MusicBot.Volume = V;
        }

        public static Video NextQueue()
        {
            MusicBot.Queue.Remove(0);
            if (!MusicBot.Queue.ContainsKey(1))
                return (Video)null;
            Video video = MusicBot.Queue[1];
            for (int key = 1; key <= MusicBot.Queue.Count + 3; ++key)
            {
                if (MusicBot.Queue.ContainsKey(key))
                {
                    try
                    {
                        MusicBot.Queue[key - 1] = MusicBot.Queue[key];
                        MusicBot.Queue.Remove(key);
                    }
                    catch (Exception ex)
                    {
                        Utils.Log(ex.StackTrace + "\n" + ex.Message, LogType.Error);
                    }
                }
                else
                    break;
            }
            return video;
        }

        public static Video FirstPlay()
        {
            return MusicBot.Queue[0];
        }

        public static bool RemoveVideo(int ID)
        {
            if (!MusicBot.Queue.ContainsKey(ID))
                return false;
            MusicBot.Queue.Remove(ID);
            for (int key = ID + 1; key <= MusicBot.Queue.Count + 3; ++key)
            {
                if (MusicBot.Queue.ContainsKey(key))
                {
                    try
                    {
                        MusicBot.Queue[key - 1] = MusicBot.Queue[key];
                        MusicBot.Queue.Remove(key);
                    }
                    catch (Exception ex)
                    {
                        Utils.Log(ex.StackTrace + "\n" + ex.Message, LogType.Error);
                    }
                }
                else
                    break;
            }
            return true;
        }

        public static void NextSong()
        {
            MusicBot.ThreadIDD = 0;
        }

        public static Video GetVideo(int ID)
        {
            if (!MusicBot.Queue.ContainsKey(ID))
                return (Video)null;
            return MusicBot.Queue[ID];
        }

        public static void ClearQueue()
        {
            MusicBot.Queue.Clear();
        }

        public static string StringList(DUser duser)
        {
            string str = "";
            for (int key = 0; key <= MusicBot.Queue.Count + 3; ++key)
            {
                try
                {
                    if (MusicBot.Queue.ContainsKey(key))
                        str = str + (object)key + ". " + MusicBot.Queue[key].Title + "\n";
                    else
                        break;
                }
                catch (Exception ex)
                {
                    Utils.Log(ex.StackTrace + "\n" + ex.Message, LogType.Error);
                }
            }
            if (str == "")
                str = "No queued Music!";
            return str;
        }

        public static string CreatePathFromVid(Video vid)
        {
            return Config.VideoDir + Utils.RemoveSpecialCharacters(vid.FullName) + ".mp3";
        }

        public static async Task Play(DUser duser, bool Next = false)
        {
            MusicBot.StopPlayingJoined = false;
            VoiceNextClient vnext = duser.VNClient;
            if (vnext == null)
            {
                await DiscordUtils.SendBotMessage("VNext is not enabled or configured.", duser);
            }
            else
            {
                VoiceNextConnection vnc = duser.VNCon;
                if (vnc == null)
                {
                    await DiscordUtils.SendBotMessage(Messages.AudioNotconnectedToServer, duser);
                    await CommandFunctions.MusicJoinCh(duser);
                    await Task.Delay(200);
                    await MusicBot.Play(duser, false);
                }
                else if (Next)
                {
                    MusicBot.ThreadID.Abort();
                    await Task.Delay(1000);
                    MusicBot.ThreadID = new Thread((ThreadStart)(() => MusicBot.StartPlay(duser, false)));
                    MusicBot.ThreadID.Start();
                }
                else if (MusicBot.CurrentPlaying(duser))
                {
                    await DiscordUtils.SendBotMessage(Messages.AudioMusicAlreadyPlaying, duser);
                }
                else
                {
                    MusicBot.ThreadID = new Thread((ThreadStart)(() => MusicBot.StartPlay(duser, false)));
                    MusicBot.ThreadID.Start();
                }
            }
        }

        private static async Task StartPlay(DUser duser, bool Next = false)
        {
            await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioStartedPlaying, "~1", duser.VoiceChannel.Name), duser);
            await duser.VNCon.SendSpeakingAsync(true);
            Video v = MusicBot.FirstPlay();
            if (Next)
                v = MusicBot.NextQueue();
            Random rd = new Random();
            int SpecialID = rd.Next(12, 123123);
            await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.AudioMusicNextSongInQueue, "~2", DUtils.GetMusicURL(v)), "~1", v.Title), duser);
            await MusicBot.VoiceStream(duser.VNCon, v, SpecialID);
            while (MusicBot.Queue.ContainsKey(1))
            {
                Utils.Debug((object)"MusicBot, Playing next song!");
                await Task.Delay(1500);
                v = MusicBot.NextQueue();
                await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.AudioMusicNextSongInQueue, "~2", DUtils.GetMusicURL(v)), "~1", v.Title), duser);
                await MusicBot.VoiceStream(duser.VNCon, v, SpecialID);
            }
            MusicBot.ClearQueue();
            await DiscordUtils.SendBotMessage(Messages.AudioMusicQueueEnded, duser);
        }

        public static async Task VoiceStream(VoiceNextConnection vnc, Video vid, int SpecialID)
        {
            MusicBot.ThreadIDD = SpecialID;
            MusicBot.UsersInChannel = DUtils.GetAmountInVoice(vnc.Channel);
            new Thread((ThreadStart)(() => vnc.VoiceReceived += (AsyncEventHandler<VoiceReceiveEventArgs>)(async e =>
            {
                if (SpecialID != MusicBot.ThreadIDD)
                    Thread.CurrentThread.Abort();
                try
                {
                    if (Config.StopPlayingIfANYsoundIsReceived)
                        MusicBot.StopPlayingJoined = true;
                    else
                        Utils.Debug((object)("Musicbot, Received sounds!!!" + e.User.Username));
                }
                catch (Exception ex)
                {
                    if (Config.StopPlayingWithNewPlayer)
                        MusicBot.StopPlayingJoined = true;
                    if (!ex.Message.Contains("De objectverwijzing is niet op een exemplaar van een object ingesteld.") || !Config.StopPlayingWithNewPlayer)
                        return;
                    MusicBot.StopPlayingJoined = true;
                }
                await Task.Delay(1);
            }))).Start();
            Exception exc = (Exception)null;
            string filename = MusicBot.CreatePathFromVid(MusicBot.FirstPlay());
            if (!File.Exists(filename))
            {
                Utils.Log(string.Format("File `{0}` does not exist.", (object)filename), LogType.Error);
            }
            else
            {
                Exception obj = null;
                int num = 0;
                try
                {
                    try
                    {
                        int SampleRate = 48000;
                        int channelCount = 2;
                        WaveFormat OutFormat = new WaveFormat(SampleRate, 16, channelCount);
                        MediaFoundationResampler resampler;
                        WaveStream mediaStream;
                        try
                        {
                            mediaStream = (WaveStream)new WaveChannel32((WaveStream)new MediaFoundationReader(filename), MusicBot.Volume, 0.0f);
                            WaveStream waveStream = mediaStream;
                            try
                            {
                                MediaFoundationResampler foundationResampler = resampler = new MediaFoundationResampler((IWaveProvider)mediaStream, OutFormat);
                                try
                                {
                                    int m = int.Parse(string.Concat((object)mediaStream.Length));
                                    MusicBot.IntPlayout = m / 2 + m / 35;
                                    MusicBot.TotalSendBytes = 0;
                                    resampler.ResamplerQuality = 60;
                                    int blockSize = OutFormat.AverageBytesPerSecond / 50;
                                    byte[] buffer = new byte[blockSize];
                                    do
                                    {
                                        int byteCount;
                                        if ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0)
                                        {
                                            while (MusicBot.StopPlayingJoined)
                                                Thread.Sleep(100);
                                            if (byteCount < blockSize)
                                            {
                                                for (int i = byteCount; i < blockSize; ++i)
                                                    buffer[i] = (byte)0;
                                            }
                                            MusicBot.TotalSendBytes += buffer.Length;
                                            Utils.Debug((object)("MusicBot, " + (object)MusicBot.TotalSendBytes + "/" + (object)MusicBot.IntPlayout));
                                            if (SpecialID == MusicBot.ThreadIDD)
                                                await MusicBot.SendVoiceData(buffer, 20, vnc);
                                            else
                                                goto label_31;
                                        }
                                        else
                                            goto label_18;
                                    }
                                    while (MusicBot.IntPlayout > MusicBot.TotalSendBytes);
                                    Utils.Debug((object)"MusicBot, Finished playing?");
                                    label_18:
                                    buffer = (byte[])null;
                                }
                                finally
                                {
                                    if (foundationResampler != null)
                                        foundationResampler.Dispose();
                                }
                                foundationResampler = (MediaFoundationResampler)null;
                            }
                            finally
                            {
                                if (waveStream != null)
                                    waveStream.Dispose();
                            }
                            waveStream = (WaveStream)null;
                        }
                        catch (Exception ex)
                        {
                            Utils.Log(ex.StackTrace + "\n" + ex.Message, LogType.Error);
                        }
                        resampler = (MediaFoundationResampler)null;
                        mediaStream = (WaveStream)null;
                        OutFormat = (WaveFormat)null;
                        goto label_33;
                    }
                    catch (Exception ex)
                    {
                        exc = ex;
                        goto label_33;
                    }
                    label_31:
                    num = 1;
                }
                catch (Exception ex)
                {
                    obj = ex;
                }
                label_33:
                await vnc.SendSpeakingAsync(false);
                Exception obj1 = obj;
                if (obj1 != null)
                {
                    Exception source = obj1 as Exception;
                    if (source == null)
                        throw obj1;
                    ExceptionDispatchInfo.Capture(source).Throw();
                }
                if (num == 1)
                    return;
                obj = (Exception)null;
                if (exc == null)
                    return;
                Utils.Log(string.Format("An exception occured during playback: `{0}: {1}`", (object)exc.GetType(), (object)exc.Message), LogType.Error);
            }
        }

        public static async Task SendVoiceData(byte[] bytes, int size, VoiceNextConnection vnc)
        {
            if (MusicBot.UsersInChannel != DUtils.GetAmountInVoice(vnc.Channel) && Config.StopPlayingWithNewPlayer)
            {
                MusicBot.UsersInChannel = DUtils.GetAmountInVoice(vnc.Channel);
                MusicBot.StopPlayingJoined = true;
            }
            if (MusicBot.StopPlayingJoined)
                return;
            await vnc.SendAsync(bytes, size, 16);
        }
    }
}
