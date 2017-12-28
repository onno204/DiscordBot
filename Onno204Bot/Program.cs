using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.Codec;
using Onno204Bot.Events;
using Onno204Bot.Lib;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onno204Bot
{
    internal class Program
    {
        public static DiscordClient discord = (DiscordClient)null;
        public static CommandsNextModule commands = (CommandsNextModule)null;
        public static VoiceNextClient Voice = (VoiceNextClient)null;
        public static VoiceNextConnection ConnectedVNC = (VoiceNextConnection)null;
        public static DiscordGuild ConnectedDG = (DiscordGuild)null;
        private const string initVector = "pemgail9uzpgzl88";
        private const int keysize = 256;

        private static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)((_param1, _param2, _param3, _param4) => true);
            Program.MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            await Program.Start();
        }

        public static void SetupEvents()
        {
            Utils.Log("Setting Up...", LogType.Console);
            Program.discord.MessageDeleted += (AsyncEventHandler<MessageDeleteEventArgs>)(async e =>
            {
                try
                {
                    if (e.Message.Content.StartsWith(Config.CommandString))
                        return;
                    string Message = e.Message.Author.Username + "#" + e.Message.Author.Discriminator + ": " + (object)e.Message.Timestamp + ", " + e.Message.Content;
                    Utils.Log(Message, LogType.DeletedMessages);
                    await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.MessageDeleted, "~2", e.Message.Content), "~1", e.Message.Author.Mention), new DUser(e.Channel.Id, 0UL, e.Guild.Id, e.Client.CurrentUser.Id, (string)null, (string[])null, (string)null));
                    Message = (string)null;
                }
                catch (Exception ex)
                {
                    Utils.Log("Error, " + ex.StackTrace, LogType.Error);
                }
            });
            Program.discord.DmChannelCreated += (AsyncEventHandler<DmChannelCreateEventArgs>)(async e => {
                await Task.Delay(1);
            });
            Utils.Log("Done setting.", LogType.Console);
        }

        public static async Task Start()
        {
            try
            {
                Console.WriteLine("Started Login!");
                await Program.Login();
            }
            catch (Exception ex)
            {
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine(Directory.GetFiles(Directory.GetCurrentDirectory()).ToString());
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                Console.WriteLine("================= ERRORRRR CRASHH ===================");
                new Thread((ThreadStart)(() => Program.ConsoleBeep())).Start();
            }
            while (true)
            {
                string crl = Console.ReadLine();
                string Cmd = crl.Split(' ')[0];
                crl = Utils.ReplaceFirstOccurrence(crl, Cmd, "");
                string str = Cmd;
                string substring;
                if (!(str == "help"))
                {
                    if (str == "join")
                    {
                        substring = crl.Substring(crl.LastIndexOf('/') + 1);
                        Utils.Log("Joined, " + substring, LogType.Misc);
                    }
                    else
                        Utils.Log("Try typing 'help'!", LogType.Console);
                }
                else
                {
                    Utils.Log("Try typing '1231'!", LogType.Console);
                    Utils.Log("Try typing 'hel123p'!", LogType.Console);
                }
                substring = (string)null;
                crl = (string)null;
                Cmd = (string)null;
            }
        }

        public static void ConsoleBeep()
        {
            while (true)
            {
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
            Program.discord = new DiscordClient(new DiscordConfiguration()
            {
                //Token = DiscordLoginUtils.GetDiscordToken(),
                Token = "Mzc5NzUxMDc5MjAzNzAwNzQy.DScCfw.H5BHfhXw-ZwvN4DMgPTdmTRBJ_c",
                TokenType = DSharpPlus.TokenType.Bot,
                LogLevel = DSharpPlus.LogLevel.Debug,
                UseInternalLogHandler = true
            });
            await Program.WaitReceiveToken();
            Utils.Log("Received token.", LogType.Console);
            Program.SetupEvents();
            await Onno204Bot.Events.Events.RegisterEvent();
            Utils.Log("Setting up commands...", LogType.Console);
            Program.commands = Program.discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = Config.CommandString,
                CaseSensitive = false,
                EnableDefaultHelp = true
            });
            Program.commands.RegisterCommands<Commands>();
            Program.commands.CommandExecuted += new AsyncEventHandler<CommandExecutionEventArgs>(CommandsEvents.Commands_CommandExecuted);
            Utils.Log("Done Setting up commands.", LogType.Console);
            Utils.Log("Setting up Voice...", LogType.Console);
            VoiceNextConfiguration vcfg = new VoiceNextConfiguration()
            {
                EnableIncoming = true,
                VoiceApplication = VoiceApplication.Voice
            };
            Program.Voice = Program.discord.UseVoiceNext(vcfg);
            Utils.Log("Done Setting up Voice.", LogType.Console);
            Utils.Log("Loggin in...", LogType.Console);
            await Program.discord.ConnectAsync();
            Utils.Log("Logged in.", LogType.Console);
            await Task.Delay(-1);
        }

        public static async Task WaitReceiveToken()
        {
            while (Program.discord == null)
                await Task.Delay(-1);
        }

        public static string EncryptString(string plainText, string passPhrase)
        {
            byte[] bytes1 = Encoding.UTF8.GetBytes("pemgail9uzpgzl88");
            byte[] bytes2 = Encoding.UTF8.GetBytes(plainText);
            byte[] bytes3 = new PasswordDeriveBytes(passPhrase, (byte[])null).GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(bytes3, bytes1);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes2, 0, bytes2.Length);
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText, string passPhrase)
        {
            byte[] bytes1 = Encoding.UTF8.GetBytes("pemgail9uzpgzl88");
            byte[] buffer = Convert.FromBase64String(cipherText);
            byte[] bytes2 = new PasswordDeriveBytes(passPhrase, (byte[])null).GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(bytes2, bytes1);
            MemoryStream memoryStream = new MemoryStream(buffer);
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] numArray = new byte[buffer.Length];
            int count = cryptoStream.Read(numArray, 0, numArray.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(numArray, 0, count);
        }
    }
}
