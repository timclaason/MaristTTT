using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TicTacToe.Core.Network.Servers
{
    public class SimpleWebServer : BaseServer
    {
        const string MIME_HEADER = "text/html";
        const string STATUS_CODE = "202 OK";

        private string getPageHeader(string pageText)
        {
            byte[] message = Encoding.ASCII.GetBytes(pageText);
            int messageLength = message.Length;

            String sendBuffer = "";
            
            sendBuffer = sendBuffer + "HTTP/1.1 " + STATUS_CODE + "\r\n";
            sendBuffer = sendBuffer + "Server: Server42-b\r\n";
            sendBuffer = sendBuffer + "Content-Type: " + MIME_HEADER + "\r\n";
            sendBuffer = sendBuffer + "Accept-Ranges: bytes\r\n";
            sendBuffer = sendBuffer + "Content-Length: " + messageLength + "\r\n\r\n";

            return sendBuffer;
        }

        private void sendWebpage(Socket socket, string pageText)
        {

            byte[] message = Encoding.ASCII.GetBytes(pageText);
            int messageLength = message.Length;

            String sendBuffer = getPageHeader(pageText);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(sendBuffer);
            
            base.SendMessageThroughSocket(socket, sendBuffer);
            base.SendMessageThroughSocket(socket, pageText);

        }

        public override Services PerformHandshake(Socket socket)
        {
            try
            {                
                sendWebpage(socket, NetworkMessages.GENERIC_WEBPAGE_MESSAGE);
                return Services.SimpleWeb;
            }
            catch
            {
                return Services.Invalid;
            }
        }

        public override void Start(Socket socket)
        {
            return;
        }
    }
}
