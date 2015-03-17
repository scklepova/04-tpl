using System;
using System.Linq;
using JapaneseCrossword.Support;

namespace JapaneseCrossword
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new CrosswordTimer();
            timer.NoteTheTime(@"TestFiles\Flower.txt", 100);
        }

        
    }
}
