﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.Core.Network;

namespace TicTacToeServer
{
    public partial class Form1 : Form
    {
        BackgroundWorker _backgroundThread = new BackgroundWorker();
        TicTacToe.Core.Network.Server _server = new TicTacToe.Core.Network.Server();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            startupServer();
        }

        bool _handlersAttached = false;

        private void startupServer()
        {



            _backgroundThread = new BackgroundWorker();
            _backgroundThread.WorkerSupportsCancellation = true;
            _backgroundThread.WorkerReportsProgress = true;
            _backgroundThread.DoWork += (sender3, e3) =>
            {
                if (_handlersAttached == false)
                {
                    _server.MessageReceived += (sender2, e2) =>
                    {
                        _backgroundThread.ReportProgress(0, Environment.NewLine + "Received: " + Environment.NewLine + "=================================================================" + Environment.NewLine + sender2.ToString() + Environment.NewLine + "=================================================================");
                    };
                    _server.SocketOpened += (sender7, e7) =>
                        {
                            _backgroundThread.ReportProgress(0, "Socket Opened: " + sender7.ToString());
                        }; 
                    _server.MessageSent += (sender5, e5) =>
                    {
                        _backgroundThread.ReportProgress(0, Environment.NewLine + "Sent: " + Environment.NewLine + "=================================================================" + Environment.NewLine + sender5.ToString() + Environment.NewLine + "=================================================================");
                    };
                    _server.ClientConnected += (sender6, e6) =>
                    {
                        _backgroundThread.ReportProgress(0, "Client Connected: " + sender6.ToString());
                    };
                }

                _handlersAttached = true;
            


                _server.Start();
            };
            _backgroundThread.ProgressChanged += (sender4, e4) =>
            {
                _txtOutput.Text += e4.UserState.ToString() + Environment.NewLine;
            };
            _backgroundThread.RunWorkerAsync();
            
        }

        private void _btnRestart_Click(object sender, EventArgs e)
        {

            _server.KillListener();
            _backgroundThread.CancelAsync();
            _backgroundThread = null;

            _txtOutput.Text += "Restarted Server" + Environment.NewLine;

            startupServer();
        }
    }
}
