using DiscordSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using DiscordSharp.Events;

namespace Onno204Bot
{
    class Program
    {
        #region Variables
        public static DiscordClient client = new DiscordClient();
        #endregion

        static void Main(string[] args)
        {
            Start();
        }


        #region Client Functions

        public static void SendBotMessage(String message, DiscordSharp.Objects.DiscordChannel channel) {
            if (Config.BlaclistedServers.Contains(channel.Parent.Name)) { return; } //Check if server is blacklisted
            if (message == "") { return; } //Check if message = Empty(Rule to not send message)
            bool Special = false;
            bool Title = false;

            if (message.StartsWith("!")) { message = Utils.ReplaceFirstOccurrence(message, "!", ""); Special = true; } //If message startswith ! Set special
            if (message.StartsWith("@")) { message = Utils.ReplaceFirstOccurrence(message, "@", ""); Title = true; }

            //Construct message
            String MsgTitle = Messages.BotTitle + "\n";
            String MsgPrefix = "```css\n";
            String Msg = message;
            String MsgSuffix = "\n```";
            if (!Special) { MsgPrefix = MsgSuffix = ""; }
            if (!Title) { MsgTitle = ""; }
            String EndMsg = MsgTitle + MsgPrefix + Msg + MsgSuffix;
            Log(EndMsg, LogType.SendedMessages);
            client.SendMessageToChannel(EndMsg, channel);
        }

        #region Command Handling
        /// <summary>
        /// The function to handle received chat messages
        /// </summary>
        /// <param name="Message">The received message</param>
        public static void HandleCommands(String Message, DiscordSharp.Events.DiscordMessageEventArgs e) {
            if (!Message.StartsWith(Config.CommandString)) { return; }
            Message = Utils.ReplaceFirstOccurrence(Message, Config.CommandString, "");
            String Command = Utils.GetWordAt(Message, 1).ToLower();
            String Msg = Utils.ReplaceFirstOccurrence(Message, Command, "");
            switch (Command)
            {
                case "help":
                    CMDHelp(e, Msg);
                    break;
                case "echo":
                    CMDEcho(e, Msg);
                    break;
                case "play":
                    PlayMusic(e, Msg);
                    break;
                default:
                    CommandNotfound(e, Command, Msg);
                    break;
            }
        }
        public static void CommandNotfound(DiscordSharp.Events.DiscordMessageEventArgs e, String Command, String message)
        {
            SendBotMessage(Utils.Replace(Messages.CommandNotFound, "~1", Command), e.Message.Channel());
        }
        public static void CMDHelp(DiscordSharp.Events.DiscordMessageEventArgs e, String message)
        {
            SendBotMessage(Utils.Replace(Messages.HelpReply, "~1", MentoinUser(e.Message.Author)), e.Message.Channel());
            client.DeleteMessage(e.Message.ID);
        }
        public static void CMDEcho(DiscordSharp.Events.DiscordMessageEventArgs e, String message)
        {
            SendBotMessage(Utils.Replace(Utils.Replace(Messages.EchoReply, "~2", message), "~1", MentoinUser(e.Message.Author)), e.Message.Channel());
            client.DeleteMessage(e.Message.ID);
        }
        public static void PlayMusic(DiscordSharp.Events.DiscordMessageEventArgs e, String message)
        {
            TestRheaStream(e);
            client.DeleteMessage(e.Message.ID);
        }

