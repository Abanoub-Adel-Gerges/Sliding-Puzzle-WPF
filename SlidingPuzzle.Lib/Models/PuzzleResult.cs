using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Lib.Models
{
    public class PuzzleResult
    {
        public SlidingPuzzle? puzzle;
        public List<PuzzleMove> Path { get; set; } = new();
        public int moves = 0, nodesExpanded = 0;
        public override string ToString()
        {
            var pathStr = string.Join(" -> ", Path);
            return $"IsSolved = {puzzle?.IsSolved()}\nMoves = {moves}\nNodes Expanded = {nodesExpanded}\nPath = {pathStr}";
        }
    }
}
