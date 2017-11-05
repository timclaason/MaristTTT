using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TicTacToe.Core.Network
{
    public class NetworkNode
    {
        public event EventHandler MessageReceived;
        public event EventHandler MessageSent;
        public event EventHandler ClientConnected;
        public event EventHandler StartedListening;

        Socket _mainListener;

        public void CloneHandlers(NetworkNode from, NetworkNode to)
        {
            to.MessageReceived += from.MessageReceived;
            to.MessageSent += from.MessageSent;
            to.ClientConnected += from.ClientConnected;
            to.StartedListening += from.StartedListening;
        }

        /// <summary>
        /// As of right now, this only supports IPV4
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public Socket GetListener(int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            foreach(IPAddress a in ipHostInfo.AddressList)
            {
                if (a.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = a;
                    break;
                }
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            _mainListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _mainListener.Bind(localEndPoint);

            return _mainListener;
        }

        public void KillListener()
        {
            _mainListener.Close();
            //_mainListener.Disconnect(true);
            //_mainListener.Shutdown(SocketShutdown.Both);
        }

        public Socket GetClientSocket(string server, int port)
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.Resolve(server);
                //Dns.GetHostEntry(server);
            
            // Dns.Resolve(server);

            if(ipHostInfo.AddressList.Length == 0)
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

            // Create a TCP/IP  socket.  
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(remoteEP);
            return socket;
        }

        public Socket ListenForConnection(Socket listener)
        {
            if (StartedListening != null)
                StartedListening("Started listening on " + listener.LocalEndPoint.ToString(), new EventArgs());

            Socket socket = listener.Accept();

            if (socket.Connected)
            {
                string message = "Connection received from client " + socket.RemoteEndPoint.ToString();

                ServerLogger.WriteToLog("CXN", message);
                if (ClientConnected != null)
                {
                    ClientConnected(message, new EventArgs());
                }
            }

            return socket;

        }

        public String ListenForMessage(Socket socket)
        {
            byte[] bytes = new Byte[1024];

            string data = String.Empty;

            int bytesRec = socket.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

            if (data.Length > 0 && MessageReceived != null)
                MessageReceived(data, new EventArgs());

            ServerLogger.WriteToLog("REC", data);

            return data;
        }

        public void SendMessageThroughSocket(Socket socket, string message)
        {
			try
			{
				byte[] msg = Encoding.ASCII.GetBytes(message);
				socket.Send(msg);

				ServerLogger.WriteToLog("SND", message);
				if (this.MessageSent != null)
					MessageSent(message, new EventArgs());
			}
			catch
			{
			}
        }

        public void CloseSocketConnection(Socket socket, string closeMessage)
        {
            this.SendMessageThroughSocket(socket, closeMessage);
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
			ServerLogger.WriteToLog("END", closeMessage);
        }
    }
}
