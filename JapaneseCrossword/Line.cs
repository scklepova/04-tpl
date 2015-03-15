using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    class Line
    {
        public List<CellState> cells;
        public List<int> blocks; 
        private bool type;
        private bool[] canBeColored, canBeEmpty;
        public int size;
        public bool Incorrect;

        public Line(List<CellState> cells, List<int> blocks )
        {
            this.type = type;
            this.cells = cells;
            this.blocks = blocks;
            this.size = cells.Count;
            this.Incorrect = false;
        }

        public bool TryInstallBlock(int blockNumber, int startCell)
        {
            if (startCell + blocks[blockNumber] > size)
                return false;
            for (var i = startCell; i < startCell + blocks[blockNumber]; i++)
                if (cells[i] == CellState.Blank)
                    return false;

            if (blockNumber == 0)
            {
                for (var i = 0; i < startCell; i++)
                    if (cells[i] == CellState.Colored)
                        return false;
            }

            if (blockNumber < blocks.Count - 1)
            {
                bool result = false;
                for (var nextStart = startCell + blocks[blockNumber] + 1;
                    nextStart < cells.Count - blocks[blockNumber + 1] + 1;
                    nextStart++)
                {
                    if (cells[nextStart - 1] == CellState.Colored)
                        break;
                    if (TryInstallBlock(blockNumber + 1, nextStart))
                    {
                        result = true;
                        for (var i = startCell; i < startCell + blocks[blockNumber]; i++)
                            canBeColored[i] = true;
                        for (var i = startCell + blocks[blockNumber]; i < nextStart; i++)
                            canBeEmpty[i] = true;
                        if (blockNumber == 0)
                            for (var i = 0; i < startCell; i++)
                                canBeEmpty[i] = true;                       
                    }
                }
                return result;
            }
            else
            {
                for (var i = startCell + blocks[blockNumber]; i < cells.Count; i++)
                    if (cells[i] == CellState.Colored)
                        return false;
                for (var i = startCell; i < startCell + blocks[blockNumber]; i++)
                    canBeColored[i] = true;
                for (var i = startCell + blocks[blockNumber]; i < cells.Count; i++)
                    canBeEmpty[i] = true;
                if (blockNumber == 0)
                    for (var i = 0; i < startCell; i++)
                        canBeEmpty[i] = true;
                return true;
            }

        }


        public bool TryFillTheLine()
        {
            canBeColored = new bool[size];
            canBeEmpty = new bool[size];
            for (var i = 0; i < size; i++)
            {
                canBeColored[i] = false;
                canBeEmpty[i] = false;
            }

            //var critical = blocks.Sum() + blocks.Count - 1;
//            for (var start = 0; start <= size - blocks.Last(); start++)
//                TryInstallBlock(0, start);
            var existsCorrestArrangement =
                Enumerable.Range(0, size - blocks.Last() + 1).Count(i => TryInstallBlock(0, i)) > 0;
            if (!existsCorrestArrangement)
                throw new Exception();

            bool needRefresh = false;

            for (var i = 0; i < size; i++)
            {
                if (canBeColored[i] && !canBeEmpty[i] && cells[i] != CellState.Colored)
                {
                    cells[i] = CellState.Colored;
                    needRefresh = true;
                }
                if (!canBeColored[i] && canBeEmpty[i] && cells[i] != CellState.Blank)
                {
                    cells[i] = CellState.Blank;
                    needRefresh = true;
                }
                if (!canBeColored[i] && !canBeEmpty[i])
                {
                    //return false; тут что-то нужно делать, кроссворд плохой
                }
            }
            return needRefresh;
        }
    }
}
