using System;
using System.Collections.Generic;
using System.Linq;
using JapaneseCrossword.Models;

namespace JapaneseCrossword
{
    public class Line
    {
        public List<Cell> Cells;
        public List<int> Blocks;
        public int size;
        public bool wasChanged;
        private Memorizer memorizer;

        public Line(int size, List<int> blocks)
        {
            try
            {
                Cells = Enumerable.Range(0, size).Select(i => new Cell()).ToList();
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new IncorrectCrosswordException();
            }
            Blocks = blocks;
            this.size = size;
            wasChanged = true;
            
        }

        public bool ExistsCorrectArrangment(int blockNumber, int startCell)
        {
            if (startCell + Blocks[blockNumber] > size) return false;
            if (EmptyCellInsideTheBlock(Blocks[blockNumber], startCell)) return false;
            if (FirstBlock(blockNumber) && ColoredCellBeforeTheBlock(startCell)) return false;

            if (!LastBlock(blockNumber))
            {
                var correctArrangementExists = false;
                memorizer.Memorize(startCell, blockNumber, PossibleStartState.Impossible);
                for (var nextStart = startCell + Blocks[blockNumber] + 1;
                    nextStart < Cells.Count - Blocks[blockNumber + 1] + 1;
                    nextStart++)
                {
                    if (memorizer.RememberState(nextStart, blockNumber + 1))
                    {
                        if (memorizer.GetState(nextStart, blockNumber + 1) == PossibleStartState.Impossible) continue;
                        UpdatePossibleStates(Blocks[blockNumber], startCell, nextStart);
                        if (FirstBlock(blockNumber))
                            PreviousCellsCanBeEmpty(startCell);
                        correctArrangementExists = true;
                    }
                    else
                    {
                        if (PreviousCellIsColored(nextStart)) break;
                        if (!ExistsCorrectArrangment(blockNumber + 1, nextStart))
                        {
                            memorizer.Memorize(nextStart, blockNumber + 1, PossibleStartState.Impossible);
                            continue;
                        }

                        correctArrangementExists = true;
                        memorizer.Memorize(nextStart, blockNumber + 1, PossibleStartState.Possible);
                        memorizer.Memorize(startCell, blockNumber, PossibleStartState.Possible);
                        UpdatePossibleStates(Blocks[blockNumber], startCell, nextStart);

                        if (FirstBlock(blockNumber))
                            PreviousCellsCanBeEmpty(startCell);
                    }
                }
                return correctArrangementExists;
            }


            if (ColoredCellsAfterTheBlock(Blocks[blockNumber], startCell))
            {
                memorizer.Memorize(startCell, blockNumber, PossibleStartState.Impossible);
                return false;
            }
            memorizer.Memorize(startCell, blockNumber, PossibleStartState.Possible);
            UpdatePossibleStates(Blocks[blockNumber], startCell, size);
            if (FirstBlock(blockNumber))
                PreviousCellsCanBeEmpty(startCell);
            return true;
        }

        private void PreviousCellsCanBeEmpty(int startCell)
        {
            UpdatePossibleStates(0, 0, startCell);
        }



        private bool PreviousCellIsColored(int nextStart)
        {
            return Cells[nextStart - 1].State == CellState.Colored;
        }



        private bool LastBlock(int blockNumber)
        {
            return blockNumber == Blocks.Count - 1;
        }



        private static bool FirstBlock(int blockNumber)
        {
            return blockNumber == 0;
        }




        private bool ColoredCellsAfterTheBlock(int blockSize, int startCell)
        {
            for (var i = startCell + blockSize; i < size; i++)
                if (Cells[i].State == CellState.Colored)
                    return true;
            return false;
        }



        private void UpdatePossibleStates(int blockSize, int start, int nextStart)
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
            memorizer = new Memorizer(size, Blocks.Count);
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
