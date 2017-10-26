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

        BoardSymbol _serverSymbol = BoardSymbol.O;
        BoardSymbol _clientSymbol = BoardSymbol.X;

        bool _isTwoPlayer = false;

        public override Services PerformHandshake(Socket socket)
        {
            ///Server sends <TICTACTOE>
            SendMessageThroughSocket(socket, NetworkMessages.TICTACTOE_REQUEST_TEXT);

            ///Client responds with <ACK>
            string received = ListenForMessage(socket);

            if (received != NetworkMessages.ACKNOWLEDGE_TEXT)
            {
                return Services.Invalid;
            }

            ///Server sends <WHATCHAWANT>
            SendMessageThroughSocket(socket, NetworkMessages.OFFER_DESIRED_SYMBOL_TEXT);

            ///Client sends either X or O
            received = ListenForMessage(socket);

            _isTwoPlayer = (received == NetworkMessages.REQUEST_SYMBOL_2_PLAYER);

            if (received == NetworkMessages.REQUEST_SYMBOL_O_TEXT)
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

        private void runOnePlayerGame(Socket socket)
        {
            Board board = new Board();

            sendUpdatedBoard(socket, board);

            bool gameHasEnded = false;

            while (gameHasEnded == false)
            {
                if (_serverSymbol == BoardSymbol.X)
                {
                    System.Threading.Thread.Sleep(125);
                    makeBestMove(board);

                    gameHasEnded = gameEnded(board);

                    if (gameHasEnded)
                    {
                        break;
                    }
                    sendUpdatedBoard(socket, board);
                }

                NetworkMessage userInput = new NetworkMessage(ListenForMessage(socket));

                foreach (NetworkMessage m in userInput.Messages)
                {
                    if (m.MessageType == MessageTypes.Board)
                    {
                        board = Board.Deserialize(m.RawMessage);
                        break;
                    }
                    ///For now, drop non-board input on the floor
                    ///This logic implies the possibility that the client could send more than 
                    ///one board.  If this happens, only take the first board in the message
                }


                gameHasEnded = gameEnded(board);

                if (gameHasEnded)
                {
                    break;
                }

                if (_serverSymbol == BoardSymbol.X)
                    continue;

                makeBestMove(board); //Updates the existing board                        

                gameHasEnded = gameEnded(board);

                if (gameHasEnded)
                {
                    break;
                }
                sendUpdatedBoard(socket, board);
            }

            sendGameEndedMessage(socket, board);
            board = null;
        }

        private bool nextPlay(Socket socket, TicTacToeGame game, BoardSymbol currentPlay)
        {
            ///Get X's move
            string receivedMessage = base.ListenForMessage(socket);
            NetworkMessage message = new NetworkMessage(receivedMessage);

            foreach (NetworkMessage m in message.Messages)
            {
                if (m.MessageType == MessageTypes.Board)
                {
                    game.CurrentBoard = Board.Deserialize(m.RawMessage);

                    if (currentPlay == BoardSymbol.X)
                        game.CurrentMove = BoardSymbol.O;
                    else
                        game.CurrentMove = BoardSymbol.X;
                }
            }

            bool gameHasEnded = gameEnded(game.CurrentBoard);

            if(!gameHasEnded)
                sendUpdatedBoard(game.GetOtherPlayer(socket), game.CurrentBoard);
            return gameHasEnded;
        }


        private void runTwoPlayerGame(Socket socket)
        {
            TicTacToeGame game = Server.Games.FindNext(socket);
            _clientSymbol = BoardSymbol.O;
            _serverSymbol = BoardSymbol.X;

            ///The server sends a message to the client, informing them of their symbol
            if(game.IsWaiting) ///First player to join
            {
                _clientSymbol = BoardSymbol.X;
                _serverSymbol = BoardSymbol.O;
                base.SendMessageThroughSocket(socket, NetworkMessages.REQUEST_SYMBOL_X_TEXT);
            }
            else ///Second player to join
            {
                _clientSymbol = BoardSymbol.O;
                _serverSymbol = BoardSymbol.X;
                base.SendMessageThroughSocket(socket, NetworkMessages.REQUEST_SYMBOL_O_TEXT);
            }

            while(game.IsWaiting)
            {
                ///
            }

            System.Threading.Thread.Sleep(500);
            
            ///Both players have joined the game - send blank board            
            sendUpdatedBoard(game.GetOtherPlayer(socket), game.CurrentBoard);



            bool gameHasEnded = false;
            while(gameHasEnded == false)
            {
                gameHasEnded = nextPlay(socket, game, BoardSymbol.X);
                                
                if(gameHasEnded)
                {
                    break;
                }

                gameHasEnded = nextPlay(game.GetOtherPlayer(socket), game, BoardSymbol.O);

                if(gameHasEnded)
                {
                    break;
                }                
            }

            sendGameEndedMessage(socket, game.CurrentBoard);
            sendGameEndedMessage(game.GetOtherPlayer(socket), game.CurrentBoard);
            game.CurrentBoard = null;
        }

        public override void Start(Socket socket)
        {
            if (_isTwoPlayer)
            {
                runTwoPlayerGame(socket);
            }
            else
            {
                runOnePlayerGame(socket);
            }
        }

        private bool gameEnded(Board board)
        {
            if (board.DetectWinner() != BoardSymbol.Blank)
                return true;
            if (board.BoardIsCatscratch() == true)
                return true;

            return false;
        }

        private void makeBestMove(Board board)
        {
            AllWinningCombinations allWinningCombos = new AllWinningCombinations();

            ///Kill shot
            foreach (WinningCombination combo in allWinningCombos)
            {
                BoardTile tile1 = board.TileAt(combo.Position1);
                BoardTile tile2 = board.TileAt(combo.Position2);
                BoardTile tile3 = board.TileAt(combo.Position3);

                if (tile1.Value == _serverSymbol && tile2.Value == _serverSymbol && tile3.Value == BoardSymbol.Blank)
                {
                    tile3.SetValue(_serverSymbol);
                    return;
                }
                if (tile1.Value == _serverSymbol && tile3.Value == _serverSymbol && tile2.Value == BoardSymbol.Blank)
                {
                    tile2.SetValue(_serverSymbol);
                    return;
                }
                if (tile2.Value == _serverSymbol && tile3.Value == _serverSymbol && tile1.Value == BoardSymbol.Blank)
                {
                    tile1.SetValue(_serverSymbol);
                    return;
                }
            }

            /////DEFENSIVE
            foreach (WinningCombination combo in allWinningCombos)
            {
                int count = 0;

                BoardTile tile1 = board.TileAt(combo.Position1);
                BoardTile tile2 = board.TileAt(combo.Position2);
                BoardTile tile3 = board.TileAt(combo.Position3);



                if (tile1.Value == _clientSymbol)
                    count++;
                if (tile2.Value == _clientSymbol)
                    count++;
                if (tile3.Value == _clientSymbol)
                    count++;

                if (count > 1)
                {
                    if (tile1.Value == BoardSymbol.Blank)
                    {
                        tile1.SetValue(_serverSymbol);
                        return;
                    }
                    if (tile2.Value == BoardSymbol.Blank)
                    {
                        tile2.SetValue(_serverSymbol);
                        return;
                    }
                    if (tile3.Value == BoardSymbol.Blank)
                    {
                        tile3.SetValue(_serverSymbol);
                        return;
                    }
                }
            }

            ///Offensive
            foreach (WinningCombination combo in allWinningCombos)
            {
                int count = 0;
                BoardTile tile1 = board.TileAt(combo.Position1);
                BoardTile tile2 = board.TileAt(combo.Position2);
                BoardTile tile3 = board.TileAt(combo.Position3);


                ///This needs to be smarter...
                count = 0;
                if (tile1.Value == _serverSymbol || tile1.Value == BoardSymbol.Blank)
                    count++;
                if (tile2.Value == _serverSymbol || tile2.Value == BoardSymbol.Blank)
                    count++;
                if (tile3.Value == _serverSymbol || tile3.Value == BoardSymbol.Blank)
                    count++;

                if (count == 3)
                {
                    if (tile1.Value == BoardSymbol.Blank)
                    {
                        tile1.SetValue(_serverSymbol);
                        return;
                    }
                    if (tile2.Value == BoardSymbol.Blank)
                    {
                        tile2.SetValue(_serverSymbol);
                        return;
                    }
                    if (tile3.Value == BoardSymbol.Blank)
                    {
                        tile3.SetValue(_serverSymbol);
                        return;
                    }
                }

                for (int space = 0; space < 9; space++)
                {
                    BoardTile tile = board.TileAt(space);
                    if (tile.Value == BoardSymbol.Blank)
                    {
                        tile.SetValue(_serverSymbol);
                        return;
                    }
                }
            }
        }

        private void sendGameEndedMessage(Socket socket, Board board)
        {
            string prepackagedMessage = String.Empty;
            if (board.DetectWinner() == BoardSymbol.O)
                prepackagedMessage = NetworkMessages.GAME_OVER_O_WINS;
            if (board.DetectWinner() == BoardSymbol.X)
                prepackagedMessage = NetworkMessages.GAME_OVER_X_WINS;
            if (board.BoardIsCatscratch())
                prepackagedMessage = NetworkMessages.GAME_OVER_CAT_SCRATCH;

            base.SendMessageThroughSocket(socket, NetworkMessage.BuildComplexMessage(board, prepackagedMessage));
        }

        private void sendUpdatedBoard(Socket socket, TicTacToe.Core.Structures.Board board)
        {
            string message = board.SerializeObject();
            SendMessageThroughSocket(socket, message);
        }

    }
}
