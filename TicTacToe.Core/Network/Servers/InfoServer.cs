using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TicTacToe.Core.Network.Servers
{
    public class InfoServer : BaseServer
    {
        public override Services PerformHandshake(Socket socket)
        {
            try
            {
                this.SendMessageThroughSocket(socket, CommonMessages.INFO_REQUEST_TEXT);
                return Services.Info;
            }
            catch
            {
                return Services.Invalid;
            }
        }

        public override void Start(Socket socket)
        {
            DateTime initiationTime = DateTime.Now;

            while(true)
            {
                string rawMessage = ListenForMessage(socket);

                if (rawMessage == Environment.NewLine)
                    continue;

                TimeSpan lengthOfConnection = DateTime.Now - initiationTime;

                if(lengthOfConnection.TotalHours > 2)
                {
                    SendMessageThroughSocket(socket, CommonMessages.DISCONNECT_TEXT);
                    break;
                }
                
                NetworkMessage message = new NetworkMessage(rawMessage);

                if(message.RawMessage.Contains(CommonMessages.DISCONNECT_TEXT))
                {
                    SendMessageThroughSocket(socket, CommonMessages.DISCONNECT_TEXT);
                    break;
                }

                if (message.MessageType == MessageTypes.Board || message.MessageType == MessageTypes.Message)
                {
                    SendMessageThroughSocket(socket, CommonMessages.INVALID_REQUEST_TEXT);
                    continue;
                }

                respondToMessage(socket, message);



                
            }
        }

        const string INFO_REQUEST_DAY = "DAY";
        const string INFO_REQUEST_DATE = "DATE";
        const string INFO_REQUEST_TIME = "TIME";
        const string INFO_REQUEST_YEAR = "YEAR";


        private void respondToMessage(Socket socket, NetworkMessage message)
        {
            if(message.MessageType != MessageTypes.Complex)
            {
                SendMessageThroughSocket(socket, CommonMessages.INVALID_REQUEST_TEXT);
                return;
            }

            if(message.Messages.Count < 2)
            {
                SendMessageThroughSocket(socket, CommonMessages.INVALID_REQUEST_TEXT);
                return;
            }
            if(message.Messages[0].RawMessage != CommonMessages.REQUEST_INFO_MESSAGE)
            {
                SendMessageThroughSocket(socket, CommonMessages.INVALID_REQUEST_TEXT);
                return;
            }

            string parameter = message.Messages[1].RawMessage;

            if (parameter.ToUpper().Contains(INFO_REQUEST_DAY.ToUpper()))
            {
                SendMessageThroughSocket(socket, DateTime.Now.DayOfWeek.ToString());
                return;
            }
            if (parameter.ToUpper().Contains(INFO_REQUEST_DATE.ToUpper()))
            {
                SendMessageThroughSocket(socket, DateTime.Now.ToString());
                return;
            }
            if (parameter.ToUpper().Contains(INFO_REQUEST_TIME.ToUpper()))
            {
                SendMessageThroughSocket(socket, DateTime.Now.ToLongTimeString());
                return;
            }
            if (parameter.ToUpper().Contains(INFO_REQUEST_YEAR.ToUpper()))
            {
                SendMessageThroughSocket(socket, DateTime.Now.Year.ToString());
                return;
            }
            SendMessageThroughSocket(socket, CommonMessages.INVALID_REQUEST_TEXT);
        }


    }
}
