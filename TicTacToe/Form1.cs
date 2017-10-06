using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            
        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            if(_board != null)
            {
                _board.Reset();
                return;
            }
            _board = new UI.Controls.UIBoard(_txtIPAddress.Text);
            this.Controls.Add(_board);
        }

        
    }
}
