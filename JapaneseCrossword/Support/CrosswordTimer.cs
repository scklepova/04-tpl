using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseCrossword.Support
{
    class CrosswordTimer
    {
        
        public void NoteTheTime(string inputFilePath, int timesToRepeat)
        {
            var iterateSolver = new CrosswordSolver();
            var parallelSolver = new ParallelCrosswordSolver();
            var reader = new CrosswordReader(inputFilePath);
            Crossword crossword;
            try
            {
                crossword = reader.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            WatchOnSolver(iterateSolver, crossword, timesToRepeat);
            WatchOnSolver(parallelSolver, crossword, timesToRepeat);

        }

        private void WatchOnSolver(CrosswordSolverBase solver, Crossword crossword, int timesToRepeat)
        {
            var stopwatch = new Stopwatch();            
            stopwatch.Start();
            for (int i = 0; i < timesToRepeat; i++)
            {
                solver.SolveCrossword(crossword);
                CleanStates(crossword);
            }          
            stopwatch.Stop();
            Console.WriteLine("{0} works {1} milliseconds on {2} times", solver, stopwatch.ElapsedMilliseconds, timesToRepeat);
           
        }

        private void CleanStates(Crossword crossword)
        {
            crossword.rows.Select(row => row.Cells.Select(cell => cell.State = CellState.Unknown)).ToArray();
            crossword.columns.Select(column => column.Cells.Select(cell => cell.State = CellState.Unknown)).ToArray();
        }
    }
}
