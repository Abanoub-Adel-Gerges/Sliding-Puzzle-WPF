using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Lib.Models
{
    public class SlidingPuzzle
    {
        private byte[][] grid;
        private byte currentRow, currentCol; // Location of the empty square (0)
        private byte width, height;
        private static readonly Random _rng = new Random();

        public SlidingPuzzle(byte width = 3, byte height = 3, int shuffleCount = 100)
        {
            this.width = width;
            this.height = height;
            grid = new byte[this.height][];
            for (int i = 0; i < height; i++) grid[i] = new byte[this.width];
            InitializeSolvedState();
            Shuffle(shuffleCount);
        }
        public SlidingPuzzle(byte[][] initialGrid)
        {
            this.height = (byte)initialGrid.Length;
            this.width = (byte)initialGrid[0].Length;
            grid = new byte[height][];
            for (int i = 0; i < height; i++)
            {
                grid[i] = new byte[width];
                for (int j = 0; j < width; j++)
                {
                    grid[i][j] = initialGrid[i][j];
                    if (grid[i][j] == 0)
                    {
                        currentRow = (byte)i;
                        currentCol = (byte)j;
                    }
                }
            }
        }
        private void InitializeSolvedState()
        {
            byte counter = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == height - 1 && j == width - 1) grid[i][j] = 0;
                    else grid[i][j] = counter++;
                }
            }
            currentRow = (byte)(height - 1);
            currentCol = (byte)(width - 1);
        }
        public void Shuffle(int iterations = 100)
        {
            for (int i = 0; i < iterations; i++)
            {
                var moves = GetLegalMoves();
                var randomMove = moves[_rng.Next(moves.Count)];
                Move(randomMove.Row, randomMove.Col);
            }
        }
        public byte[][] GetGrid() { return grid; }
        public byte Width => width;
        public byte Height => height;
        public long GetGridSerialize()
        {
            long key = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    key = (key << 4) | grid[i][j];
                }
            }
            return key;
        }
        public bool Move(byte newR, byte newC)
        {
            if (!IsLegalMove(newR, newC)) { return false; }
            grid[currentRow][currentCol] = grid[newR][newC];
            grid[newR][newC] = 0;
            currentRow = newR;
            currentCol = newC;
            return true;
        }
        public bool IsSolved()
        {
            byte counter = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == height - 1 && j == width - 1) { return grid[i][j] == 0; }
                    if (grid[i][j] != counter++) { return false; }
                }
            }
            return true;
        }
        public bool IsLegalMove(byte r, byte c)
        {
            if (r >= height || c >= width) return false;
            int dr = Math.Abs(currentRow - r);
            int dc = Math.Abs(currentCol - c);
            return dr + dc == 1;
        }
        public List<PuzzleMove> GetLegalMoves()
        {
            var moves = new List<PuzzleMove>();
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nr = currentRow + dx[i];
                int nc = currentCol + dy[i];
                if (nr >= 0 && nr < height && nc >= 0 && nc < width)
                {
                    moves.Add(new PuzzleMove((byte)nr, (byte)nc));
                }
            }
            return moves;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string border = new string('-', (width * 4) + 1);
            sb.AppendLine(border);
            for (int i = 0; i < height; i++)
            {
                sb.Append("| ");
                for (int j = 0; j < width; j++)
                {
                    string val = grid[i][j] == 0 ? " " : grid[i][j].ToString();
                    sb.Append(val + " | ");
                }
                sb.AppendLine("\n" + border);
            }
            return sb.ToString();
        }
    }
}
