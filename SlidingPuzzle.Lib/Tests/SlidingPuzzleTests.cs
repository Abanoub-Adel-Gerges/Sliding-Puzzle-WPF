using SlidingPuzzle.Lib.Models;
using SlidingPuzzle.Lib.Solvers;
using SlidingPuzzle.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlidingPuzzle.Lib.Logging;

namespace SlidingPuzzle.Lib.Tests
{
    public class SlidingPuzzleTests
    {
        public static List<Models.SlidingPuzzle> puzzles = new();
        public static ILogger _logger = new Logger("SlidingPuzzleTestsLogger.txt");
        public static void TestMove()
        {
            Models.SlidingPuzzle puzzle = new Models.SlidingPuzzle();
            _logger.Log("\n");
            _logger.Log(puzzle, LoggerTypes.Debug, includeHeader: false);
            var move = puzzle.GetLegalMoves()[0];
            puzzle.Move(move.Row, move.Col);
            _logger.Log($"Next Move {{Row = {move.Row}, Col = {move.Col}}}", LoggerTypes.Debug, includeHeader: false);
            _logger.Log(puzzle, LoggerTypes.Debug, includeHeader: false);
            WriteLine();
        }
        public static void TestAlgorithms(int shuffleCount = 100)
        {
            Models.SlidingPuzzle puzzle = new Models.SlidingPuzzle(3, 3, shuffleCount);
            _logger.Log("\n");
            _logger.Log(puzzle, LoggerTypes.Debug, includeHeader: false);

            SlidingPuzzleSolver solver = new SlidingPuzzleSolver();
            _logger.Log("UCS", LoggerTypes.Debug, includeHeader: false);
            PuzzleResult result = solver.UCS(puzzle);
            _logger.Log(result, LoggerTypes.Debug, includeHeader: false);
            _logger.Log(result.puzzle, LoggerTypes.Debug, includeHeader: false);

            _logger.Log("Greedy", LoggerTypes.Debug, includeHeader: false);
            result = solver.GreedySearch(puzzle);
            _logger.Log(result, LoggerTypes.Debug, includeHeader: false);
            _logger.Log(result.puzzle, LoggerTypes.Debug, includeHeader: false);

            _logger.Log("A Search", LoggerTypes.Debug, includeHeader: false);
            result = solver.ASearch(puzzle);
            _logger.Log(result, LoggerTypes.Debug, includeHeader: false);
            _logger.Log(result.puzzle, LoggerTypes.Debug, includeHeader: false);
            WriteLine();
        }
        public static void TestAlgorithmsPerformance(int trials = 100)
        {
            _logger.Log($"\nTrials = {trials}");
            puzzles = new List<Models.SlidingPuzzle>(trials);
            _logger.Log("Puzzles Generation time = " + CodeTimer.Measure(() =>
            {
                for (int i = 0; i < trials; i++) { puzzles.Add(new Models.SlidingPuzzle()); } // Same Tests for all algorithms
            }), LoggerTypes.Debug, includeHeader: false);
            var solver = new SlidingPuzzleSolver();
            TestAlgorithmPerformance(solver.UCS, "UCS", trials);
            TestAlgorithmPerformance(solver.GreedySearch, "Greedy", trials);
            TestAlgorithmPerformance(solver.ASearch, "A*", trials);
            WriteLine();
        }
        private static void TestAlgorithmPerformance(Func<Models.SlidingPuzzle, PuzzleResult> solverFunc, string name, int trials = 100)
        {
            TimeSpan timeTaken = CodeTimer.Measure(() =>
            {
                long totalMoves = 0, totalNodes = 0, solvedCount = 0;
                PuzzleResult result;
                for (int i = 0; i < trials; i++)
                {
                    result = solverFunc(puzzles[i]);
                    totalMoves += result.moves;
                    totalNodes += result.nodesExpanded;
                    solvedCount += (result.puzzle != null && result.puzzle.IsSolved()) ? 1 : 0;
                }
                _logger.Log($"{name,-10}{{Moves = {totalMoves}, Solved = {solvedCount}, Nodes = {totalNodes}}}"
                    , LoggerTypes.Debug, includeHeader: false);
            });
            _logger.Log($"Total time taken = {timeTaken}", LoggerTypes.Debug, includeHeader: false);
            _logger.Log($"Average time taken = {timeTaken / trials}", LoggerTypes.Debug, includeHeader: false);
            WriteLine(0);
        }
        public static void WriteLine(int size = 100)
        {
            size = Math.Max(size, 0);
            _logger.Log(new string('_', size), LoggerTypes.Debug, includeHeader: false);
        }
    }
}
