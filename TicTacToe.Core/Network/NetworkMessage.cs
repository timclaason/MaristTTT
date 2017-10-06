using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Structures;

namespace TicTacToe.Core.Network
{
    public class NetworkMessage
    {
        string _payload;

        const string COMPLEX_SEPARATOR = "@@@@@@@";

        public NetworkMessage(string message)
        {
            _payload = message;
        }

        public string RawMessage
        {
            get
            {
                return _payload;
            }
        }

        public bool MessageContainsGameOverIndicator
        {
            get
            {
                foreach(NetworkMessage m in this.Messages)
                {
                    if (m.MessageType != MessageTypes.Message)
                        continue;

                    if (m.RawMessage == NetworkMessages.GAME_OVER_CAT_SCRATCH ||
                                m.RawMessage == NetworkMessages.GAME_OVER_O_WINS ||
                                m.RawMessage == NetworkMessages.GAME_OVER_X_WINS)
                        return true;
                }
                return false;
            }
        }

        public List<NetworkMessage> Messages
        {
            get
            {
                List<NetworkMessage> messages = new List<NetworkMessage>();

                if (_payload.Contains(COMPLEX_SEPARATOR) == false)
                {
                    messages.Add(this);
                    return messages;
                }

                string[] parts = System.Text.RegularExpressions.Regex.Split(_payload, COMPLEX_SEPARATOR);

                foreach(String part in parts)
                {
                    NetworkMessage m = new NetworkMessage(part);
                    messages.Add(m);
                }

                return messages;               

            }
        }

        public Board Board
        {
            get
            {
                if (this.MessageType == MessageTypes.Message)
                    return null;
                return Board.Deserialize(_payload);
            }
        }

        public MessageTypes MessageType
        {
            get
            {
                if (_payload.Contains(COMPLEX_SEPARATOR))
                    return MessageTypes.Complex;
                if (_payload.Contains('<') && _payload.Contains('>'))
                    return MessageTypes.Message;
                if (_payload.Contains(':') && _payload.Contains(';') && _payload.Contains('0') && _payload.Contains('1'))
                    return MessageTypes.Board;
                return MessageTypes.Message;
            }
        }
    }
}
