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
        
		
        public void CloneHandlers(NetworkNode from, NetworkNode to)
        {
            to.MessageReceived += from.MessageReceived;
            to.MessageSent += from.MessageSent;
        }		       
		
        public String ListenForMessage(Socket socket)
        {
            byte[] bytes = new Byte[1024];

            string data = String.Empty;

            int bytesRec = socket.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

            if (data.Length > 0 && MessageReceived != null)
                MessageReceived(data + " [Remote Endpoint: " + socket.RemoteEndPoint.ToString() + "]", new EventArgs());

            ServerLogger.WriteToLog("REC", data);

            return data;
        }

		public void SendMessageThroughSocket(Socket socket, string message, bool signalLock)
		{
			if (!signalLock)
			{
				this.SendMessageThroughSocket(socket, message);
				return;
			}

			string newMessage = NetworkMessage.BuildComplexMessage(message, CommonMessages.LOCK_SIGNAL);
			this.SendMessageThroughSocket(socket, newMessage);

		}


		public void SendMessageThroughSocket(Socket socket, string message)
        {
			try
			{
				byte[] msg = Encoding.ASCII.GetBytes(message);
				socket.Send(msg);

				ServerLogger.WriteToLog("SND", message);
				if (this.MessageSent != null)
					MessageSent(message + " [Remote Endpoint: " + socket.RemoteEndPoint.ToString() + "]", new EventArgs());
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
