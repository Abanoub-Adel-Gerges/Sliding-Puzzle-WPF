using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Lib.Models
{
    public class PuzzleStateNode
    {
        public SlidingPuzzle State { get; }
        public PuzzleStateNode? Parent { get; }
        public PuzzleMove? LastMove { get; }
        public int G { get; } // Cost from start to current node
        public int H { get; } // Estimated cost to goal
        public int F => G + H; // Total cost
        public PuzzleStateNode(SlidingPuzzle state, PuzzleStateNode? parent, PuzzleMove? lastMove, int g, int h)
        {
            State = state;
            Parent = parent;
            LastMove = lastMove;
            G = g;
            H = h;
        }
        // Used by PriorityQueue to pick the "best" node
        public int CompareTo(PuzzleStateNode? other)
        {
            if (other == null) { return 1; }
            return this.F.CompareTo(other.F);
        }
    }
}
