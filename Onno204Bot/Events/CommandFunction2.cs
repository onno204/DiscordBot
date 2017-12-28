using Onno204Bot.Lib;
using System.Threading.Tasks;

namespace Onno204Bot.Events
{
    internal class CommandFunction2
    {
        public static async Task ExecuteCmd(string Function, DUser duser)
        {
            Function = Function.ToLower();
            if (!(Function == "hi"))
            {
                if (Function == "echo")
                    await CommandFunctions.Echo(duser);
                else if (Function == "ping")
                    await CommandFunctions.Ping(duser);
                else if (Function == "purge")
                    await CommandFunctions.Purge(duser);
                else if (Function == "musicjoinch")
                    await CommandFunctions.MusicJoinCh(duser);
                else if (Function == "musicleave")
                    await CommandFunctions.MusicLeave(duser);
                else if (Function == "musicplay")
                    await CommandFunctions.MusicPlay(duser);
                else if (Function == "musicqueue")
                    await CommandFunctions.MusicQueue(duser);
                else if (Function == "musicskip")
                    await CommandFunctions.MusicSkip(duser);
                else if (Function == "musiccontinuecrash")
                    await CommandFunctions.MusicContinueCrash(duser);
                else if (Function == "musiccontinueafterjoin")
                    await CommandFunctions.MusicContinueAfterJoin(duser);
                else if (Function == "musicremovesong")
                    await CommandFunctions.MusicRemoveSong(duser);
                else if (Function == "musicprocentleft")
                    await CommandFunctions.MusicProcentLeft(duser);
                else if (Function == "musicchangevolume")
                    await CommandFunctions.MusicChangeVolume(duser);
                else
                    await DiscordUtils.SendBotMessage("The command u entered wasn't found!", duser);
            }
            await Task.Delay(1);
        }
    }
}
