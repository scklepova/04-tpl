using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JapaneseCrossword
{
    class CrosswordReader
    {
        private readonly string inputFilePath;

        public CrosswordReader(string inputFilePath)
        {
            this.inputFilePath = inputFilePath;
        }

        public Crossword Read()
        {
            List<List<int>> rowsBlocks;
            List<List<int>> columnsBlocks;
            using (var reader = new StreamReader(inputFilePath))
            {   
                var rowsCount = int.Parse(reader.ReadLine().Split(':').ToList().ElementAt(1));
                rowsBlocks =
                    Enumerable.Range(0, rowsCount)
                        .Select(i => reader.ReadLine().Split(' ').Select(int.Parse).ToList())
                        .ToList();

                var columnsCount = int.Parse(reader.ReadLine().Split(':').ToList().ElementAt(1));
                columnsBlocks =
                    Enumerable.Range(0, columnsCount)
                        .Select(i => reader.ReadLine().Split(' ').Select(int.Parse).ToList())
                        .ToList();
            }

            return new Crossword(rowsBlocks, columnsBlocks);
        }
    }
}
