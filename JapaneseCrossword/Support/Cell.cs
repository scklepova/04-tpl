

namespace JapaneseCrossword
{
    public class Cell
    {
        public CellState State;
        public bool CanBeColored;
        public bool CanBeEmpty;

        public Cell()
        {
            State = CellState.Unknown;
            CanBeColored = false;
            CanBeEmpty = false;
        }

        public bool CanBeOnlyColored()
        {
            return CanBeColored && !CanBeEmpty;
        }

        public bool CanBeOnlyEmpty()
        {
            return CanBeEmpty && !CanBeColored;
        }

        public bool CanHaveAnyState()
        {
            return CanBeColored || CanBeEmpty;
        }
    }
}
