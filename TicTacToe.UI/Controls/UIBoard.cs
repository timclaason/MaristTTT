using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.Core.Structures;

namespace TicTacToe.UI.Controls
{
    public class UIBoard : Panel
    {
        TicTacToe.Core.Structures.Board _underlyingBoard = null;
        TicTacToe.Core.Network.TicTacToeClient _client = null;
        TicTacToe.Core.Network.InputDevice _input = new Core.Network.InputDevice();

        
        const int BASE_X = 10;
        const int BASE_Y = 10;

        const int CONTROL_WIDTH = 310;
        const int CONTROL_HEIGHT = 310;

        const int CONTROL_DISTANCE = 99;

        string _ipAddress;


        public UIBoard(string ipAddress)
        {
            _ipAddress = ipAddress;
            initialize();
            startClient();
        }

        public void Reset()
        {
            foreach(Control c in this.Controls)
            {
                if(c is UIButton)
                {
                    ((UIButton)c).Reset();
                }
            }
            this.IsLocked = false;

            initialize();
            startClient();

            this.Refresh();
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
                UIButton targetButton = (UIButton)targetControl[0];

                targetButton.SetTextBasedOnSymbol(board.TileAt(b).Value);
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
                UIButton b = new UIButton(button);
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
                        _underlyingBoard.TileAt(int.Parse(b.Name)).SetValue(BoardSymbol.X);
                    }
                    this.IsLocked = true;
                    _input.CurrentValue = _underlyingBoard;
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

        

        private void startClient()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender2, e2) =>
            {
                _client = new TicTacToe.Core.Network.TicTacToeClient();
                _client.NewBoardReceived += (sender3, e3) =>
                {
                    if (sender3 is Board)
                    {
                        _underlyingBoard = (TicTacToe.Core.Structures.Board)sender3;
                        worker.ReportProgress(0, sender3);
                    }
                    if(sender3 is String)
                    {
                        if(sender3.ToString() == TicTacToe.Core.Network.NetworkMessages.GAME_OVER_CAT_SCRATCH ||
                            sender3.ToString() == TicTacToe.Core.Network.NetworkMessages.GAME_OVER_O_WINS ||
                            sender3.ToString() == TicTacToe.Core.Network.NetworkMessages.GAME_OVER_X_WINS
                            )
                        MessageBox.Show(sender3.ToString());
                    }
                };
                //_client.Start("192.168.1.3", _input);
                _client.Start(_ipAddress, _input);
            };
            worker.ProgressChanged += (sender4, e4) =>
            {
                if (e4.UserState is TicTacToe.Core.Structures.Board)
                {
                    this.Redraw((TicTacToe.Core.Structures.Board)e4.UserState);
                    this.IsLocked = false;
                }
            };
            worker.RunWorkerAsync();
        }

    }
}
 
