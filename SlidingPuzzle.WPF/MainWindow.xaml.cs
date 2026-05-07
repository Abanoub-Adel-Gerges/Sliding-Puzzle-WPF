using SlidingPuzzle.Lib.Models;
using SlidingPuzzle.Lib.Solvers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SlidingPuzzle.WPF
{
    public partial class MainWindow : Window
    {
        private Lib.Models.SlidingPuzzle _currentPuzzle = new();
        private List<PuzzleMove>? _solutionPath;

        public MainWindow()
        {
            InitializeComponent();
            NewGame_Click(null, null); // Start with a game
        }
        private void NewGame_Click(object? sender, RoutedEventArgs? e)
        {
            if (!byte.TryParse(TxtH.Text, out byte h) || !byte.TryParse(TxtW.Text, out byte w)) return;
            // Range Validation [2, 4]
            if (h < 2 || h > 4 || w < 2 || w > 4)
            {
                MessageBox.Show("Please enter dimensions between 2 and 4.", "Invalid Size", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtH.Text = "3";
                TxtW.Text = "3";
                return;
            }
            _solutionPath = null;
            int shuffleCount = w * h * (w + h);
            _currentPuzzle = new Lib.Models.SlidingPuzzle(w, h, shuffleCount: shuffleCount);
            RefreshUI();
            ListPath.ItemsSource = null;
            TxtStats.Text = "Ready! Click tiles to move.";
        }

        private Border CreateTile(byte val, byte r, byte c, bool isHint)
        {
            var isEmpty = val == 0;
            Brush backgroundBrush = isEmpty ? Brushes.Transparent : new SolidColorBrush(Color.FromRgb(33, 150, 243));
            if (isHint) backgroundBrush = Brushes.OrangeRed;

            var border = new Border
            {
                Margin = new Thickness(4),
                CornerRadius = new CornerRadius(8),
                Background = backgroundBrush,
                Width = 80,
                Height = 80,
                Cursor = isEmpty ? Cursors.Arrow : Cursors.Hand,
                Tag = new Point(r, c)
            };

            if (!isEmpty)
            {
                border.MouseLeftButtonDown += Tile_Click;
                border.Child = new TextBlock
                {
                    Text = val.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 28,
                    FontWeight = FontWeights.ExtraBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            return border;
        }

        private async void Solve_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPuzzle == null) return;
            if (!int.TryParse(TxtTimeout.Text, out int maxSeconds) || maxSeconds < 1 || maxSeconds > 60)
            {
                MessageBox.Show("Please enter a timeout between 1 and 60 seconds.", "Invalid Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTimeout.Text = "5";
                return;
            }
            var senderButton = ((Button)sender);
            senderButton.IsEnabled = false;
            TxtStats.Text = "Solving... Please wait.";
            var solver = new SlidingPuzzleSolver();
            var puzzleCopy = new Lib.Models.SlidingPuzzle(_currentPuzzle.GetGrid());
            string algoTag = (CboAlgorithm.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "AStar";
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(maxSeconds));

            long memoryBefore = GC.GetTotalMemory(forceFullCollection: true), memoryAfter = 0;
            double memoryUsedMB = 0;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                dynamic result = await Task.Run(() =>
                {
                    return algoTag switch
                    {
                        "Greedy" => solver.GreedySearch(puzzleCopy, cts.Token),
                        "UCS" => solver.UCS(puzzleCopy, cts.Token),
                        _ => solver.ASearch(puzzleCopy, cts.Token)
                    };
                }, cts.Token);
                _solutionPath = result.Path;
                ListPath.ItemsSource = _solutionPath;
                TxtStats.Text = $"Algorithm: {((ComboBoxItem)CboAlgorithm.SelectedItem).Content}\n" +
                                $"Moves: {result.moves}\nNodes Expanded: {result.nodesExpanded}";
            }
            catch (OperationCanceledException)
            {
                TxtStats.Text = $"Search timed out after {maxSeconds}s! Try a smaller puzzle or different algorithm.";
                _solutionPath = null;
                ListPath.ItemsSource = null;
            }
            finally
            {
                sw.Stop();
                memoryAfter = GC.GetTotalMemory(false);
                memoryUsedMB = (memoryAfter - memoryBefore) / (1024 * 1024.0);
                TxtStats.Text += $"\nTime: {sw.ElapsedMilliseconds}ms\nMemory Used: {memoryUsedMB:F2} MB";
                senderButton.IsEnabled = true;
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            PuzzleGrid.Children.Clear();
            var grid = _currentPuzzle.GetGrid();
            byte h = _currentPuzzle.Height;
            byte w = _currentPuzzle.Width;

            PuzzleGrid.Rows = h;
            PuzzleGrid.Columns = w;
            PuzzleGrid.Width = w * 80;
            PuzzleGrid.Height = h * 80;

            PuzzleMove? hintMove = (_solutionPath != null && _solutionPath.Count > 0)
                           ? _solutionPath[0]
                           : null;

            for (byte r = 0; r < h; r++)
            {
                for (byte c = 0; c < w; c++)
                {
                    var val = grid[r][c];
                    bool isHint = hintMove.HasValue && hintMove.Value.Row == r && hintMove.Value.Col == c;
                    var border = CreateTile(val, r, c, isHint);
                    PuzzleGrid.Children.Add(border);
                }
            }
        }

        private void Tile_Click(object sender, MouseButtonEventArgs e)
        {
            Border? border = sender as Border;
            if(border == null) { return; }
            Point pos = (Point)border.Tag;

            byte clickedRow = (byte)pos.X;
            byte clickedCol = (byte)pos.Y;
            if (_currentPuzzle.Move((byte)pos.X, (byte)pos.Y))
            {
                if (_solutionPath != null && _solutionPath.Count > 0)
                {
                    var nextMove = _solutionPath[0];
                    if (nextMove.Row == clickedRow && nextMove.Col == clickedCol)
                    {
                        _solutionPath.RemoveAt(0);
                        ListPath.ItemsSource = null; // Force refresh of the ListBox
                        ListPath.ItemsSource = _solutionPath;
                    }
                    else
                    {
                        // If they moved a different tile, the old solution path is now invalid
                        _solutionPath = null;
                        ListPath.ItemsSource = null;
                    }
                }
                RefreshUI();
                if (_currentPuzzle.IsSolved())
                {
                    MessageBox.Show("Congratulations! You solved it!");
                }
            }
        }
    }
}