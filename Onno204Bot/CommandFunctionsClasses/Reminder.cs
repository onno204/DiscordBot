using Newtonsoft.Json.Linq;
using Onno204Bot.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onno204Bot.CommandFunctionsClasses
{
    class Reminder
    {
        public DateTime Date { get; set; }
        public DUser duser { get; set; }
        public string RemindOfWhat { get; set; }
        public int JsonID { get; set; }

        public Reminder(DateTime Date, DUser duser, string RemindOfWhat) {
            this.Date = Date;
            this.duser = duser;
            this.RemindOfWhat = RemindOfWhat;
            Random rdn = new Random();
            this.JsonID = rdn.Next(1, 10000000);
        }
        
        public string FormatToString() {
            return Date.Millisecond + ":" + duser.Member.Id + ":" + RemindOfWhat;
        }
        
        public Reminder FromString(string s) {
            string[] ssplit = s.Split(':');
            int DateMili = Int32.Parse(ssplit[0]);
            this.Date = new DateTime(DateMili);
            this.duser = null;
            this.RemindOfWhat = ssplit[2];
            return this;
        }

        
        public void SaveReminder()
        {
            String filePath = @Config.SaveFiles + "Reminders.json";
            if (!File.Exists(filePath)) { File.Create(filePath).Close(); }
            JObject JSON = null;
            try {
                JSON = JObject.Parse(File.ReadAllText(@filePath));
            }
            catch (Exception e) { Utils.Log(e.Message + ":" + e.StackTrace, LogType.Error);  }
            if (JSON == null) {
                JSON = new JObject();
            }
            JSON["Rand" + this.JsonID] = FormatToString();
            File.WriteAllText(filePath, JSON.ToString());
        }
        public void ReadAllReminders()
        {
            String filePath = @Config.SaveFiles + "Reminders.json";
            if (!File.Exists(filePath)) { File.Create(filePath).Close(); }
            JObject JSON = JObject.Parse(File.ReadAllText(@filePath));
            


            //String rtn = JSON[video.Title].ToString();
            File.WriteAllText(filePath, JSON.ToString());
            List<Reminder> RemndL = new List<Reminder>();
        }

    }


    enum ReminderType {
        normal, message, messagespam, PrivateCall
    }

}
