using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Structures;
using System.Net.Sockets;

namespace TicTacToe.Core.Network.Servers
{
	public class TwoPlayerTicTacToeServer : TicTacToeServer
	{
		public override void Start(Socket socket)
		{
			TicTacToeGame game = runTwoPlayerGame(socket);
			Server.Games.Kill(game, socket);
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

			if (gameHasEnded)
				return gameHasEnded;

			sendUpdatedBoard(game.GetOtherPlayer(socket), game.CurrentBoard);
			return false;
		}


		private TicTacToeGame runTwoPlayerGame(Socket socket)
		{
			TicTacToeGame game = Server.Games.FindNext(socket);
			_clientSymbol = BoardSymbol.O;
			_serverSymbol = BoardSymbol.X;

			///The server sends a message to the client, informing them of their symbol
			if (game.IsWaiting) ///First player to join
			{
				_clientSymbol = BoardSymbol.X;
				_serverSymbol = BoardSymbol.O;				
				base.SendMessageThroughSocket(socket, CommonMessages.REQUEST_SYMBOL_X_TEXT, true);
			}
			else ///Second player to join
			{
				_clientSymbol = BoardSymbol.O;
				_serverSymbol = BoardSymbol.X;
				base.SendMessageThroughSocket(socket, CommonMessages.REQUEST_SYMBOL_O_TEXT, true);
			}

			while (game.IsWaiting)
			{
				///
			}

			System.Threading.Thread.Sleep(125);

			///Both players have joined the game - send blank board            
			sendUpdatedBoard(game.GetOtherPlayer(socket), game.CurrentBoard);



			bool gameHasEnded = false;
			while (gameHasEnded == false)
			{
				gameHasEnded = nextPlay(socket, game, BoardSymbol.X);

				if (gameHasEnded)
				{
					break;
				}

				gameHasEnded = nextPlay(game.GetOtherPlayer(socket), game, BoardSymbol.O);

				if (gameHasEnded)
				{
					break;
				}
			}

			sendGameEndedMessage(socket, game.CurrentBoard, false);
			sendGameEndedMessage(game.GetOtherPlayer(socket), game.CurrentBoard, false);

			base.CloseSocketConnection(game.GetOtherPlayer(socket), CommonMessages.DISCONNECT_TEXT);
			base.CloseSocketConnection(socket, CommonMessages.DISCONNECT_TEXT);
			

			return game;
		}
	}
}
