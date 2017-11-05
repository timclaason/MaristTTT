using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Structures;

namespace TicTacToe.Core.Network
{
    public class TicTacToeGame : NetworkNode
    {
        public TicTacToeGame(Socket socket)
        {
            this.Player1 = socket;
            this.CurrentBoard = new Board();
            this.ID = Guid.NewGuid();
            this.CurrentMove = BoardSymbol.X;
            this.StartDate = DateTime.Now;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public TimeSpan TimeSinceStart
        {
            get
            {
                return DateTime.Now - this.StartDate;
            }
        }

        public Guid ID
        {
            get;
            set;
        }

        public BoardSymbol CurrentMove
        {
            get;
            set;
        }

        public Socket Player1
        {
            get;
            set;
        }

        public Socket Player2
        {
            get;
            set;
        }

        public bool IsWaiting
        {
            get
            {
                return this.Player1 != null && this.Player2 == null;
            }
        }

        public Socket GetOtherPlayer(Socket s)
        {
            if (s.Handle == Player1.Handle)
                return Player2;
            return Player1;
        }

        public Board CurrentBoard
        {
            get;
            set;
        }

    }

    public class TicTacToeGameCollection : List<TicTacToeGame>
    {
        public void Kill(TicTacToeGame g, Socket s)
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                TicTacToeGame game = this[i];

                
                if (game.TimeSinceStart.TotalMinutes > 10 && (game.Player1 == null || game.Player2 == null))
                {
                    this.RemoveAt(i);
                    continue;
                }
                if (game.Player1.Handle == s.Handle || game.Player2.Handle == s.Handle)
                {
                    this.RemoveAt(i);
                    continue;
                }
                if(game.TimeSinceStart.Hours > 1)
                {
                    this.RemoveAt(i);
                    continue;
                }
                
            }
        }


        public TicTacToeGame FindNext(Socket socket)
        {
            foreach(TicTacToeGame g in this)
            {
                if (g.IsWaiting == false)
                    continue;
                g.Player2 = socket;
                return g;
            }

            TicTacToeGame newGame = new TicTacToeGame(socket);
            this.Add(newGame);
            return newGame;
        }
    }
}
