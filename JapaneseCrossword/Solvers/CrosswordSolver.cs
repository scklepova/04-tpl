using System;
using System.Linq;

namespace JapaneseCrossword
{
    public class CrosswordSolver : CrosswordSolverBase
    {
        public override SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            Crossword crossword;
            var reader = new CrosswordReader(inputFilePath);
            try
            {               
                crossword = reader.Read();
                SolveCrossword(crossword);
            }
            catch (IncorrectCrosswordException e)
            {
                return SolutionStatus.IncorrectCrossword;
            }
            catch (Exception e)
            {
                return SolutionStatus.BadInputFilePath;
            }

            var writer = new CrosswordWriter(outputFilePath);
            try
            {              
                writer.Write(crossword);
            }
            catch (Exception e)
            {
                return SolutionStatus.BadOutputFilePath;
            }

            return crossword.PartiallySolved() ? SolutionStatus.PartiallySolved : SolutionStatus.Solved;
        }

        public override void SolveCrossword(Crossword crossword)
        {
            var needRefresh = true;
            while (needRefresh)
            {
                needRefresh = crossword.rows.Count(row => row.WasChanged()) > 0;
                MergeResults(crossword.rows, crossword.columns);
                needRefresh = crossword.columns.Count(column => column.WasChanged()) > 0 || needRefresh;
                MergeResults(crossword.columns, crossword.rows);
            }
        }

       
        

    }
}