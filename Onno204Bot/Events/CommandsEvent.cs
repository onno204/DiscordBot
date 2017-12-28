using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;

namespace Onno204Bot.Events
{
    internal class CommandsEvents
    {
        public static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            Utils.Log(DateTime.Now.ToString() + " Command: " + e.Context.Member.Username + ", " + e.Context.Message.Content, LogType.ActionLog);
            return Task.CompletedTask;
        }
    }
}
