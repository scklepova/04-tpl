using System;
using System.Collections.Generic;
using System.IO;
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

//        [Test]
//        public void SuperBig()
//        {
//            var inputFilePath = @"TestFiles\SuperBig.txt";
//            var outputFilePath = Path.GetRandomFileName();
//           
//            var solutionStatus = solver.Solve(inputFilePath, outputFilePath);
//            Assert.AreEqual(SolutionStatus.Solved, solutionStatus);
//        }
    }
}
