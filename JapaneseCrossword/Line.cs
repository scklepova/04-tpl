using System.Collections.Generic;
using System.Linq;

namespace JapaneseCrossword
{
    public class Line
    {
        public List<Cell> Cells;
        public List<int> Blocks; 
        public int size;
        private bool wasChanged;

        public Line(List<Cell> cells, List<int> blocks )
        {
            Cells = cells;
            Blocks = blocks;
            size = cells.Count;
            wasChanged = true;
        }

        public bool TryInstallBlock(int blockNumber, int startCell)
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
                    if (TryInstallBlock(blockNumber + 1, nextStart))
                    {
                        correctArrangementExists = true;
                        RefreshPossibleStates(Blocks[blockNumber], startCell, nextStart);
                        if (blockNumber == 0)
                            for (var i = 0; i < startCell; i++)
                                Cells[i].CanBeEmpty = true;                       
                    }
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


        public void TryFillTheLine()
        {
            InitializeCanBeMassives();
            var criticalSum = Blocks.Sum() + Blocks.Count - 1;
            var existsCorreсtArrangement =
                Enumerable.Range(0, size - criticalSum + 1).Count(i => TryInstallBlock(0, i)) > 0;
            if (!existsCorreсtArrangement)
                throw new IncorrectCrosswordException();
            
            RefreshLine();
        }

        public bool WasChanged()
        {
            TryFillTheLine();
            return wasChanged;
        }

        private void RefreshLine()
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


        private void InitializeCanBeMassives()
        {
            foreach (var cell in Cells)
            {
                cell.CanBeColored = false;
                cell.CanBeEmpty = false;
            }
        }
    }
}
