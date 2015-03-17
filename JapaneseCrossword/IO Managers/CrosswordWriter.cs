using System;
using System.IO;

namespace JapaneseCrossword
{
    class CrosswordWriter
    {
        private readonly string outputFilePath;

        public CrosswordWriter(string outputFilePath)
        {
            this.outputFilePath = outputFilePath;
        }

        public void Write(Crossword crossword)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var row in crossword.rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        writer.Write(cell.State.ToSymbol());
                    }
                    writer.Write(Environment.NewLine);
                }
            }
        }

        
    }
}
