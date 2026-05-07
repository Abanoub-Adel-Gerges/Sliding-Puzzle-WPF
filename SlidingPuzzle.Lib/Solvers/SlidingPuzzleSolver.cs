using SlidingPuzzle.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Lib.Solvers
{
    public class SlidingPuzzleSolver
    {
        private int CalculateH(Models.SlidingPuzzle state)
        {
            int distance = 0;
            var grid = state.GetGrid();
            int h = state.Height;
            int w = state.Width;

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    byte val = grid[i][j];
                    if (val == 0) continue;
                    int targetRow = (val - 1) / w;
                    int targetCol = (val - 1) % w;
                    distance += Math.Abs(i - targetRow) + Math.Abs(j - targetCol);
                }
            }

            return distance;
        }
        private byte[][] CopyGrid(Models.SlidingPuzzle state)
        {
            byte[][] source = state.GetGrid();
            int h = state.Height;
            int w = state.Width;

            byte[][] target = new byte[h][];

            for (int i = 0; i < h; i++)
            {
                target[i] = new byte[w];
                Array.Copy(source[i], target[i], w);
            }
            return target;
        }
        private List<PuzzleMove> ReconstructPath(PuzzleStateNode? node)
        {
            var path = new List<PuzzleMove>();
            while (node != null && node.LastMove.HasValue)
            {
                path.Add(node.LastMove.Value);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
        private PuzzleResult RunSearch(Models.SlidingPuzzle initial, Func<int, int, int> priorityFunc, CancellationToken? token = null)
        {
            var pq = new PriorityQueue<PuzzleStateNode, int>();
            var visited = new HashSet<long>(); // Note: long only supports up to 16 tiles (4x4)
            int expandedCount = 0;

            var start = new PuzzleStateNode(initial, null, null, 0, CalculateH(initial));
            pq.Enqueue(start, priorityFunc(start.G, start.H));

            PuzzleStateNode? node = null;
            while (pq.Count > 0)
            {
                if (token != null && token.Value.IsCancellationRequested) { token?.ThrowIfCancellationRequested(); }

                node = pq.Dequeue();
                expandedCount++;

                long key = node.State.GetGridSerialize();
                if (!visited.Add(key))
                    continue;

                if (node.State.IsSolved())
                {
                    return new PuzzleResult
                    {
                        puzzle = node.State,
                        Path = ReconstructPath(node),
                        moves = node.G,
                        nodesExpanded = expandedCount
                    };
                }

                foreach (var move in node.State.GetLegalMoves())
                {
                    // Create a deep copy of the current grid to simulate the next state
                    var gridCopy = CopyGrid(node.State);
                    var nextState = new Models.SlidingPuzzle(gridCopy);

                    nextState.Move(move.Row, move.Col);

                    long nextKey = nextState.GetGridSerialize();
                    if (visited.Contains(nextKey))
                        continue;

                    int g = node.G + 1;
                    int h = CalculateH(nextState);

                    var nextNode = new PuzzleStateNode(nextState, node, move, g, h);
                    pq.Enqueue(nextNode, priorityFunc(g, h));
                }
            }

            return new PuzzleResult
            {
                puzzle = node?.State,
                moves = (node == null) ? 0 : node.G,
                nodesExpanded = expandedCount
            };
        }
        public PuzzleResult UCS(Models.SlidingPuzzle initial)
        {
            return RunSearch(initial, (g, h) => g);
        }

        public PuzzleResult GreedySearch(Models.SlidingPuzzle initial)
        {
            return RunSearch(initial, (g, h) => h);
        }

        public PuzzleResult ASearch(Models.SlidingPuzzle initial)
        {
            return RunSearch(initial, (g, h) => g + h);
        }
        public PuzzleResult UCS(Models.SlidingPuzzle initial, CancellationToken token)
        {
            return RunSearch(initial, (g, h) => g, token);
        }

        public PuzzleResult GreedySearch(Models.SlidingPuzzle initial, CancellationToken token)
        {
            return RunSearch(initial, (g, h) => h, token);
        }

        public PuzzleResult ASearch(Models.SlidingPuzzle initial, CancellationToken token)
        {
            return RunSearch(initial, (g, h) => g + h, token);
        }
    }
}
