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

        const string SERVER_FILE_NAME = "TTTServer.txt";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(File.Exists(SERVER_FILE_NAME))
            {
                string contents = File.ReadAllText(SERVER_FILE_NAME);
                _txtIPAddress.Text = contents;
            }
        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            writeToServerFile();
            if(_board != null)
            {
                _board.Reset();
                return;
            }
            _board = new UI.Controls.UIBoard(_txtIPAddress.Text);
            this.Controls.Add(_board);
        }

        private void writeToServerFile()
        {
            if(_txtIPAddress.Text != String.Empty)
            {
                if (File.Exists(SERVER_FILE_NAME))
                    File.Delete(SERVER_FILE_NAME);
                File.WriteAllText(SERVER_FILE_NAME, _txtIPAddress.Text);
            }
        }

        
    }
}
