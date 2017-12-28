using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using Onno204Bot.Cfg;

namespace Onno204Bot.Lib
{
    class DUser
    {
        public DiscordMember Member { get { return _Member; } set { _Member = value; ValueUpdated(); } }
        public DiscordChannel TextChannel { get { return _TextChannel; } set { _TextChannel = value; ValueUpdated(); } }
        public DiscordChannel VoiceChannel { get { return _VoiceChannel; } set { _VoiceChannel = value; ValueUpdated(); } }
        public String Command { get { return _Command; } set { _Command = value; ValueUpdated(); } }
        public String Args { get { return _Args; } set { _Args = value; ValueUpdated(); } }
        public String[] Arg { get { return _Arg; } set { _Arg = value; ValueUpdated(); } }
        public VoiceNextClient VNClient { get { return _VNClient; } set { _VNClient = value; ValueUpdated(); } }
        public VoiceNextConnection VNCon { get { return _VNCon; } set { _VNCon = value; ValueUpdated(); } }
        public DiscordClient Client { get { return _Client; } set { _Client = value; ValueUpdated(); } }
        public DiscordGuild Guild { get { return _Guild; } set { _Guild = value; ValueUpdated(); } }
        public VoiceNextClient BuildinVoiceNextClient { get { return _BuildinVoiceNextClient; } set { _BuildinVoiceNextClient = value; ValueUpdated(); } }
        public int RemoteInt { get { return _RemoteInt; } set { _RemoteInt = value; ValueUpdated(); } }

        private DiscordMember _Member { get; set; }
        private DiscordChannel _TextChannel { get; set; }
        private DiscordChannel _VoiceChannel { get; set; }
        private String _Command { get; set; }
        private String _Args { get; set; }
        private String[] _Arg { get; set; }
        private VoiceNextClient _VNClient { get; set; }
        private VoiceNextConnection _VNCon { get; set; }
        private DiscordClient _Client { get; set; }
        private DiscordGuild _Guild { get; set; }
        private VoiceNextClient _BuildinVoiceNextClient { get; set; }
        private int _RemoteInt { get; set; } = 0;




        private void ValueUpdated() {
            //GetSetItems.LastDUser = this;
            return;
        }

        public DUser(CommandContext ctx, Boolean FullSetup = false, ulong VoiceChn = 0) {
            if (!FullSetup) {
                Setup(ctx.Channel.Id, 0, ctx.Member.Id, ctx.Member.Guild.Id);
            }
            else
            {
                DiscordChannel chn = null;
                try { 
                    if (VoiceChn == 0)
                    {
                        DiscordVoiceState vstat = ctx.Member.VoiceState;
                        if (vstat.Channel == null && chn == null)
                        {
                            // they did not specify a channel and are not in one
                        }
                        else { chn = vstat.Channel; }
                    }
                    else {
                        Task<DiscordChannel> t = Program.discord.GetChannelAsync(VoiceChn);
                        t.Wait();
                        chn = t.Result;
                    }
                }catch(Exception e) { Utils.Log(e.Message + ":" + e.StackTrace, LogType.Error); }
                ulong CnhID = (chn == null) ? 394488303161704448 : chn.Id;
                Setup(ctx.Channel.Id, CnhID, ctx.Member.Id, ctx.Channel.Guild.Id, ctx.Command.ToString(), ctx.Message.Content.Split(' '), null);
            }
        }
        
        public DUser(ulong TextChannelID, ulong VoiceChannelID, ulong DMember, ulong GuildID, string command = null, string[] Arg = null, string Args = null) {
            Setup(TextChannelID, VoiceChannelID, DMember, GuildID, command, Arg, Args);
        }
        
        public void Setup(ulong TextChannelID, ulong VoiceChannelID, ulong DMember, ulong GuildID, string command = null, string[] Arg = null, string Args = null)
        {
            Task<DiscordChannel> t1 = Program.discord.GetChannelAsync(TextChannelID);
            t1.Wait();
            this._TextChannel = t1.Result;
            Task<DiscordChannel> t2 = Program.discord.GetChannelAsync(VoiceChannelID);
            t2.Wait();
            this._VoiceChannel = t2.Result;
            Task<DiscordMember> t3 = TextChannel.Guild.GetMemberAsync(DMember);
            t3.Wait();
            this._Member = t3.Result;
            this._Command = command;
            this._Arg = Arg;
            this._Args = Args;
            this._Client = Program.discord;
            this._BuildinVoiceNextClient = Program.Voice;
            Task<DiscordGuild> t4 = Program.discord.GetGuildAsync(GuildID);
            t4.Wait();
            this._Guild = t4.Result;
            this._VNClient = Program.Voice;
            this._VNCon = VNClient.GetConnection(Guild);
            GetSetItems.LastDUser = this;
        }


    }
}
