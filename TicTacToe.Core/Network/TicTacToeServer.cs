using System;
using System.Collections.Generic;
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

        private bool performClientHandshake(Socket socket)
        {
            /*
             1.  Client connects to server
             2.  Client sends <PLAY> to server
             3.  Server sends <PLAY> to client
             4.  Client sends <ACK> to server
             * */
            string gameInitiationMessage = ListenForMessage(socket);
            if (gameInitiationMessage != NetworkMessages.GAME_REQUEST_TEXT)
                return false;

            SendMessageThroughSocket(socket, NetworkMessages.GAME_REQUEST_TEXT);

            string received = ListenForMessage(socket);

            if (received != NetworkMessages.CONFIRM_GAME_REQUEST_TEXT)
            {
                return false;
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

                if (tile1.Value == BoardSymbol.O && tile2.Value == BoardSymbol.O && tile3.Value == BoardSymbol.Blank)
                {
                    tile3.SetValue(BoardSymbol.O);
                    return;
                }
                if (tile1.Value == BoardSymbol.O && tile3.Value == BoardSymbol.O && tile2.Value == BoardSymbol.Blank)
                {
                    tile2.SetValue(BoardSymbol.O);
                    return;
                }
                if (tile2.Value == BoardSymbol.O && tile3.Value == BoardSymbol.O && tile1.Value == BoardSymbol.Blank)
                {
                    tile1.SetValue(BoardSymbol.O);
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


                
                if (tile1.Value == BoardSymbol.X)
                    count++;
                if (tile2.Value == BoardSymbol.X)
                    count++;
                if (tile3.Value == BoardSymbol.X)
                    count++;

                if (count > 1)
                {
                    if (tile1.Value == BoardSymbol.Blank)
                    {
                        tile1.SetValue(BoardSymbol.O);
                        return;
                    }
                    if (tile2.Value == BoardSymbol.Blank)
                    {
                        tile2.SetValue(BoardSymbol.O);
                        return;
                    }
                    if (tile3.Value == BoardSymbol.Blank)
                    {
                        tile3.SetValue(BoardSymbol.O);
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
                if (tile1.Value == BoardSymbol.O || tile1.Value == BoardSymbol.Blank)
                    count++;
                if (tile2.Value == BoardSymbol.O || tile2.Value == BoardSymbol.Blank)
                    count++;
                if (tile3.Value == BoardSymbol.O || tile3.Value == BoardSymbol.Blank)
                    count++;

                if(count == 3)
                {
                    if(tile1.Value == BoardSymbol.Blank)
                    {
                        tile1.SetValue(BoardSymbol.O);
                        return;
                    }
                    if (tile2.Value == BoardSymbol.Blank)
                    {
                        tile2.SetValue(BoardSymbol.O);
                        return;
                    }
                    if (tile3.Value == BoardSymbol.Blank)
                    {
                        tile3.SetValue(BoardSymbol.O);
                        return;
                    }
                }

                for(int space = 0; space < 9; space++)
                {
                    BoardTile tile = board.TileAt(space);
                    if(tile.Value == BoardSymbol.Blank)
                    {
                        tile.SetValue(BoardSymbol.O);
                        return;
                    }
                }
            }
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
                        NetworkMessage userInput = new NetworkMessage(ListenForMessage(socket));

                        foreach(NetworkMessage m in userInput.Messages)
                        {
                            if (m.MessageType == MessageTypes.Board)
                            {
                                board = Board.Deserialize(m.RawMessage);
                            }
                            ///For now, drop non-board input on the floor
                        }
                                                                      

                        gameHasEnded = gameEnded(socket, board);

                        if (gameHasEnded)
                        { 
                            break;
                        }

                        guessBestMove(board); //Updates the existing board                        

                        gameHasEnded = gameEnded(socket, board);

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

            }
            catch (Exception e)
            {
                ServerLogger.WriteToLog("ERR", e.Message);
                throw;
            }
        }

        private bool gameEnded(Socket socket, Board board)
        {
            if (board.DetectWinner() != BoardSymbol.Blank)
                return true;
            if (board.BoardIsCatscratch() == true)
                return true;
            
            return false;            
        }

        private void sendGameEndedMessage(Socket socket, Board board)
        {
            if (board.DetectWinner() == BoardSymbol.O)
                base.SendMessageThroughSocket(socket, NetworkMessages.GAME_OVER_O_WINS);
            if (board.DetectWinner() == BoardSymbol.X)
                base.SendMessageThroughSocket(socket, NetworkMessages.GAME_OVER_X_WINS);
            if (board.BoardIsCatscratch())
                base.SendMessageThroughSocket(socket, NetworkMessages.GAME_OVER_CAT_SCRATCH);
        }
    }
}
