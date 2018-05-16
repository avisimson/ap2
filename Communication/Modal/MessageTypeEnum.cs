using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Modal
{
    //Type for messages in MessageRecievedEventArgs.
    public enum MessageTypeEnum : int
    {
        INFO,
        WARNING,
        FAIL
    }
}
