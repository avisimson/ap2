using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Communication
{
    public class CommandMessage
    {
        public int CommandID { get; set; }

        public JObject CommandArgs { get; set; }
        /*
         * convert this object back to json
         * returns the json object inside a string.
         */
        public string ToJSON()
        {
            JObject Obj = new JObject();
            Obj["CommandID"] = CommandID;
            JObject args = new JObject(CommandArgs);
            Obj["CommandArgs"] = args;
            return Obj.ToString();
        }
        //convert json type to commandmessage.
        //returns=converted message.
        public static CommandMessage ParseJSON(string str)
        {
            CommandMessage msg = new CommandMessage();
            JObject Obj = JObject.Parse(str);
            msg.CommandID = (int)Obj["CommandID"];
            JObject arr = (JObject)Obj["CommandArgs"];
            msg.CommandArgs = arr;
            return msg;
        }
    }
}