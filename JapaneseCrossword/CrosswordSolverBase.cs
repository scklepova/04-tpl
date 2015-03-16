using System.Collections.Generic;

namespace JapaneseCrossword
{
    public abstract class CrosswordSolverBase
    {
        public abstract SolutionStatus Solve(string inputFilePath, string outputFilePath);

        public void MergeResults(List<Line> from, List<Line> to)
        {
            for (var i = 0; i < from.Count; i++)
                for (var j = 0; j < from[i].Cells.Count; j++)
                    to[j].Cells[i] = from[i].Cells[j];
        }

    }
}