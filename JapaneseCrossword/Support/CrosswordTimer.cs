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
        
        public void NoteTheTime(string inputFilePath)
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

            WatchOnSolver(iterateSolver, crossword);
            WatchOnSolver(parallelSolver, crossword);

        }

        private void WatchOnSolver(CrosswordSolverBase solver, Crossword inputCrossword)
        {
            var stopwatch = new Stopwatch();
            var crossword = inputCrossword;            
            stopwatch.Start();
            solver.SolveCrossword(crossword);           
            stopwatch.Stop();
            Console.WriteLine("{0} works {1} milliseconds", solver, stopwatch.ElapsedMilliseconds);
           
        }
    }
}
