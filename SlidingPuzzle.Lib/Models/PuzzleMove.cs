using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Lib.Models
{
    public readonly struct PuzzleMove
    {
        public byte Row { get; }
        public byte Col { get; }

        public PuzzleMove(byte row, byte col)
        {
            Row = row;
            Col = col;
        }
        public override string ToString() => $"({Row}, {Col})";
    }
}
