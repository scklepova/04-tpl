using System;
using System.Diagnostics;

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            WatchOnSolver(iterateSolver, crossword, timesToRepeat);
            WatchOnSolver(parallelSolver, crossword, timesToRepeat);

        }

        private void WatchOnSolver(CrosswordSolverBase solver, Crossword crossword, int timesToRepeat)
        {
            
            Console.WriteLine("Timer is working...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < timesToRepeat; i++)
            {
                solver.SolveCrossword(crossword);
                crossword.Clean();
            }
            stopwatch.Stop();
            Console.WriteLine("{0} works {1} milliseconds on {2} times", solver, stopwatch.ElapsedMilliseconds, timesToRepeat);
           
        }

        
    }
}
