using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    
                case CellState.Blank:
                    return '.';
            }
            return '?';
        }
    }

    public enum CellState
    {
        /// <description>
        /// Клетка точно закрашена
        /// </description>
        Colored,

        /// <description>
        /// Точно незакрашена
        /// </description>
        Blank,

        /// <description>
        /// Неизвестно
        /// </description>
        Unknown
    }
}
   
    

