using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TicTacToe.Core.Network
{
    public class ServerLogger
    {
        static String LOG_FILE = "Server.log";

        public static void WriteToLog(string messageType, string message)
        {
            try
            {
                string outputMessage = DateTime.Now.ToString() + "\t" + messageType + ":" + message + Environment.NewLine;
                File.AppendAllText(LOG_FILE, outputMessage);
            }
            catch
            {
            }
        }
    }
}
