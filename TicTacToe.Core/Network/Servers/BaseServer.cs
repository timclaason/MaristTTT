﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TicTacToe.Core.Network.Servers
{
    public abstract class BaseServer : NetworkNode
    {
        public abstract Services PerformHandshake(Socket socket);

        public abstract void Start(Socket socket);
    }
}
