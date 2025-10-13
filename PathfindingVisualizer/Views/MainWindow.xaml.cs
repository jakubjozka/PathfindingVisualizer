using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void SetupEventHandlers()
        {
            RadioStart.Checked += (s, e) => ViewModel.CurrentDrawMode = DrawMode.Start;
            RadioEnd.Checked += (s, e) => ViewModel.CurrentDrawMode = DrawMode.End;
            RadioWall.Checked += (s, e) => ViewModel.CurrentDrawMode = DrawMode.Wall;
            RadioErase.Checked += (s, e) => ViewModel.CurrentDrawMode = DrawMode.Erase;

            BtnClearPath.Click += (s, e) => ViewModel.ClearPath();
            BtnClearGrid.Click += (s, e) => ViewModel.ClearGrid();

            BtnVisualize.Click += async (s, e) =>
            {
                var selected = (AlgorithmSelector.SelectedItem as ComboBoxItem)?.Content.ToString();

                switch (selected)
                {
                    case "Dijkstra":
                        await ViewModel.RunDijsktra();
                        break;
                    case "A*":
                        await ViewModel.RunAStar();
                        break;
                    default:
                        MessageBox.Show("Please select an algorithm.");
                        break;
                }
            };

            GridControl.MouseLeftButtonUp += (s, e) => _isMouseDown = false;
            GridControl.MouseLeave += (s, e) => _isMouseDown = false;
        }

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