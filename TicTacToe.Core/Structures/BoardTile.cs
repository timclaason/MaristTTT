using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Structures
{
    public class BoardTile
    {
        int _tileNumber = 0;

        BoardSymbol _currentValue = BoardSymbol.Blank;

        public BoardTile(int tile)
        {
            _tileNumber = tile;
        }

        public bool IsOccupied
        {
            get
            {
                return _currentValue != BoardSymbol.Blank;
            }
        }

        public BoardSymbol Value
        {
            get
            {
                return _currentValue;
            }
        }

        public bool SetValue(BoardSymbol symbol)
        {
            if (_currentValue == BoardSymbol.Blank)
            {
                _currentValue = symbol;
                return true;
            }
            return false;
        }
    }
}
