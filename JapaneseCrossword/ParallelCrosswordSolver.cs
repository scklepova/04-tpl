using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace JapaneseCrossword
{
    class ParallelCrosswordSolver : ICrosswordSolver
    {
        public SolutionStatus Solve(string inputFilePath, string outputFilePath)
        {
            Crossword crossword;
            try
            {
                var reader = new CrosswordReader(inputFilePath);
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
                //needRefresh = crossword.rows.Count(row => row.TryFillTheLine()) > 0;
                
                var t = Task.WhenAll(crossword.rows.Select(row => Task.Run(() => row.TryFillTheLine())).ToArray());
                t.IgnoreExceptions().Wait();
                needRefresh = t.Result.Any(i => i);             
                MergeResults(crossword.rows, crossword.columns);
                //needRefresh = needRefresh || crossword.columns.Count(column => column.TryFillTheLine()) > 0;
                t = Task.WhenAll(crossword.columns.Select(column => Task.Run(() => column.TryFillTheLine())).ToArray());
                t.IgnoreExceptions().Wait();
                
                needRefresh = needRefresh || t.Result.Any(i => i);
                MergeResults(crossword.columns, crossword.rows);
            }
        }

        private void MergeResults(List<Line> from, List<Line> to)
        {
            for (var i = 0; i < from.Count; i++)
                for (var j = 0; j < from[i].Cells.Count; j++)
                    to[j].Cells[i] = from[i].Cells[j];
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
