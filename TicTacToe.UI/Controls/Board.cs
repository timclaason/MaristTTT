using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.Core.Structures;

namespace TicTacToe.UI.Controls
{
    public class Board : Panel
    {
        TicTacToe.Core.Structures.Board _underlyingBoard = null;

        const int BUTTON_HEIGHT = 90;
        const int BUTTON_WIDTH = 90;
        const int BASE_X = 10;
        const int BASE_Y = 10;

        const int CONTROL_WIDTH = 310;
        const int CONTROL_HEIGHT = 310;

        const int CONTROL_DISTANCE = 99;

        public Board()
        {
            initialize();
        }

        public bool IsLocked
        {
            get;
            set;
        }

        public void AttachBoard(TicTacToe.Core.Structures.Board board)
        {
            _underlyingBoard = board;
        }

        public void Redraw(TicTacToe.Core.Structures.Board board)
        {
            this.AttachBoard(board);
            initialize();

            for(int b = 0; b < 9; b++)
            {
                Control[] targetControl = this.Controls.Find(b.ToString(), true);
                
                if(targetControl == null || targetControl.Length == 0)
                    throw new KeyNotFoundException();
                Button targetButton = (Button)targetControl[0];

                if (board.TileAt(b).Value == BoardSymbol.O)
                    targetButton.Text = "O";
                if (board.TileAt(b).Value == BoardSymbol.X)
                    targetButton.Text = "X";
            }

            this.IsLocked = false;
            this.Refresh();
        }

        private void initialize()
        {
            this.Width = CONTROL_WIDTH;
            this.Height = CONTROL_HEIGHT;

            for(int button = 0; button < 9; button++)
            {
                Button b = new Button();
                b.Height = BUTTON_HEIGHT;
                b.Width = BUTTON_WIDTH;
                b.Text = String.Empty;
                b.Name = button.ToString();
                b.Click += (sender2, e2) =>
                {
                    if (b.Text != String.Empty)
                        return;
                    if (this.IsLocked)
                        return;
                    b.Text = "X";
                    this.IsLocked = true;

                    if(_underlyingBoard != null)
                    {
                        _underlyingBoard.TileAt(button).SetValue(BoardSymbol.X);
                    }
                    this.IsLocked = true;
                };

                int x = 0;
                int y = 0;
                if(button < 3)
                    y = BASE_Y;
                if( button >= 3 && button < 6)
                    y = BASE_Y + CONTROL_DISTANCE;
                if(button >= 6)
                    y = BASE_Y + CONTROL_DISTANCE + CONTROL_DISTANCE;
                if(button == 0 || button == 3 || button == 6)
                    x = BASE_X;
                if(button == 1 || button == 4 || button == 7)
                    x = BASE_X + CONTROL_DISTANCE;
                if(button == 2 || button == 5 || button == 8)
                    x = BASE_X + CONTROL_DISTANCE + CONTROL_DISTANCE;

                b.Location = new System.Drawing.Point(x, y);
                this.Controls.Add(b);               

            }
        }

    }
}
 
