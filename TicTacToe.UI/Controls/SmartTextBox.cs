using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe.UI.Controls
{
    public class SmartTextBox : TextBox
    {
        const string SERVER_FILE_NAME =  "TTTServer.txt";
        public void Remember()
        {
            writeToServerFile();
        }

        public void Recall()
        {
            if (File.Exists(SERVER_FILE_NAME))
            {
                string contents = File.ReadAllText(SERVER_FILE_NAME);
                this.Text = contents;
            }
        }

        private void writeToServerFile()
        {
            if (this.Text != String.Empty)
            {
                if (File.Exists(SERVER_FILE_NAME))
                    File.Delete(SERVER_FILE_NAME);
                File.WriteAllText(SERVER_FILE_NAME, this.Text);
            }
        }
    }
}
