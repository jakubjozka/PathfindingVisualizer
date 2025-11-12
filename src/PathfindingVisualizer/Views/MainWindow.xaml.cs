using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PathfindingVisualizer.Algorithms;
using PathfindingVisualizer.Models;
using PathfindingVisualizer.ViewModels;

namespace PathfindingVisualizer
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        private bool _isMouseDown = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid();
            SetupEventHandlers();
        }

        private void InitializeGrid()
        {
            List<Node> allNodes = new List<Node>();

            for (int row = 0; row < ViewModel.Grid.Rows; row++)
            {
                for (int col = 0; col < ViewModel.Grid.Cols; col++)
                {
                    allNodes.Add(ViewModel.Grid.Nodes[row, col]);
                }
            }

            GridControl.ItemsSource = allNodes;
        }

        // Setup event handlers for UI controls
        private void SetupEventHandlers()
        {
            RadioStart.Checked += (s, e) => ViewModel.CurrentDrawMode = DrawMode.Start;
            RadioEnd.Checked += (s, e) => ViewModel.CurrentDrawMode = DrawMode.End;
            RadioErase.Checked += (s, e) => ViewModel.CurrentDrawMode = DrawMode.Erase;

            RadioTerrain.Checked += (s, e) => UpdateTerrainMode();
            TerrainSelector.SelectionChanged += (s, e) =>
            {
                if (RadioTerrain.IsChecked == true)
                {
                    UpdateTerrainMode();
                }
            };

            SetAlgorithmToolTips();

            BtnClearPath.Click += (s, e) => ViewModel.ClearPath();
            BtnClearGrid.Click += (s, e) => ViewModel.ClearGrid();

            BtnVisualize.Click += async (s, e) =>
            {
                if (ViewModel.IsRunning)
                    return;

                var selected = (AlgorithmSelector.SelectedItem as ComboBoxItem)?.Content.ToString();

                IPathfindingAlgorithm? algorithm = selected switch
                {
                    "Dijkstra" => new Dijkstra(),
                    "A*" => new AStar(),
                    "BFS" => new BFS(),
                    "Multi-Target Dijkstra" => new MultiTargetDijkstra(),
                    "Bellman-Ford" => new BellmanFord(),
                    _ => null
                };

                if (algorithm == null)
                {
                    return;
                }

                bool isSinglePathAlgorithm = algorithm is Dijkstra or AStar or BFS;
                bool isMultiPathAlgorithm = algorithm is MultiTargetDijkstra or BellmanFord;
                int endNodeCount = ViewModel.Grid.EndNodes.Count;

                if (isSinglePathAlgorithm && endNodeCount != 1)
                {
                    MessageBox.Show($"{algorithm.AlgorithmName} requires exactly 1 End node. You have {endNodeCount}.");
                    return;
                }

                if (isMultiPathAlgorithm && endNodeCount < 1)
                {
                    MessageBox.Show($"{algorithm.AlgorithmName} requires at least 1 End node.");
                    return;
                }

                await ViewModel.RunAlgorithm(algorithm);
            };

            GridControl.MouseLeftButtonUp += (s, e) => _isMouseDown = false;
            GridControl.MouseLeave += (s, e) => _isMouseDown = false;
        }

        private void SetAlgorithmToolTips()
        {
            var algorithms = new Dictionary<string, IPathfindingAlgorithm>
            {
                { "Dijkstra", new Dijkstra() },
                { "A*", new AStar() },
                { "BFS", new BFS() },
                { "Multi-Target Dijkstra", new MultiTargetDijkstra() },
                { "Bellman-Ford", new BellmanFord() }
            };

            foreach (ComboBoxItem item in AlgorithmSelector.Items)
            {
                string? content = item.Content.ToString();
                if (content != null && algorithms.ContainsKey(content))
                {
                    item.ToolTip = algorithms[content].ToolTip;
                }
            }
        }
        private void UpdateTerrainMode()
        {
            var selected = (TerrainSelector.SelectedItem as ComboBoxItem)?.Content.ToString();

            ViewModel.CurrentDrawMode = selected switch
            {
                "Wall" => DrawMode.Wall,
                "Grass (2x)" => DrawMode.Grass,
                "Mud (5x)" => DrawMode.Mud,
                _ => DrawMode.Wall
            };
        }


        // Event handlers for node interactions
        private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;

            if (sender is Border border && border.DataContext is Node node)
            {
                ViewModel.HandleNodeClick(node);
            }
        }

        private void Node_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Border border && border.DataContext is Node node)
            {
                ViewModel.HandleNodeClick(node);
            }
        }
    }
}