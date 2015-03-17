using System.Collections.Generic;
using System.Linq;

namespace JapaneseCrossword
{
    class Crossword
    {
        public List<Line> rows;
        public List<Line> columns; 
        public int rowsCount;
        public int columnsCount;
        
        
        

        public Crossword(List<List<int>> rowsBlocks, List<List<int>> columnsBlocks)
        {
            rowsCount = rowsBlocks.Count;
            columnsCount = columnsBlocks.Count;
            rows = Enumerable.Range(0, rowsCount)
                .Select( i => new Line(columnsCount, rowsBlocks[i]) )
                .ToList();

            columns = Enumerable.Range(0, columnsCount)
                .Select( i => new Line(rowsCount, columnsBlocks[i]))
                .ToList();
            
        }

        public bool PartiallySolved()
        {
            return rows.Any(row => row.Cells.Any(cell => cell.State == CellState.Unknown));
        }

        
    }
}
