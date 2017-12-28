using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onno204Bot.Remote
{
    class RemoteInfo
    {

        public int Id { get; set; }
        public ulong VoiceChannelID { get; set; }
        public ulong TextChannelID { get; set; }
        public string Command { get; set; }
        public Boolean Running { get; set; }
        public Boolean Done { get; set; }
    }
}
