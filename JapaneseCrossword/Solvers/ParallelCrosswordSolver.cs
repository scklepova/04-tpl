using System;
using System.Linq;
using System.Threading.Tasks;

namespace JapaneseCrossword
{
    class ParallelCrosswordSolver : CrosswordSolverBase
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
            catch (AggregateException e)
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
                var t = Task.WhenAll(crossword.rows.Select(row => Task.Run(() => row.WasChanged())).ToArray());
                t.IgnoreExceptions().Wait();
                needRefresh = t.Result.Any(i => i);             
                MergeResults(crossword.rows, crossword.columns);
               
                t = Task.WhenAll(crossword.columns.Select(column => Task.Run(() => column.WasChanged())).ToArray());
                t.IgnoreExceptions().Wait();
                
                needRefresh = needRefresh || t.Result.Any(i => i);
                MergeResults(crossword.columns, crossword.rows);
            }
        }

    }

    public static class TaskExtensions
    {
        public static Task IgnoreExceptions(this Task t)
        {
            return t.ContinueWith(_ => { });
        }
    }
}
