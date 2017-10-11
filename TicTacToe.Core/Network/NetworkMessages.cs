using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Network
{
    public class NetworkMessages
    {
        public static string TICTACTOE_REQUEST_TEXT = "<TICTACTOE>";
        public static string INFO_REQUEST_TEXT = "<INFO>";
        public static string WEB_SERVER_REQUEST_TEXT = "GET / HTTP";
        public static string ACKNOWLEDGE_TEXT = "<ACK>";
        public static string OFFER_DESIRED_SYMBOL_TEXT = "<WHATCHAWANT>";
        public static string REQUEST_SYMBOL_X_TEXT = "<X>";
        public static string REQUEST_SYMBOL_O_TEXT = "<O>";
        public static string INVALID_REQUEST_TEXT = "<INVALID>";
        public static string CLOSING_SOCKET_TEXT = "<BUZZOFF>";
        public static string GAME_OVER_CAT_SCRATCH = "<CATSCRATCH>";
        public static string GAME_OVER_X_WINS = "<XWINS>";
        public static string GAME_OVER_O_WINS = "<OWINS>";
        public static string DISCONNECT_TEXT = "<BYE>";

        public static string REQUEST_INFO_MESSAGE = "<REQUEST>";

        public static string GENERIC_WEBPAGE_MESSAGE = "<HTML><HEAD><TITLE>Generic</TITLE></HEAD><BODY>Welcome to the (Marist TTT) machine.<br/><br/>I'm sorry I do not have more to say right now.  I am merely a Tic Tac Toe server that has primitive abilities to serve out webpages on the port you requested.  <br/><br/>It is not inconceivable that I will evolve someday, assuming external pressures and my ability to survive in the wilderness; however, this page, along with any insight gleaned from it, is all I have to offer right now.</BODY></HTML>";
    }
}
