using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JapaneseCrossword.Support
{
    class CrosswordTimer
    {
        
        public void NoteTheTime(string inputFilePath, int timesToRepeat)
        {
            var iterateSolver = new CrosswordSolver();
            var parallelSolver = new ParallelCrosswordSolver();
            var reader = new CrosswordReader(inputFilePath);

            WatchOnSolver(iterateSolver, reader, timesToRepeat);
            WatchOnSolver(parallelSolver, reader, timesToRepeat);

        }

        private void WatchOnSolver(CrosswordSolverBase solver, CrosswordReader reader, int timesToRepeat)
        {
            var crosswords = new List<Crossword>();
            try
            {
                crosswords = Enumerable.Range(0, timesToRepeat).Select(i => reader.Read()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Timer is working...");
            var stopwatch = new Stopwatch();            
            stopwatch.Start();
            crosswords.ForEach(solver.SolveCrossword);
            stopwatch.Stop();
            Console.WriteLine("{0} works {1} milliseconds on {2} times", solver, stopwatch.ElapsedMilliseconds, timesToRepeat);
           
        }
    }
}
