﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Communication
{
    public interface IClientHandler
    {
        void HandleClient(TcpClient client, List<TcpClient> clients);
    }
}