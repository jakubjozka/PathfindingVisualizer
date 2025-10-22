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
    // ViewModel for the main application logic
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

        private int _animationDelay = 10;
        public int AnimationDelay
        {
            get => _animationDelay;
            set
            {
                _animationDelay = value;
                OnPropertyChanged(nameof(AnimationDelay));
            }
        }

        public bool CanRunSinglePath => !IsRunning && Grid.EndNodes.Count == 1;
        public bool CanRunMultiPath => !IsRunning && Grid.EndNodes.Count >= 1;


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
            if (node.Type == NodeType.End)
            {
                Grid.EndNodes.Remove(node);
                node.Type = NodeType.Empty;

                Grid.EndNode = Grid.EndNodes.Count > 0 ? Grid.EndNodes[0] : null;
            }
            else
            {
                node.Type = NodeType.End;
                Grid.EndNodes.Add(node);

                if (Grid.EndNode == null)
                {
                    Grid.EndNode = node;
                }
            }

            OnPropertyChanged(nameof(CanRunSinglePath));
            OnPropertyChanged(nameof(CanRunMultiPath));
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
            Grid.ResetForPathfinding();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task RunAlgorithm(IPathfindingAlgorithm algorithm)
        {
            if (Grid.StartNode == null)
            {
                System.Windows.MessageBox.Show("Please place a Start node!");
                return;
            }

            if (Grid.EndNodes.Count == 0)
            {
                System.Windows.MessageBox.Show("Please place at least one End node!");
                return;
            }

            IsRunning = true;

            bool pathFound = await algorithm.FindPathAsync(
                Grid, onNodeVisited: (node) => { }, delayMs: AnimationDelay);

            IsRunning = false;

            if (!pathFound)
            {
                System.Windows.MessageBox.Show($"No path found using {algorithm.AlgorithmName}!");
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
