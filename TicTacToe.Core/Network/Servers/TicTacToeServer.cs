using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Core.Structures;

namespace TicTacToe.Core.Network.Servers
{
    public class TicTacToeServer : BaseServer
    {
        
        public bool GameInProgress = false;

        protected BoardSymbol _serverSymbol = BoardSymbol.O;
        protected BoardSymbol _clientSymbol = BoardSymbol.X;

        bool _isTwoPlayer = false;

        public override Services PerformHandshake(Socket socket)
        {
            ///Server sends <TICTACTOE>
            SendMessageThroughSocket(socket, CommonMessages.TICTACTOE_REQUEST_TEXT);

            ///Client responds with <ACK>
            string received = ListenForMessage(socket);

            if (received != CommonMessages.ACKNOWLEDGE_TEXT)
            {
                return Services.Invalid;
            }

            ///Server sends <WHATCHAWANT>
            SendMessageThroughSocket(socket, CommonMessages.OFFER_DESIRED_SYMBOL_TEXT);

            ///Client sends either X or O
            received = ListenForMessage(socket);

            _isTwoPlayer = (received == CommonMessages.REQUEST_SYMBOL_2_PLAYER);

            if (received == CommonMessages.REQUEST_SYMBOL_O_TEXT)
            {
                _serverSymbol = BoardSymbol.X;
                _clientSymbol = BoardSymbol.O;
            }
            else
            {
                _serverSymbol = BoardSymbol.O;
                _clientSymbol = BoardSymbol.X;
            }



            return Services.TicTacToe;
        }

        

        

        

        public override void Start(Socket socket)
        {
			return;
        }

        protected bool gameEnded(Board board)
        {
            if (board.DetectWinner() != BoardSymbol.Blank)
                return true;
            if (board.BoardIsCatscratch() == true)
                return true;

            return false;
        }

        

        protected void sendGameEndedMessage(Socket socket, Board board, bool closeConnection)
        {
            string prepackagedMessage = String.Empty;
            if (board.DetectWinner() == BoardSymbol.O)
                prepackagedMessage = CommonMessages.GAME_OVER_O_WINS;
            if (board.DetectWinner() == BoardSymbol.X)
                prepackagedMessage = CommonMessages.GAME_OVER_X_WINS;
            if (board.BoardIsCatscratch())
                prepackagedMessage = CommonMessages.GAME_OVER_CAT_SCRATCH;

            base.SendMessageThroughSocket(socket, NetworkMessage.BuildComplexMessage(board, prepackagedMessage));

			if(closeConnection)
				base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
        }

        protected void sendUpdatedBoard(Socket socket, TicTacToe.Core.Structures.Board board)
        {
            string message = board.SerializeObject();
            SendMessageThroughSocket(socket, message);
        }

    }
}
