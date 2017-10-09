using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TicTacToe.Core.Network
{
    public class InfoClient : NetworkNode
    {
        Socket _socket;
        string _server;

        public bool Connect(string server)
        {
            _server = server;
            bool connected = performHandshake();
            return connected;
        }

        private bool performHandshake()
        {
            _socket = base.GetClientSocket(_server, Settings.LISTENING_PORT);
            this.SendMessageThroughSocket(_socket, NetworkMessages.INFO_REQUEST_TEXT);
            string messageReceived = this.ListenForMessage(_socket);
            return messageReceived == NetworkMessages.INFO_REQUEST_TEXT;
        }

        public String SendInfoRequest(string message)
        {
            List<String> messageParts = new List<string>();
            messageParts.Add(NetworkMessages.REQUEST_INFO_MESSAGE);
            messageParts.Add(message);
            string messageToSend = NetworkMessage.BuildComplexMessage(messageParts);
            this.SendMessageThroughSocket(_socket, messageToSend);
            return getServerResponse();
        }

        private string getServerResponse()
        {
            try
            {
                string received = base.ListenForMessage(_socket);
                return received;
            }
            catch
            {
                return "Error getting response";
            }
        }
    }
}
