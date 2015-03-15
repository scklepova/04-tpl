using System;
using System.Collections.Generic;
using System.Linq;

namespace JapaneseCrossword
{
    public class CrosswordSolver : ICrosswordSolver
    {
        public SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            Crossword crossword;
            try
            {
                var reader = new CrosswordReader(inputFilePath);
                crossword = reader.Read();
                SolveCrossword(crossword);
                if (crossword.Incorrect)
                    return SolutionStatus.IncorrectCrossword;
            }
            catch (IncorrectCrosswordException e)
            {
                return SolutionStatus.IncorrectCrossword;
            }
            catch (Exception e)
            {
                return SolutionStatus.BadInputFilePath;
            }


            try
            {
                var writer = new CrosswordWriter(outputFilePath);
                writer.Write(crossword);
            }
            catch (Exception e)
            {
                return SolutionStatus.BadOutputFilePath;
            }

            return crossword.PartiallySolved() ? SolutionStatus.PartiallySolved : SolutionStatus.Solved;
        }

        private void SolveCrossword(Crossword crossword)
        {
            var needRefresh = true;
            while (needRefresh)
            {
                needRefresh = crossword.rows.Count(row => row.TryFillTheLine()) > 0;
                MergeResults(crossword.rows, crossword.columns);
                needRefresh = needRefresh || crossword.columns.Count(column => column.TryFillTheLine()) > 0;
                MergeResults(crossword.columns, crossword.rows);
            }
        }

        private void MergeResults(List<Line> from, List<Line> to)
        {
            for(var i = 0; i < from.Count; i++)
                for (var j = 0; j < from[i].Cells.Count; j++)
                    to[j].Cells[i] = from[i].Cells[j];
        }
        

    }
}