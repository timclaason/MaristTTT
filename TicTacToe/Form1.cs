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
        TicTacToe.UI.Controls.UIBoard _uiBoard;
        

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

            bool twoPlayer = _cmbSymbol.Text.Contains("2");
            

            _txtIPAddress.Remember();

			_uiBoard = null;
			foreach (Control c in this.Controls)
			{
				if (!(c is UI.Controls.UIBoard))
					continue;
				this.Controls.Remove(c);
			}
			this.Refresh();

			/*
            if(_uiBoard != null)
            {
                _uiBoard.Reset(desiredSymbol, twoPlayer);
                return;
            }
			*/
            _uiBoard = new UI.Controls.UIBoard(_txtIPAddress.Text, desiredSymbol, twoPlayer);
            this.Controls.Add(_uiBoard);
        }

        

        
    }
}