        #region Audio playback
        WaveFormat waveFormat;
        BufferedWaveProvider bufferedWaveProvider;
        WaveCallbackInfo waveCallbackInfo;
        IWavePlayer outputDevice;
        VolumeWaveProvider16 volumeProvider;
        System.Timers.Timer stutterReducingTimer;
        public static void TestRheaStream(DiscordMessageEventArgs e)
        {
            DiscordVoiceClient vc = client.GetVoiceClient();
            if (vc== null) { SendBotMessage("!@Not in a channel!", e.Channel); }
            try
            {
                int ms = 20;//buffer
                int channels = 1;
                int sampleRate = 48000;

                int blockSize = 48 * 2 * channels * ms; //sample rate * 2 * channels * milliseconds
                byte[] buffer = new byte[blockSize];
                var outFormat = new WaveFormat(sampleRate, 16, channels);
                vc.SetSpeaking(true);
                using (var mp3Reader = new MediaFoundationReader("https://www.youtube.com/watch?v=2Vv-BfVoq4g"))
                {
                    using (var resampler = new MediaFoundationResampler(mp3Reader, outFormat) { ResamplerQuality = 60 })
                    {
                        int byteCount;
                        while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0)
                        {
                            if (vc.Connected)
                            {
                                vc.SendVoice(buffer);
                            }
                            else
                                break;
                        }
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Voice finished enqueuing");
                        Console.ForegroundColor = ConsoleColor.White;
                        resampler.Dispose();
                        mp3Reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Exception during voice: `" + ex.Message + "`\n\n```" + ex.StackTrace + "\n```", LogType.Console);
            }
            #endregion
        }

        #endregion

        public static string MentoinUser(DiscordSharp.Objects.DiscordMember Auth) {
            return "@"+Auth.Username+"#"+Auth.Discriminator;
        }

        //All the Message functions
        public static Task SetupEvents(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.White;
            try
            {
                return Task.Run(() =>
                {

                    //Instant reaction on a Mention
                    client.MentionReceived += (sender, e) =>
                    {
                        SendBotMessage( Utils.Replace( Utils.Replace(Messages.MentionReply, "~2", e.Message.Content), "~1", MentoinUser(e.Message.Author) ), e.Message.Channel());
                    };


                    //Message Delete
                    client.MessageReceived += (sender, e) =>
                    {
                        HandleCommands(e.MessageText, e);
                    };

                    //Message Delete
                    client.MessageDeleted += (sender, e) =>
                    {
                        try
                        {
                            String Message = e.DeletedMessage.Author.Nickname + ": " + e.DeletedMessage.Content + "|| AT ||" + e.DeletedMessage.timestamp;
                            Log(Message, LogType.DeletedMessages);
                            //client.SendMessageToUser(Message, DiscordSharp.Objects.DiscordMember())
                        }
                        catch (Exception ee) {
                            Log("Error, " + ee.Message, LogType.Error);
                        }
                    };
                    
                    //Task to do when connected/Loggedin
                    client.Connected += (sender, e) =>
                    {
                        client.UpdateCurrentGame("Getting written by @onno204", true, "http://prio1gaming.nl.eu.org/");
                    };

                    //Closed Event
                    client.SocketClosed += (sender, e) =>
                    {
                        Console.Title = "Onno204 - Discord - Socket Closed..";
                        if (!Config.Exiting)
                        {
                            WriteError($"\n\nSocket Closed Unexpectedly! Code: {e.Code}. Reason: {e.Reason}. Clear: {e.WasClean}.\n\n");
                            Console.WriteLine("Waiting 6 seconds to reconnect..");
                            Thread.Sleep(6 * 1000);

                            LetsGoAgain();
                        }
                        else
                        {
                            Console.WriteLine($"Shutting down ({e.Code}, {e.Reason}, {e.WasClean})");
                        }
                    };


                }, token);
            }
            catch { return SetupEvents(token); }
        }

        //Restart  
        public static void LetsGoAgain()
        {
            client.Dispose();
            client = null;

            client = new DiscordClient();
            client.RequestAllUsersOnStartup = true;
            client.ClientPrivateInformation.Email = Config.Email;
            client.ClientPrivateInformation.Password = Config.Password;

            SetupEvents(Config.cancelToken);
        }

        #endregion

        #region Logging
        /// <summary>
        /// LogType's at the bottom
        /// </summary>
        /// <param name="Text">Text to write</param>
        /// <param name="LT">Log Type</param>
        public static void Log(String Text, LogType LT) {

            Text = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt") + "-> " + Text;
            if (LT == LogType.Chat)
            {
                Log("Chat ||| " + Text, LogType.Console);
                WriteToFile("ChatLog.txt", Text);
            }
            else if (LT == LogType.Error)
            {
                Log("ERROR Log ||| " + Text, LogType.Console);
                WriteToFile("ErrorLog.txt", Text);
            }
            else if (LT == LogType.SendedMessages)
            {
                Log("SendMessage ||| " + Text, LogType.Console);
                WriteToFile("ErrorLog.txt", Text);
            }
            else if (LT == LogType.DeletedMessages)
            {
                Log("Deleted Messages ||| " + Text, LogType.Console);
                WriteToFile("ErrorLog.txt", Text);
            }
            else if (LT == LogType.Log)
            {
                Log("Unknow Log ||| " + Text, LogType.Console);
                WriteToFile("DerpLog.txt", Text);
            }
            else if (LT == LogType.Console)
            {
                Console.WriteLine(Text);
            }
            else
            {
                Log("Not specified Logg ||| " + Text, LogType.Console);
                WriteToFile("Log.txt", Text);
            }
        }

        public static void WriteToFile(String path, String Text) {
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(Text);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(Text);
            }
        }

