using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using TicTacToe.Core.Structures;

namespace TicTacToe.Core.Network.Clients
{
    public class TicTacToeClient : Client
    {
        public event EventHandler GameOver;
        public event EventHandler NewBoardReceived;
		public event EventHandler LockSignaled;
        BoardSymbol _desiredSymbol = BoardSymbol.X;
        bool _twoPlayer;



        public TicTacToeClient(BoardSymbol desiredSymbol, bool twoPlayer)
        {
			this.MessageReceived += (sender2, e2) =>
			{
				if (sender2.ToString().Contains(CommonMessages.LOCK_SIGNAL))
					this.LockSignaled(sender2, e2);
			};

            _desiredSymbol = desiredSymbol;
            _twoPlayer = twoPlayer;
        }

        public BoardSymbol DesiredSymbol
        {
            get
            {
                return _desiredSymbol;
            }
            set
            {
                _desiredSymbol = value;
            }
        }

        private bool performServerHandshake(Socket socket)
        {
			///Client sends <PLAY>
			///
			string twoPlayerInitiationMessage = NetworkMessage.BuildComplexMessage(CommonMessages.TICTACTOE_REQUEST_TEXT, CommonMessages.REQUEST_SYMBOL_2_PLAYER);

			if(_twoPlayer)
				SendMessageThroughSocket(socket, twoPlayerInitiationMessage);
			else
				SendMessageThroughSocket(socket, CommonMessages.TICTACTOE_REQUEST_TEXT);

            ///Server sends <PLAY>
            string receivedMessage = ListenForMessage(socket);

            ///Expectation is that we receive a game request.  If not, die
            if (receivedMessage != CommonMessages.TICTACTOE_REQUEST_TEXT)
            {
                if (this.GameOver != null)
                    this.GameOver(new object(), new EventArgs());
				base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
                return false;
            }

            ///Client sends <ACK>
            SendMessageThroughSocket(socket, CommonMessages.ACKNOWLEDGE_TEXT);

            ///Server offers symbol selection
            receivedMessage = ListenForMessage(socket);

            ///Client sends desired symbol...
            if (receivedMessage == CommonMessages.OFFER_DESIRED_SYMBOL_TEXT)
            {
                string messageToSend = CommonMessages.REQUEST_SYMBOL_X_TEXT;
                if (_desiredSymbol == BoardSymbol.O)
                    messageToSend = CommonMessages.REQUEST_SYMBOL_O_TEXT;
                if (_twoPlayer)
                    messageToSend = CommonMessages.REQUEST_SYMBOL_2_PLAYER;
                SendMessageThroughSocket(socket, messageToSend);

                ///Problem here:  the sequence of messaging has changed (10/25 @ 9:52)
                ///We get a board, instead of a <X> or <O>
                ///

                if (!_twoPlayer && _desiredSymbol == BoardSymbol.O)
                    return true;

                string serverResponse = ListenForMessage(socket);
				NetworkMessage parsedResponse = new NetworkMessage(serverResponse);
                _desiredSymbol = BoardSymbol.X;

                if (parsedResponse.AnyMessageContains(CommonMessages.REQUEST_SYMBOL_O_TEXT))
                    _desiredSymbol = BoardSymbol.O;
                
            }
            else
            {
                if (this.GameOver != null)
                    this.GameOver(new object(), new EventArgs());

				base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
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

        private void runTwoPlayer(Socket socket, InputDevice input)
        {
            string lastBoardSent = String.Empty;
            string lastBoardReceived = String.Empty;
            int gamePlayIterations = 0;
            while (true)
            {
                ///1st one should be the board - 1st message should be an empty board
                NetworkMessage receivedMessage = null;
                //new NetworkMessage(ListenForMessage(socket));

                if (_twoPlayer || gamePlayIterations > 0)
                    receivedMessage = new NetworkMessage(ListenForMessage(socket));

                if (_desiredSymbol == BoardSymbol.O && gamePlayIterations == 0)
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

                if (lastBoardReceived == lastBoardSent && (lastBoardSent != String.Empty || lastBoardReceived != String.Empty))
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

                while (input.CurrentValue == null)
                {
                    System.Threading.Thread.Sleep(1000);
                    sleepIterations++;

                    if (sleepIterations > 60 * 10) //10 minute timeout
                    {
						base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
                        return;
                    }

                }

                lastBoardSent = input.CurrentValue.SerializeObject();

                SendMessageThroughSocket(socket, lastBoardSent);
                input.CurrentValue = null;

                gamePlayIterations++;
            }
        }

        private void runOnePlayer(Socket socket, InputDevice input)
        {
            int gamePlayIterations = 0;
            string lastBoardSent = String.Empty;
            string lastBoardReceived = String.Empty;
            while (true)
            {
                ///1st one should be the board - 1st message should be an empty board
                NetworkMessage receivedMessage = null;
                //new NetworkMessage(ListenForMessage(socket));

                if (_twoPlayer || gamePlayIterations > 0)
                    receivedMessage = new NetworkMessage(ListenForMessage(socket));

                if (_desiredSymbol == BoardSymbol.O && gamePlayIterations == 0)
                {
                    receivedMessage = new NetworkMessage(ListenForMessage(socket));
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

                if (lastBoardReceived == lastBoardSent && (lastBoardSent != String.Empty || lastBoardReceived != String.Empty))
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

                while (input.CurrentValue == null)
                {
                    System.Threading.Thread.Sleep(1000);
                    sleepIterations++;

                    if (sleepIterations > 60 * 10) //10 minute timeout
                    {
						base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
                        return;
                    }

                }

                lastBoardSent = input.CurrentValue.SerializeObject();

                SendMessageThroughSocket(socket, lastBoardSent);
                input.CurrentValue = null;

                gamePlayIterations++;
            }
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
                                        
                    if (_twoPlayer)
                        runTwoPlayer(socket, input);
                    else
                        runOnePlayer(socket, input);

					base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);

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
