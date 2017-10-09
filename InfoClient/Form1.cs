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


namespace InfoClient
{
    public partial class InfoClientForm : Form
    {
        bool _connected = false;
        TicTacToe.Core.Network.InfoClient _client = new TicTacToe.Core.Network.InfoClient();

        public InfoClientForm()
        {
            
            InitializeComponent();
        }

        private void _btnSendRequest_Click(object sender, EventArgs e)
        {
            if (!_connected)
                _connected = _client.Connect(_txtIPAddress.Text);
            _txtIPAddress.Remember();

            if (!_connected)
            {
                MessageBox.Show("Error connecting");
                return;
            }

            string infoRequest = _cmbRequest.Text;

            if(infoRequest == String.Empty)
            {
                MessageBox.Show("Please select an option in the dropdown");
                return;
            }
            string response =  _client.SendInfoRequest(infoRequest);
        }

        private void InfoClientForm_Load(object sender, EventArgs e)
        {
            _txtIPAddress.Recall();
            _client.MessageReceived += (sender2, e2) =>
            {
                _txtOutput.Text += DateTime.Now.ToString() + " [Received Message]: " + sender2.ToString() + Environment.NewLine;
            };
            _client.MessageSent += (sender2, e2) =>
                {
                    _txtOutput.Text += DateTime.Now.ToString() + " [Sent Message]: " + sender2.ToString() + Environment.NewLine;
                };

        }
    }
}
