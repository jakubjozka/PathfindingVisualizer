using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using PathfindingVisualizer.Models;
using PathfindingVisualizer.Algorithms;

namespace PathfindingVisualizer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Grid Grid { get; private set; }

        private DrawMode _currentDrawMode;
        public DrawMode CurrentDrawMode
        {
            get => _currentDrawMode;
            set
            {
                _currentDrawMode = value;
                OnPropertyChanged(nameof(CurrentDrawMode));
            }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public MainViewModel()
        {
            Grid = new Grid(25, 50); // Changeable
            CurrentDrawMode = DrawMode.Wall;
            IsRunning = false;
        }

        public void HandleNodeClick(Node node)
        {
            if (IsRunning) return;

            switch (CurrentDrawMode)
            {
                case DrawMode.Start:
                    SetStartNode(node);
                    break;
                case DrawMode.End:
                    SetEndNode(node);
                    break;
                case DrawMode.Wall:
                    ToggleWall(node);
                    break;
                case DrawMode.Erase:
                    EraseNode(node);
                    break;
            }
        }

        private void SetStartNode(Node node)
        {
            if (Grid.StartNode != null)
            {
                Grid.StartNode.Type = NodeType.Empty;
            }

            node.Type = NodeType.Start;
            Grid.StartNode = node;
        }

        private void SetEndNode(Node node)
        {
            if (Grid.EndNode != null)
            {
                Grid.EndNode.Type = NodeType.Empty;
            }

            node.Type = NodeType.End;
            Grid.EndNode = node;
        }

        private void ToggleWall(Node node)
        {
            if (node.Type == NodeType.Start || node.Type == NodeType.End) return;

            node.Type = node.Type == NodeType.Wall ? NodeType.Empty : NodeType.Wall;
        }

        private void EraseNode(Node node)
        {
            if (node.Type == NodeType.Start)
            {
                Grid.StartNode = null;
            } 
            else if (node.Type == NodeType.End)
            {
                Grid.EndNode = null;
            }

            node.Type = NodeType.Empty;
        }

        public void ClearGrid()
        {
            if (IsRunning) return;
            Grid.ClearGrid();
        }

        public void ClearPath()
        {
            if (IsRunning) return;
            Grid.ResetForPathFinding();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task RunDijsktra()
        {
            if (Grid.StartNode == null || Grid.EndNode == null)
            {
                System.Windows.MessageBox.Show("Please place both Start and End nodes");
                return;
            }

            IsRunning = true;

            bool pathFound = await Dijkstra.FindPath(
                Grid, onNodeVisited: (node) => { }, delayMs: 10);

            IsRunning = false;

            if (!pathFound)
            {
                System.Windows.MessageBox.Show("No path found!");
            }
        }

        public async Task RunAStar()
        {
            if (Grid.StartNode == null || Grid.EndNode == null)
            {
                System.Windows.MessageBox.Show("Please place both Start and End nodes");
                return;
            }

            IsRunning = true;

            bool pathFound = await AStar.FindPath(
                Grid, onNodeVisited: (node) => { }, delayMs: 10);

            IsRunning = false;

            if (!pathFound)
            {
                System.Windows.MessageBox.Show("No path found!");
            }
        }
    }

    public enum DrawMode
    {
        Start,
        End,
        Wall,
        Erase
    }
}
