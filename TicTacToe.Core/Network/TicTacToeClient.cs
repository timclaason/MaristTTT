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
        BoardSymbol _desiredSymbol = BoardSymbol.X;
        bool _twoPlayer;

        public TicTacToeClient(BoardSymbol desiredSymbol, bool twoPlayer)
        {
            _desiredSymbol = desiredSymbol;
            _twoPlayer = twoPlayer;
        }

        public BoardSymbol DesiredSymbol
        {
            get
            {
                return _desiredSymbol;
            }
        }

        private bool performServerHandshake(Socket socket)
        {
            ///Client sends <PLAY>
            SendMessageThroughSocket(socket, NetworkMessages.TICTACTOE_REQUEST_TEXT);

            ///Server sends <PLAY>
            string receivedMessage = ListenForMessage(socket);

            ///Expectation is that we receive a game request.  If not, die
            if (receivedMessage != NetworkMessages.TICTACTOE_REQUEST_TEXT)
            {
                if (this.GameOver != null)
                    this.GameOver(new object(), new EventArgs());
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return false;
            }

            ///Client sends <ACK>
            SendMessageThroughSocket(socket, NetworkMessages.ACKNOWLEDGE_TEXT);

            ///Server offers symbol selection
            receivedMessage = ListenForMessage(socket);

            ///Clent sends desired symbol...
            if (receivedMessage == NetworkMessages.OFFER_DESIRED_SYMBOL_TEXT)
            {
                string messageToSend = NetworkMessages.REQUEST_SYMBOL_X_TEXT;
                if (_desiredSymbol == BoardSymbol.O)
                    messageToSend = NetworkMessages.REQUEST_SYMBOL_O_TEXT;
                if (_twoPlayer)
                    messageToSend = NetworkMessages.REQUEST_SYMBOL_2_PLAYER;
                SendMessageThroughSocket(socket, messageToSend);

                string serverResponse = ListenForMessage(socket);

                _desiredSymbol = BoardSymbol.X;

                if (serverResponse == NetworkMessages.REQUEST_SYMBOL_O_TEXT)
                    _desiredSymbol = BoardSymbol.O;
                
            }
            else
            {
                if (this.GameOver != null)
                    this.GameOver(new object(), new EventArgs());
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return false;
            }



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

                    int gamePlayIterations = 0;
                    string lastBoardSent = String.Empty;
                    string lastBoardReceived = String.Empty;
                    while(true)
                    {
                        ///1st one should be the board - 1st message should be an empty board
                        NetworkMessage receivedMessage = null;
                            //new NetworkMessage(ListenForMessage(socket));

                        if(_twoPlayer || gamePlayIterations > 0)
                            receivedMessage = new NetworkMessage(ListenForMessage(socket));

                        if(_desiredSymbol == BoardSymbol.O && gamePlayIterations == 0)
                        {
                            foreach (NetworkMessage m in receivedMessage.Messages)
                            {
                                if (m.MessageType == MessageTypes.Board)
                                    triggerBoardReceived(m.Board);
                            }

                            ///If the user wants to be O, the 2nd message will be another board with 1 space populated
                            receivedMessage = new NetworkMessage(ListenForMessage(socket));
                        }



                        if (receivedMessage != null)
                        {
                            foreach (NetworkMessage m in receivedMessage.Messages)
                            {
                                if (m.MessageType == MessageTypes.Board)
                                {
                                    triggerBoardReceived(m.Board);
                                    lastBoardReceived = m.Board.SerializeObject();
                                }

                                if (m.MessageType == MessageTypes.Message && m.MessageContainsGameOverIndicator)
                                    triggerGameOver(m.RawMessage);
                            }

                            if (receivedMessage.MessageContainsGameOverIndicator)
                                break;
                        }

                        if(lastBoardReceived == lastBoardSent && (lastBoardSent != String.Empty || lastBoardReceived != String.Empty))
                        {
                            receivedMessage = new NetworkMessage(ListenForMessage(socket));
                            foreach (NetworkMessage m in receivedMessage.Messages)
                            {
                                if (m.MessageType == MessageTypes.Board)
                                    triggerBoardReceived(m.Board);
                            }
                        }



                        ///Waits for user to make their selection.  Times out after 50 * 1000 ms
                        int sleepIterations = 0;

                        while(input.CurrentValue == null)
                        {
                            System.Threading.Thread.Sleep(1000);
                            sleepIterations++;

                            if (sleepIterations > 60 * 10) //10 minute timeout
                            {
                                socket.Shutdown(SocketShutdown.Both);
                                socket.Close();
                                return;
                            }
                                
                        }

                        lastBoardSent = input.CurrentValue.SerializeObject();

                        SendMessageThroughSocket(socket, lastBoardSent);
                        input.CurrentValue = null;

                        gamePlayIterations++;
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
