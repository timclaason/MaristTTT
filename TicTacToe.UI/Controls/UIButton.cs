using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.Core.Structures;

namespace TicTacToe.UI.Controls
{
    public class UIButton : Button
    {
        const int BUTTON_HEIGHT = 90;
        const int BUTTON_WIDTH = 90;

        public UIButton(int buttonID)
        {
            this.ButtonID = buttonID;
            this.Name = buttonID.ToString();
            this.Height = BUTTON_HEIGHT;
            this.Width = BUTTON_WIDTH;
        }

        public int ButtonID
        {
            get;
            set;
        }

        public void Reset()
        {
            this.Text = String.Empty;
        }

        public void SetTextBasedOnSymbol(BoardSymbol symbol)
        {
            this.Text = String.Empty;
            if (symbol == BoardSymbol.O)
                this.Text = "O";
            if (symbol == BoardSymbol.X)
                this.Text = "X";
        }
    }
}
