using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordSharp;

namespace Onno204Bot
{
    class DiscordLoginUtils
    {

        public static DiscordClient client = new DiscordClient();
        public static String GetDiscordToken() {

            client.ClientPrivateInformation.Email = Config.Email;
            client.ClientPrivateInformation.Password = Config.Password;

            client.Connected += (sender, e) =>
            {
                Console.WriteLine($"Connected! User: {e.User.Username}");
            };
            String tok = client.SendLoginRequest();
            //client.Connect();
            //Console.WriteLine($"Token: {tok}");
            return tok;
        }
    }
}
