using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Network
{
    public class NetworkMessages
    {
        public static string GAME_REQUEST_TEXT = "<PLAY>";
        public static string CONFIRM_GAME_REQUEST_TEXT = "<ACK>";
        public static string INVALID_REQUEST_TEXT = "<INVALID>";
        public static string CLOSING_SOCKET_TEXT = "<BUZZOFF>";
        public static string GAME_OVER_CAT_SCRATCH = "<CATSCRATCH>";
        public static string GAME_OVER_X_WINS = "<XWINS>";
        public static string GAME_OVER_O_WINS = "<OWINS>";
    }
}
