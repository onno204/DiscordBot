using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onno204Bot
{
    class Messages
    {
        //Leave a message empty and it won't be send!

            //Markup and Style
        //$ = New message part(Re-use)
        //First the '!' Than the '@' !!

        //~1 - ~2 - ~3 - ~4 --- e.t.c.
        public static String BotTitle = "***~1 bot at your service!***"; // ~1 = User Name

        /*
            1. await DiscordUtils.SendBotMessage(Messages.AudioMusicQueueEnded, ctx.Message.Channel, user: ctx.User); 
            2. await DiscordUtils.SendBotMessage(Utils.Replace(Messages.AudioMusicAddedToQueue, "~1", vnc.Channel.Name), ctx.Message.Channel, user: ctx.User);
            3. await DiscordUtils.SendBotMessage(Utils.Replace(Utils.Replace(Messages.MessageDeleted, "~2", e.Message.Content), "~1", e.Message.Author.Mention), e.Channel);
         */


        //command Messages
        public static String CommandNotFound = "Het commando '~1' hebben we niet herkent in ons systeem."; // ~1 = Command
        public static String EchoReply = "~2"; // ~1 = User --- ~2 = Message
        public static String MentionReply = "Ik kom zo snel mogelijk bij je terug! ~1"; // ~1 = User --- ~2 = Message
        public static String HelpReply = "Nog geen Hulp pagina Beschrikbaar!"; // ~1 = User
        public static String PlayingGamesStatus = "Getting written by @onno204";
        public static String NewDMCreated = "Hoi, Ik zag dat je een prive kanaal maakte!";
        public static String MessageDeleted = "~1 heeft een bericht verwijderd!$!~2"; // ~1 = User --- ~2 = Message
        public static String PurgeMessage = "Ik heb met plezier ~1 berichten verwijderd."; // ~1 = Amount of messages
        

        //Music Stuff
        public static String AudioAlreadyInguild = "Ik ben al verbonden in deze server! :)";
        public static String AudioUserNotInChannel = "Ik wil wel verbinden maar je zit zelf niet in een channel?";
        public static String AudioConnectedTo = "Ik ben het kanaal '~1' gejoined!"; // ~1 = Channel name
        public static String AudioDisconnected = "Ik heb het kanaal '~1' verlaten!"; // ~1 = Channel name
        public static String AudioNotConnected = "Ik ben niet verbonden in deze server!";
        public static String AudioNotconnectedToServer = "Ik ben niet verbonden in deze server, Laat me het proberen!";
        public static String AudioStartedPlaying = "Begonnen met het afspelen in '~1'"; // ~1 = Channel name
        public static String AudioDownloading = "Ik ben bezig om je video te downloaden!!";
        public static String AudioMusicAddedToQueue = "succesfully added '~1' to the queue!"; // ~1 = Youtube name
        public static String AudioMusicNextSongInQueue = "Now playing '~1'."; // ~1 = Youtube name
        public static String AudioMusicAlreadyPlaying = "Er wordt al muziek afgespeelt!";
        public static String AudioMusicRemove = "Het nummer '~1' is verwijderd!"; // ~1 = Youtube name
        public static String AudioMusicRemoveERROR = "Het ID '~1' is niet gevonden in de queue!"; // ~1 = ID
        public static String AudioMusicQueueEnded = "Er staat geen muziek meer in de queue!";
        public static String AudioMusicProcentLeft = "De muziek is nu op ~1 met het afspelen."; // ~1 = **%
        public static String AudioVolumeChange = "Het volume van de muziek is aangepast!";


    }
}
