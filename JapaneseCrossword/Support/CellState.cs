namespace JapaneseCrossword
{
    public static class Extensions
    {
        public static char ToSymbol(this CellState state)
        {
            switch (state)
            {
                case CellState.Colored:
                    return '*';
                    
                case CellState.Empty:
                    return '.';
            }
            return '?';
        }
    }

    public enum CellState
    {
        Colored,
        Empty,
        Unknown
    }
}
   
    

