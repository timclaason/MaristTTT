using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace TicTacToe.Core.Structures
{
    [Serializable]
    public class Board
    {
        const int MIN_TILE = 0;
        const int MAX_TILE = 8;
        public List<BoardTile> Tiles;

        static string X_TRANSLATION = "X";
        static string O_TRANSLATION = "O";
        static string BLANK_TRANSLATION = "B";

        AllWinningCombinations _winningCombinations = new AllWinningCombinations();
        
        public Board()
        {
            initialize();
        }

        private void initialize()
        {
            Tiles = new List<BoardTile>();

            for (int tileNumber = MIN_TILE; tileNumber <= MAX_TILE; tileNumber++)
                Tiles.Add(new BoardTile(tileNumber));
        }
        
        public bool BoardIsCatscratch()
        {
            return this.BoardIsFull() && this.DetectWinner() == BoardSymbol.Blank;
        }

        public bool BoardIsFull()
        {
            int occupiedCount = 0;

            foreach(BoardTile t in Tiles)
            {
                if (t.IsOccupied)
                    occupiedCount++;
            }
            return occupiedCount == 9;

        }


        public BoardSymbol DetectWinner()
        {
            foreach(WinningCombination combination in _winningCombinations)
            {
                if (!TileAt(combination.Position1).IsOccupied ||
                    !TileAt(combination.Position2).IsOccupied ||
                    !TileAt(combination.Position3).IsOccupied)
                    continue;

                if (TileAt(combination.Position1).Value == TileAt(combination.Position2).Value && TileAt(combination.Position1).Value == TileAt(combination.Position3).Value)
                    return TileAt(combination.Position1).Value;
            }

            return BoardSymbol.Blank;
        }
        

        public BoardTile TileAt(int index)
        {
            if (index < MIN_TILE || index > MAX_TILE)
                throw new IndexOutOfRangeException();

            return Tiles[index];

        }

        public String SerializedString
        {
            get
            {
                try { return this.SerializeObject(); }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public string SerializeObject()
        {
            string returnString = String.Empty;

            for (int i = MIN_TILE; i <= MAX_TILE; i++ )
            {
                BoardTile tile = this.TileAt(i);

                string currentRecord = i.ToString() + ":";

                if (tile.Value == BoardSymbol.X)
                    currentRecord += Board.X_TRANSLATION;
                if (tile.Value == BoardSymbol.O)
                    currentRecord += Board.O_TRANSLATION;
                if (tile.Value == BoardSymbol.Blank)
                    currentRecord += Board.BLANK_TRANSLATION;
                returnString += currentRecord;

                if (i < MAX_TILE)
                    returnString += ";";
            }

            return returnString;
        }

        

        public static Board Deserialize(string input)
        {
            Board returnObject = new Board();

            string[] tiles = input.Split(';');

            foreach(string tile in tiles)
            {
                if (tile.Length < 3)
                    continue;
                int index = -1;
                bool parsed = int.TryParse(tile.Substring(0, 1), out index);

                if(index > -1)
                {
                    string value = tile.Substring(2, 1);

                    if(value == Board.X_TRANSLATION)
                    {
                        returnObject.TileAt(index).SetValue(BoardSymbol.X);
                    }
                    if(value == Board.O_TRANSLATION)
                    {
                        returnObject.TileAt(index).SetValue(BoardSymbol.O);
                    }
                }
            }

            return returnObject;

        }
    }
}
