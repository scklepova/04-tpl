using System.Collections.Generic;
using System.Linq;

namespace JapaneseCrossword
{
    public class Line
    {
        public List<Cell> Cells;
        public List<int> Blocks; 
        public int size;
        public  bool wasChanged;

        public Line(int size, List<int> blocks )
        {
            Cells = Enumerable.Range(0, size).Select(i => new Cell()).ToList();
            Blocks = blocks;
            this.size = size;
            wasChanged = true;
        }

        public bool ExistsCorrectArrangment(int blockNumber, int startCell)
        {
//            if (blockNumber == size)
//            {
//                if (ColoredCellBeforeTheBlock(startCell))
//                    return false;
//                return true;
//            }
                
            if (startCell + Blocks[blockNumber] > size)
                return false;
            if (EmptyCellInsideTheBlock(Blocks[blockNumber], startCell)) return false;
            if (blockNumber == 0 && ColoredCellBeforeTheBlock(startCell)) return false; ///
            
            if (blockNumber < Blocks.Count - 1)
            {
                var correctArrangementExists = false;
                for (var nextStart = startCell + Blocks[blockNumber] + 1;
                    nextStart < Cells.Count - Blocks[blockNumber + 1] + 1;
                    nextStart++)
                {
                    if (Cells[nextStart - 1].State == CellState.Colored)
                        break;
                    if (!ExistsCorrectArrangment(blockNumber + 1, nextStart)) continue;
                    correctArrangementExists = true;

                    RefreshPossibleStates(Blocks[blockNumber], startCell, nextStart);
                    if (blockNumber != 0) continue;

                    for (var i = 0; i < startCell; i++)
                        Cells[i].CanBeEmpty = true;
                }
                return correctArrangementExists;
            }
           

            if (ColoredCellsAfterTheBlock(Blocks[blockNumber], startCell)) return false;
            RefreshPossibleStates(Blocks[blockNumber], startCell, size);            
            if (blockNumber == 0)
                for (var i = 0; i < startCell; i++)
                    Cells[i].CanBeEmpty = true;
            return true;
        }


        private bool ColoredCellsAfterTheBlock(int blockSize, int startCell)
        {
            for (var i = startCell + blockSize; i < size; i++)
                if (Cells[i].State == CellState.Colored)
                    return true;
            return false;
        }

        private void RefreshPossibleStates(int blockSize, int start, int nextStart)
        {
            for (var i = start; i < start + blockSize; i++)
                Cells[i].CanBeColored = true;
            for (var i = start + blockSize; i < nextStart; i++)
                Cells[i].CanBeEmpty = true;
        }

        private bool ColoredCellBeforeTheBlock(int startCell)
        {
            for (var i = 0; i < startCell; i++)
                if (Cells[i].State == CellState.Colored)
                    return true;
            return false;
        }

        private bool EmptyCellInsideTheBlock(int blockSize, int startCell)
        {
            for (var i = startCell; i < startCell + blockSize; i++)
                if (Cells[i].State == CellState.Empty)
                    return true;
            return false;
        }


        public void TryPlaceBlocks()
        {
            ClearPossibleStates();
            if (Blocks.Count == 0)
            {
                MarkAllCellsEmpty();
                return;
            }
            var criticalSum = Blocks.Sum() + Blocks.Count - 1;
            var existsCorreсtArrangement =
                Enumerable.Range(0, size - criticalSum + 1).Count(i => ExistsCorrectArrangment(0, i)) > 0;
            if (!existsCorreсtArrangement)
                throw new IncorrectCrosswordException();
            
            ChangeLine();
        }

        private void MarkAllCellsEmpty()
        {
            foreach (var cell in Cells)
            {
                cell.State = CellState.Empty;
            }
            wasChanged = true;
        }

        public bool WasChanged()
        {
            TryPlaceBlocks();
            return wasChanged;
        }

        public void ChangeLine()
        {
            wasChanged = false;

            foreach (var cell in Cells)
            {
                if (cell.CanBeOnlyColored() && cell.State != CellState.Colored)
                {
                    cell.State = CellState.Colored;
                    wasChanged = true;
                }
                if (cell.CanBeOnlyEmpty() && cell.State != CellState.Empty)
                {
                    cell.State = CellState.Empty;
                    wasChanged = true;
                }
                if (!cell.CanHaveAnyState())
                {
                    throw new IncorrectCrosswordException();
                }
            }
            
        }


        private void ClearPossibleStates()
        {
            foreach (var cell in Cells)
            {
                cell.CanBeColored = false;
                cell.CanBeEmpty = false;
            }
        }
    }
}
