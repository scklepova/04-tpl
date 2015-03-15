using System;

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
            }
            catch (Exception e)
            {
                return SolutionStatus.BadInputFilePath;
            }

            try
            {
                crossword.Solve();
            }
            catch (IncorrectCrosswordException e)
            {
                return SolutionStatus.IncorrectCrossword;
            }

            if(crossword.Incorrect)
                return SolutionStatus.IncorrectCrossword;
            

            try
            {
                var writer = new CrosswordWriter(outputFilePath);
                writer.Write(crossword);
            }
            catch (Exception e)
            {
                return SolutionStatus.BadOutputFilePath;
            }

            if (crossword.PartiallySolved())
                return SolutionStatus.PartiallySolved;
            return SolutionStatus.Solved;
            
        }
    }
}