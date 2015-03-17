using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace JapaneseCrossword
{
    [TestFixture]
    public class Line_should
    {


        [Test]
        public void Fail_if_correct_arrangement_doesnt_exist()
        {
            var blocks = new List<int>() {2, 3};
            var line = new Line(5, blocks);
            Assert.Catch(line.TryPlaceBlocks);
        }

        [Test]
        public void Be_empty_when_no_blocks()
        {
            var blocks = new List<int>();
            var line = new Line(5, blocks);
            line.TryPlaceBlocks();
            foreach (var cell in line.Cells)
            {
                Assert.AreEqual(cell.State, CellState.Empty);
            }
        }

        [Test]
        public void Change_if_any_cell_gain_final_state()
        {
            var blocks = new List<int>() { 2, 1 };
            var line = new Line(5, blocks);
            foreach (var cell in line.Cells)
            {
                cell.CanBeEmpty = true;
                cell.CanBeColored = true;
            }
            line.Cells[2].CanBeEmpty = true;
            line.Cells[2].CanBeColored = false;
            line.ChangeLine();
            Assert.True(line.wasChanged);
        }

        [Test]
        public void Fail_if_any_cell_gain_no_possible_state()
        {
            var blocks = new List<int>() { 2, 1 };
            var line = new Line(5, blocks);
            Assert.Catch(line.ChangeLine);
        }

        [Test]
        public void Change_if_all_arrangements_have_intersections()
        {
            var blocks = new List<int>() { 3 };
            var line = new Line(5, blocks);
            Assert.True(line.WasChanged());
            var correctCells = Enumerable.Range(0, 5).Select(i => new Cell()).ToList();
            correctCells[2].State = CellState.Colored;
            Enumerable.Range(0, 5).Select(i => Assert.Equals(line.Cells[i].State, correctCells[i].State));
        }

        [Test]
        public void Not_change_if_all_arrangements_have_intersections()
        {
            var blocks = new List<int>() { 2 };
            var line = new Line(5, blocks);
            Assert.False(line.WasChanged());
           
        }
    }
}