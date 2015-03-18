namespace JapaneseCrossword.Models
{
    public class Memorizer
    {
        private PossibleStartState[,] possibleBlocksStarts;

        public Memorizer(int cellsNumber, int blocksNumber)
        {
            possibleBlocksStarts = new PossibleStartState[cellsNumber, blocksNumber];
            possibleBlocksStarts.Initialize();
        }

        public void Memorize(int startCell, int blockNumber, PossibleStartState state)
        {
            possibleBlocksStarts[startCell, blockNumber] = state;
        }

        public bool RememberState(int startCell, int blockNumber)
        {
            return possibleBlocksStarts[startCell, blockNumber] != PossibleStartState.Unknown;
        }

        public PossibleStartState GetState(int startCell, int blockNumber)
        {
            return possibleBlocksStarts[startCell, blockNumber];
        }
    }
}
