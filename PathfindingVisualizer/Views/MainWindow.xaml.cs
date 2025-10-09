using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PathfindingVisualizer.Models;
using PathfindingVisualizer.ViewModels;

namespace PathfindingVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;
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
            BtnVisualize.Click += async (s, e) => await ViewModel.RunDijsktra();
        }

        private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Node node)
            {
                ViewModel.HandleNodeClick(node);
            }
        }
    }
}