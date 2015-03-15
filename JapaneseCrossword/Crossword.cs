using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    class Crossword
    {
        public List<Line> rows, columns; 
        public int rowsCount;
        public int columnsCount;
        
        public bool Incorrect;
        

        public Crossword(List<List<int>> rowsBlocks, List<List<int>> columnsBlocks)
        {
            this.rowsCount = rowsBlocks.Count;
            this.columnsCount = columnsBlocks.Count;
            this.rows = Enumerable.Range(0, rowsCount)
                .Select(
                    i => new Line(
                        Enumerable.Range(0, columnsCount)
                            .Select(j => new Cell())
                            .ToList(),
                        rowsBlocks[i]
                        )
                ).ToList();

            this.columns = Enumerable.Range(0, columnsCount)
                .Select(
                    i => new Line(
                        Enumerable.Range(0, rowsCount)
                            .Select(j => new Cell())
                            .ToList(),
                        columnsBlocks[i]
                        )
                ).ToList();
            this.Incorrect = false;
        }

        public bool PartiallySolved()
        {
            return rows.Any(row => row.Cells.Any(cell => cell.State == CellState.Unknown));
        }

        
    }
}
