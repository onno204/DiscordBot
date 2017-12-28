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
            //Console.WriteLine("Loging in with:  " + client.ClientPrivateInformation.Email + ":" + client.ClientPrivateInformation.Password);
            client.Connected += (sender, e) =>
            {
                Console.WriteLine($"Connected! User: {e.User.Username}");
            };
            String tok = client.SendLoginRequest();
            //client.Connect();
<<<<<<< HEAD
            Console.WriteLine($"Token: {tok}");
=======
            //Console.WriteLine($"Token: {tok}");
>>>>>>> parent of 0992202... Now support for Custom Webinterface(Code not public YET)
            return tok;
        }
    }
}
