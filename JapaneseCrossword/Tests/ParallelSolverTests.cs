using System.IO;
using NUnit.Framework;

namespace JapaneseCrossword.Tests
{
    [TestFixture]
    public class ParallelSolverTests : CrosswordSolverTests
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            solver = new ParallelCrosswordSolver();
        }

        
    }
}
