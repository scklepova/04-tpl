using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
