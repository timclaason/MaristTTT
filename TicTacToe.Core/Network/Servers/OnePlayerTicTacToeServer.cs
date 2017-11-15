using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Structures;
using System.Net.Sockets;

namespace TicTacToe.Core.Network.Servers
{
	public class OnePlayerTicTacToeServer : TicTacToeServer
	{
		public override void Start(Socket socket)
		{
			runOnePlayerGame(socket);
		}

		private void makeBestMove(Board board)
		{
			BoardTile centerTile = board.TileAt(4);

			///Take the center spot right away, if not occupied
			if (centerTile.IsOccupied == false)
			{
				centerTile.SetValue(_serverSymbol);
				return;
			}

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

			sendGameEndedMessage(socket, board, true);
			board = null;
		}
	}
}
