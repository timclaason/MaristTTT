using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Core.Structures;

namespace TicTacToe.Core.Network
{
    public class TicTacToeServer : NetworkNode
    {
        public bool GameInProgress = false;

        BoardSymbol _serverSymbol = BoardSymbol.O;
        BoardSymbol _clientSymbol = BoardSymbol.X;
        
        private bool performClientHandshake(Socket socket)
        {
            /*
             1.  Client connects to server
             2.  Client sends <PLAY> to server
             3.  Server sends <PLAY> to client
             4.  Client sends <ACK> to server
             5.  Server sends <WHATCHAWANT> to client
             * */

            ///Client sends <PLAY>
            string gameInitiationMessage = ListenForMessage(socket);
            if (gameInitiationMessage != NetworkMessages.GAME_REQUEST_TEXT)
                return false;

            ///Server sends <PLAY>
            SendMessageThroughSocket(socket, NetworkMessages.GAME_REQUEST_TEXT);

            ///Client sends <ACK>
            string received = ListenForMessage(socket);

            if (received != NetworkMessages.CONFIRM_GAME_REQUEST_TEXT)
            {
                return false;
            }

            ///Server sends <WHATCHAWANT>
            SendMessageThroughSocket(socket, NetworkMessages.OFFER_DESIRED_SYMBOL_TEXT);

            ///Client sends either X or O
            received = ListenForMessage(socket);

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


            return true;
        }

        private void sendUpdatedBoard(Socket socket, TicTacToe.Core.Structures.Board board)
        {
            string message = board.SerializeObject();
            SendMessageThroughSocket(socket, message);
        }

        private void guessBestMove(Board board)
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
            foreach(WinningCombination combo in allWinningCombos)
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

                if(count == 3)
                {
                    if(tile1.Value == BoardSymbol.Blank)
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

                for(int space = 0; space < 9; space++)
                {
                    BoardTile tile = board.TileAt(space);
                    if(tile.Value == BoardSymbol.Blank)
                    {
                        tile.SetValue(_serverSymbol);
                        return;
                    }
                }
            }
        }

        private void spinupGameServerThread(Socket socket)
        {
            BackgroundWorker bg = new BackgroundWorker();

            bg.DoWork += (sender, e2) =>
                {
                    try
                    {
                        bool handshakeOccurred = performClientHandshake(socket);

                        if (!handshakeOccurred)
                        {
                            this.CloseSocketConnection(socket, NetworkMessages.CLOSING_SOCKET_TEXT);
                        }

                        Board board = new Board();

                        sendUpdatedBoard(socket, board);

                        bool gameHasEnded = false;

                        while (gameHasEnded == false)
                        {
                            if (_serverSymbol == BoardSymbol.X)
                            {
                                System.Threading.Thread.Sleep(500);
                                guessBestMove(board);

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
                                }
                                ///For now, drop non-board input on the floor
                            }


                            gameHasEnded = gameEnded(board);

                            if (gameHasEnded)
                            {
                                break;
                            }

                            if (_serverSymbol == BoardSymbol.X)
                                continue;

                            guessBestMove(board); //Updates the existing board                        

                            gameHasEnded = gameEnded(board);

                            if (gameHasEnded)
                            {
                                break;
                            }
                            sendUpdatedBoard(socket, board);
                        }

                        sendGameEndedMessage(socket, board);
                        board = null;


                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    catch
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                        catch { }
                        return;
                    }
                };
            bg.RunWorkerAsync();

        }


        public void Start()
        {
            Socket listener = base.GetListener(Settings.LISTENING_PORT);

            try
            {
                listener.Listen(10);

                // Start listening for connections.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.

                    Socket socket = base.ListenForConnection(listener);
                    spinupGameServerThread(socket);                    
                }

            }
            catch (Exception e)
            {
                ServerLogger.WriteToLog("ERR", e.Message);
                throw;
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

        private void sendGameEndedMessage(Socket socket, Board board)
        {
            string prepackagedMessage = String.Empty;
            if (board.DetectWinner() == BoardSymbol.O)
                prepackagedMessage = NetworkMessages.GAME_OVER_O_WINS;
            if (board.DetectWinner() == BoardSymbol.X)
                prepackagedMessage = NetworkMessages.GAME_OVER_X_WINS;
            if (board.BoardIsCatscratch())
                prepackagedMessage = NetworkMessages.GAME_OVER_CAT_SCRATCH;

            base.SendMessageThroughSocket(socket, NetworkMessage.GetComplexMessage(board, prepackagedMessage));
        }
    }
}
