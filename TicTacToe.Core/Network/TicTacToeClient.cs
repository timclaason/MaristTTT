using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Core.Structures;

namespace TicTacToe.Core.Network
{
    public class TicTacToeClient : NetworkNode
    {
        public event EventHandler GameOver;
        public event EventHandler NewBoardReceived;

        private bool performServerHandshake(Socket socket)
        {
            SendMessageThroughSocket(socket, NetworkMessages.GAME_REQUEST_TEXT);
            string receivedMessage = ListenForMessage(socket);

            ///Expectation is that we receive a game request.  If not, die
            if (receivedMessage != NetworkMessages.GAME_REQUEST_TEXT)
            {
                if (this.GameOver != null)
                    this.GameOver(new object(), new EventArgs());
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return false;
            }

            ///Send final ack, then wait to receive the simple object
            SendMessageThroughSocket(socket, NetworkMessages.CONFIRM_GAME_REQUEST_TEXT);
            return true;
        }

        private void triggerBoardReceived(Board board)
        {
            if (NewBoardReceived == null)
                return;
            NewBoardReceived(board, new EventArgs());
        }

        private void triggerGameOver(string message)
        {
            if (NewBoardReceived == null)
                return;
            NewBoardReceived(message, new EventArgs());
        }
       
        public void Start(string server, InputDevice input)
        {
            try
            {
                Socket socket = null;

                try
                {
                    socket = base.GetClientSocket(server, Settings.LISTENING_PORT);
                    bool handshakeSuccess = performServerHandshake(socket);

                    while(true)
                    {
                        string receivedBoardAsString = ListenForMessage(socket);

                        if(receivedBoardAsString == NetworkMessages.GAME_OVER_CAT_SCRATCH || 
                            receivedBoardAsString == NetworkMessages.GAME_OVER_O_WINS ||
                            receivedBoardAsString == NetworkMessages.GAME_OVER_X_WINS)
                        {
                            triggerGameOver(receivedBoardAsString);
                            break;
                        }

                        TicTacToe.Core.Structures.Board board = TicTacToe.Core.Structures.Board.Deserialize(receivedBoardAsString);
                        triggerBoardReceived(board);

                        if (board.BoardIsCatscratch() || board.DetectWinner() != BoardSymbol.Blank)
                            break;

                        int sleepIterations = 0;

                        while(input.CurrentValue == null)
                        {
                            System.Threading.Thread.Sleep(1000);
                            sleepIterations++;

                            if (sleepIterations > 50)
                            {
                                socket.Shutdown(SocketShutdown.Both);
                                socket.Close();
                                break;
                            }
                                
                        }
                        SendMessageThroughSocket(socket, input.CurrentValue.SerializeObject());
                        input.CurrentValue = null;
                        
                    }


                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
