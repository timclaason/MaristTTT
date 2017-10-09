using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.Core.Network;
using TicTacToe.Core.Structures;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        TicTacToe.UI.Controls.UIBoard _board;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _txtIPAddress.Recall();

        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            BoardSymbol desiredSymbol = BoardSymbol.X;
            if (_cmbSymbol.Text == "O")
                desiredSymbol = BoardSymbol.O;

            _txtIPAddress.Remember();
            if(_board != null)
            {
                _board.Reset(desiredSymbol);
                return;
            }
            _board = new UI.Controls.UIBoard(_txtIPAddress.Text, desiredSymbol);
            this.Controls.Add(_board);
        }

        

        
    }
}