        public static void ReadFile(String path) {
            // Open the file to read from.
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
        }

        #endregion
        
        #region Login & Start & error
        static void Start()
        {
            try
            {
                Console.WriteLine("Started Login!");
                Login();
            }
            catch (Exception e)
            {
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine(e.Message);
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                new Thread(() => ConsoleBeep()).Start();
            }

            //Console commands Menu
            while(true){
                string crl = Console.ReadLine();
                string Cmd = crl.Split(' ')[0];
                crl = Utils.ReplaceFirstOccurrence(crl, Cmd, "");
                switch (Cmd)
                {
                    case "help":
                        Log("Try typing '1231'!", LogType.Console);
                        Log("Try typing 'hel123p'!", LogType.Console);
                        break;
                    case "join":
                        string substring = crl.Substring(crl.LastIndexOf('/') + 1);
                        client.AcceptInvite(substring);
                        Log("Joined, "+ substring, LogType.Misc);
                        break;
                    default:
                        Log("Try typing 'help'!", LogType.Console);
                        break;
                }
            }

        }

        public static void WriteError(string text)
        {
            Console.Beep(658, 125);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Error: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text + "\n");
        }

        public static void WriteWarning(string text)
        {
            Console.Beep(658, 125);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Warning: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text + "\n");
        }



        static void ConsoleBeep()
        {
            while (true) {
                Console.Beep(658, 125);
                Console.Beep(1320, 500);
                Console.Beep(990, 250);
                Console.Beep(1056, 250);
                Console.Beep(1188, 250);
                Console.Beep(1320, 125);
                Console.Beep(1188, 125);
                Console.Beep(1056, 250);
                Console.Beep(990, 250);
                Console.Beep(880, 500);
                Console.Beep(880, 250);
                Console.Beep(1056, 250);
                Console.Beep(1320, 500);
                Console.Beep(1188, 250);
                Console.Beep(1056, 250);
                Console.Beep(990, 750);
                Console.Beep(1056, 250);
                Console.Beep(1188, 500);
                Console.Beep(1320, 500);
                Console.Beep(1056, 500);
                Console.Beep(880, 500);
                Console.Beep(880, 500);
                Thread.Sleep(250);
                Console.Beep(1188, 500);
                Console.Beep(1408, 250);
                Console.Beep(1760, 500);
                Console.Beep(1584, 250);
                Console.Beep(1408, 250);
                Console.Beep(1320, 750);
                Console.Beep(1056, 250);
                Console.Beep(1320, 500);
                Console.Beep(1188, 250);
                Console.Beep(1056, 250);
                Console.Beep(990, 500);
                Console.Beep(990, 250);
                Console.Beep(1056, 250);
                Console.Beep(1188, 500);
                Console.Beep(1320, 500);
                Console.Beep(1056, 500);
                Console.Beep(880, 500);
                Console.Beep(880, 500);
                Thread.Sleep(500);
                Console.Beep(1320, 500);
                Console.Beep(990, 250);
                Console.Beep(1056, 250);
                Console.Beep(1188, 250);
                Console.Beep(1320, 125);
                Console.Beep(1188, 125);
                Console.Beep(1056, 250);
                Console.Beep(990, 250);
                Console.Beep(880, 500);
                Console.Beep(880, 250);
                Console.Beep(1056, 250);
                Console.Beep(1320, 500);
                Console.Beep(1188, 250);
                Console.Beep(1056, 250);
                Console.Beep(990, 750);
                Console.Beep(1056, 250);
                Console.Beep(1188, 500);
                Console.Beep(1320, 500);
                Console.Beep(1056, 500);
                Console.Beep(880, 500);
                Console.Beep(880, 500);
                Thread.Sleep(250);
                Console.Beep(1188, 500);
                Console.Beep(1408, 250);
                Console.Beep(1760, 500);
                Console.Beep(1584, 250);
                Console.Beep(1408, 250);
                Console.Beep(1320, 750);
                Console.Beep(1056, 250);
                Console.Beep(1320, 500);
                Console.Beep(1188, 250);
                Console.Beep(1056, 250);
                Console.Beep(990, 500);
                Console.Beep(990, 250);
                Console.Beep(1056, 250);
                Console.Beep(1188, 500);
                Console.Beep(1320, 500);
                Console.Beep(1056, 500);
                Console.Beep(880, 500);
                Console.Beep(880, 500);
                Thread.Sleep(500);
                Console.Beep(660, 1000);
                Console.Beep(528, 1000);
                Console.Beep(594, 1000);
                Console.Beep(495, 1000);
                Console.Beep(528, 1000);
                Console.Beep(440, 1000);
                Console.Beep(419, 1000);
                Console.Beep(495, 1000);
                Console.Beep(660, 1000);
                Console.Beep(528, 1000);
                Console.Beep(594, 1000);
                Console.Beep(495, 1000);
                Console.Beep(528, 500);
                Console.Beep(660, 500);
                Console.Beep(880, 1000);
                Console.Beep(838, 2000);
                Console.Beep(660, 1000);
                Console.Beep(528, 1000);
                Console.Beep(594, 1000);
                Console.Beep(495, 1000);
                Console.Beep(528, 1000);
                Console.Beep(440, 1000);
                Console.Beep(419, 1000);
                Console.Beep(495, 1000);
                Console.Beep(660, 1000);
                Console.Beep(528, 1000);
                Console.Beep(594, 1000);
                Console.Beep(495, 1000);
                Console.Beep(528, 500);
                Console.Beep(660, 500);
                Console.Beep(880, 1000);
                Console.Beep(838, 2000);
            }
        }

        static void Login()
        {
            Config.cancelToken = new CancellationToken();
            client.RequestAllUsersOnStartup = true;
            client.ClientPrivateInformation.Email = Config.Email;
            client.ClientPrivateInformation.Password = Config.Password;

            client.Connected += (sender, e) =>
            {
                Console.WriteLine($"Connected! User: {e.User.Username}");
            };
            client.SendLoginRequest();
            Thread t = new Thread(() => client.Connect());
            t.Start();
            SetupEvents(Config.cancelToken);
        }

        #endregion

        #region Crypt and Decrypt
        //http://tekeye.biz/2015/encrypt-decrypt-c-sharp-string
        private const string initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;

        public WaveFormat WaveFormat { get => waveFormat; set => waveFormat = value; }
        public WaveFormat WaveFormat1 { get => waveFormat; set => waveFormat = value; }

        //Encrypt
        public static string EncryptString(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        //Decrypt
        public static string DecryptString(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
        #endregion



    }
    public enum LogType { Chat, Console, Log, SendedMessages, Error, DeletedMessages, ActionLog, Misc };
}
