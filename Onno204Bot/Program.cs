using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.Codec;
using Onno204Bot.Events;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onno204Bot
{
    class Program
    {
        /// <summary>
        /// START ERROR PROBALY IN THE CONFIG.CS!!!
        /// cHANGE THE FILE.READ TO YOUR OWN FILE OR JUST WRITE IT!
        /// </summary>
        #region Variables
        public static DiscordClient discord = null;
        public static CommandsNextModule commands = null;
        public static VoiceNextClient Voice = null;
        public static VoiceNextConnection ConnectedVNC = null;
        public static DiscordGuild ConnectedDG = null;
        #endregion
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public static async Task MainAsync(string[] args)
        {
            await Start();
        }

        
        #region Client Functions

        //All the Message functions
        public static void SetupEvents()
        {
            Utils.Log("Setting Up...", LogType.Console);
            discord.MessageDeleted += async e => {
                try {
                    if (e.Message.Content.StartsWith(Config.CommandString)) { return; }
                    String Message = e.Message.Author.Username+"#"+e.Message.Author.Discriminator + ": " + e.Message.Timestamp + ", " + e.Message.Content;
                    Utils.Log(Message, LogType.DeletedMessages);
                    await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.MessageDeleted, "~2", e.Message.Content), "~1", e.Message.Author.Mention), e.Channel);
                }
                catch (Exception ee) {
                    Utils.Log("Error, " + ee.Message, LogType.Error);
                }
            };
            discord.DmChannelCreated += async e => {
                await DiscordUtils.SendBotMessage(Messages.NewDMCreated, e.Channel);
            };
            
            /*
            Keep crashing the program without exiting
                //Task to do when connected/Loggedin
                discord.SocketOpened += async () => {
                    await discord.UpdateStatusAsync(new DiscordGame(Messages.PlayingGamesStatus), UserStatus.Online);
                };

                //Instant reaction on a Mention
                client.MentionReceived += (sender, e) =>
                {
                    SendBotMessage( Utils.Replace( Utils.Replace(Messages.MentionReply, "~2", e.Message.Content), "~1", MentoinUser(e.Message.Author) ), e.Message.Channel());
                };
            */

            Utils.Log("Done setting.", LogType.Console);
        }

        #endregion
        

        #region Login & Start & error
        public static async Task Start()
        {
            try
            {
                Console.WriteLine("Started Login!");
                await Login();
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
                        Utils.Log("Try typing '1231'!", LogType.Console);
                        Utils.Log("Try typing 'hel123p'!", LogType.Console);
                        break;
                    case "join":
                        string substring = crl.Substring(crl.LastIndexOf('/') + 1);
                        //client.AcceptInvite(substring);
                        Utils.Log("Joined, "+ substring, LogType.Misc);
                        break;
                    default:
                        Utils.Log("Try typing 'help'!", LogType.Console);
                        break;
                }
            }

        }
        
        public static void ConsoleBeep()
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

        public static async Task Login()
        {
            Utils.Log("Receiving token...", LogType.Console);
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = DiscordLoginUtils.GetDiscordToken(),
                //Token = "Mzc5NzUxMDc5MjAzNzAwN1z2Qy.DO-632Q.242hCMjJe1WKb32tulLsdgsf1lJk",
                TokenType = TokenType.User,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });
            await WaitReceiveToken();
            Utils.Log("Received token.", LogType.Console);
            
            SetupEvents();
            Utils.Log("Setting up commands...", LogType.Console);
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Config.CommandString,
                CaseSensitive = false,
                EnableDefaultHelp = true
            });

            commands.RegisterCommands<Commands>();// Register commands Class
            commands.RegisterCommands<AdminCommands>();// Register commands Class
            commands.CommandExecuted += Events.CommandsEvents.Commands_CommandExecuted; //Register Command Events Class
            //commands.SetHelpFormatter<HelpFormatter>();

            Utils.Log("Done Setting up commands.", LogType.Console);
            Utils.Log("Setting up Voice...", LogType.Console);
            var vcfg = new VoiceNextConfiguration {
                VoiceApplication = VoiceApplication.Voice
            };

            // and let's enable it
            Voice = discord.UseVoiceNext(vcfg);
            Utils.Log("Done Setting up Voice.", LogType.Console);


            Utils.Log("Loggin in...", LogType.Console);
            await discord.ConnectAsync();
            Utils.Log("Logged in.", LogType.Console);
            await Task.Delay(-1);
        }

        public static async Task WaitReceiveToken() {
            while (discord == null)
            {
                await Task.Delay(-1);
            }
        }

        #endregion

        #region Crypt and Decrypt
        //http://tekeye.biz/2015/encrypt-decrypt-c-sharp-string
        private const string initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;

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
}
