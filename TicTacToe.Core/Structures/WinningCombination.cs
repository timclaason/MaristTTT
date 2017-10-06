using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Structures
{
    public class WinningCombination
    {        
        public WinningCombination(int position1, int position2, int position3)
        {
            this.Position1 = position1;
            this.Position2 = position2;
            this.Position3 = position3;
        }

        public int Position1
        {
            get;
            set;
        }

        public int Position2
        {
            get;
            set;
        }

        public int Position3
        {
            get;
            set;
        }
    }

    public class AllWinningCombinations : List<WinningCombination>
    {
        public AllWinningCombinations()
        {            
            this.Add(new WinningCombination(0, 1, 2));
            this.Add(new WinningCombination(3, 4, 5));
            this.Add(new WinningCombination(6, 7, 8));
            this.Add(new WinningCombination(0, 3, 6));
            this.Add(new WinningCombination(1, 4, 7));
            this.Add(new WinningCombination(2, 5, 8));
            this.Add(new WinningCombination(0, 4, 8));
            this.Add(new WinningCombination(2, 4, 6));
        }
    }
}
