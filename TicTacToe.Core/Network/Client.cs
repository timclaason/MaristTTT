using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TicTacToe.Core.Network
{
	public class Client : NetworkNode
	{
		public Socket GetClientSocket(string server, int port)
		{			
			IPHostEntry ipHostInfo = Dns.Resolve(server);
			
			if (ipHostInfo.AddressList.Length == 0)
			{
				ipHostInfo = Dns.GetHostByAddress(server);
			}

			IPAddress ipAddress = ipHostInfo.AddressList[0];

			foreach (IPAddress a in ipHostInfo.AddressList)
			{
				if (a.AddressFamily == AddressFamily.InterNetwork)
				{
					ipAddress = a;
					break;
				}
			}

			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(remoteEP);
			return socket;
		}
	}
}
