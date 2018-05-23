using Communication.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface IServer
    {
        void Start(); //start server execution.
        void Stop(); //stop server execution.
        void sendUpdatedLog(Object sender, MessageReceivedEventArgs e);
    }
}
