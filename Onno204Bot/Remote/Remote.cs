using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;
using Onno204Bot.Events;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Onno204Bot.Remote
{
    class Remote
    {

        public static List<RemoteInfo> infos = new List<RemoteInfo>();
        public static async Task Start() {
            while (true) {
                try {
                    WebClient wc = new WebClient();
                    String commands = wc.DownloadString(RemoteConf.URL + "json.php");

                    string convert = commands.Replace('[', ' ').Replace(']', ' ').Replace("null", "0").Replace("},{", "}@{");//Remove errors
                    string[] Objects = convert.Split('@');//Split every json code
                    foreach (string jsonS in Objects) {
                        JObject json = JObject.Parse(jsonS);//String to json
                        RemoteInfo info = new RemoteInfo();
                        info.Id = Int32.Parse(json.SelectToken("ID").ToString());
                        info.Command = json.SelectToken("command").ToString();
                        info.TextChannelID = ulong.Parse(json.SelectToken("TextChannelID").ToString());
                        info.VoiceChannelID = ulong.Parse(json.SelectToken("VoiceChannelID").ToString());
                        info.Running = (Int32.Parse(json.SelectToken("Running").ToString()) == 1) ? true : false;
                        info.Done = (Int32.Parse(json.SelectToken("Done").ToString()) == 1) ? true : false;

                        infos.Add(info); //Add to a List
                    }
                }
                catch (Exception ee) { Error(1, ee.StackTrace); }

                //Handle the list
                foreach (RemoteInfo inf in infos) {
                    try {
                        if (inf.Done || inf.Running) { continue; }//If Done/Running, Ignore
                        if (!(inf.Command == "blabla123bla123")) {
                            new Task(() => {
                                try {
                                    Console.WriteLine("Doing: " + inf.Command);
                                    SendRunning(inf.Id);//Say running state
                                    string cmd = inf.Command.Split(' ')[0];

                                    Task<DiscordChannel> t1 = Program.discord.GetChannelAsync(inf.VoiceChannelID);
                                    t1.Wait();
                                    DUser duser = new DUser(inf.TextChannelID, inf.VoiceChannelID, 383323298681061386, t1.Result.GuildId, cmd, null, "");
                                    duser.RemoteInt = inf.Id;
                                    duser.Args = Utils.ReplaceFirstOccurrence(inf.Command, cmd, "");

                                    CommandFunction2.ExecuteCmd(cmd, duser);


                                    SendDone(inf.Id, "output");//Done
                                }catch (Exception ee) { Error(inf.Id, ee.Message + "<:>" + ee.StackTrace); }
                            }).Start();
                        }
                    }
                    catch (Exception e) { Error(inf.Id, e.StackTrace); }
                }
                infos.Clear();
                Thread.Sleep(5000);
                await Task.Delay(1);
            }
        }
        public static void SendDone(int ID, String Output)
        {
            WebClient wc = new WebClient();
            NameValueCollection vals = new NameValueCollection();
            vals.Add("DoneID", "" + ID);
            vals.Add("DoneCmd", Output);
            wc.UploadValues(RemoteConf.URL + "UpdateDB.php", vals);
        }
        public static void SendRunning(int ID)
        {
            WebClient wc2 = new WebClient();
            String rtn1 = wc2.DownloadString(RemoteConf.URL + "UpdateDB.php?RunningCmd=" + ID);
        }
        public static void Error(int ID, String Error)
        {
            try
            {
                WebClient wc = new WebClient();
                NameValueCollection vals = new NameValueCollection();
                vals.Add("DoneID", "" + ID);
                vals.Add("DoneCmd", "Error: " + Error);
                wc.UploadValues(RemoteConf.URL + "UpdateDB.php?", vals);
                Utils.Log("Remote Error:" + ID + "ID, Msg: " + Error, LogType.Error);
            }
            catch (Exception e) {
                Console.WriteLine("MAIN-ERROR: " + e.Message);
            }
        }
    }

}
