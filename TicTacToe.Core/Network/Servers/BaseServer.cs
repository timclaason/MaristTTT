using System;
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
        public abstract ServerApplication PerformHandshake(Socket socket);
    }
}
