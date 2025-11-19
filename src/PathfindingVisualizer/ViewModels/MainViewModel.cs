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

        private int _nodesExplored;
        public int NodesExplored
        { 
            get => _nodesExplored;
            set
            {
                _nodesExplored = value;
                OnPropertyChanged(nameof(NodesExplored));
            }
        }

        private int _pathLength;
        public int PathLength
        {
            get => _pathLength;
            set
            {
                _pathLength = value;
                OnPropertyChanged(nameof(PathLength));
            }
        }

        private double _timeTaken;
        public double TimeTaken
        {
            get => _timeTaken;
            set
            {
                _timeTaken = value;
                OnPropertyChanged(nameof(TimeTaken));
            }
        }

        private int _pathCost;
        public int PathCost
        {
            get => _pathCost;
            set
            {
                _pathCost = value;
                OnPropertyChanged(nameof(PathCost));
            }
        }

        private string _currentAlgorithm = "None";
        public string CurrentAlgorithm
        {
            get => _currentAlgorithm;
            set
            {
                _currentAlgorithm = value;
                OnPropertyChanged(nameof(CurrentAlgorithm));
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
                case DrawMode.Grass:
                    SetTerrain(node, NodeType.Grass, 2);
                    break;
                case DrawMode.Mud:
                    SetTerrain(node, NodeType.Mud, 5);
                    break;
                case DrawMode.Erase:
                    EraseNode(node);
                    break;
            }
        }

        private void SetTerrain(Node node, NodeType terraingType, int weight)
        {
            if (node.Type == NodeType.Start || node.Type == NodeType.End) return;

            node.Type = terraingType;
            node.BaseTerrainType = terraingType;
            node.Weight = weight;
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

            if (node.Type == NodeType.Wall)
            {
                node.Type = NodeType.Empty;
                node.Weight = 1;
            }
            else
            {
                node.Type = NodeType.Wall;
            }
        }

        private void EraseNode(Node node)
        {
            if (node.Type == NodeType.Start)
            {
                Grid.StartNode = null;
            } 
            else if (node.Type == NodeType.End)
            {
                Grid.EndNodes.Remove(node);
                Grid.EndNode = Grid.EndNodes.Count > 0 ? Grid.EndNodes[0] : null;

                OnPropertyChanged(nameof(CanRunSinglePath));
                OnPropertyChanged(nameof(CanRunMultiPath));
            }

            node.Type = NodeType.Empty;
            node.BaseTerrainType = NodeType.Empty;
            node.Weight = 1;
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

            NodesExplored = 0;
            PathLength = 0;
            PathCost = 0;
            CurrentAlgorithm = algorithm.AlgorithmName;

            IsRunning = true;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            bool pathFound = await algorithm.FindPathAsync(
                Grid, onNodeVisited: (node) => 
                {
                    NodesExplored++;
                },
                delayMs: AnimationDelay);

            stopwatch.Stop();
            TimeTaken = stopwatch.Elapsed.TotalSeconds;

            IsRunning = false;

            if (pathFound)
            {
                CalculatePathStastistics();
            }
            else
            {
                System.Windows.MessageBox.Show($"No path found using {algorithm.AlgorithmName}!");
            }
        }

        private void CalculatePathStastistics()
        {
            int totalPathLength = 0;
            int totalPathCost = 0;

            foreach (var endNode in Grid.EndNodes)
            {
                Node? currentNode = endNode;
                int pathLength = 0;
                int pathCost = 0;

                while (currentNode != null && currentNode.Parent != null)
                {
                    pathLength++;
                    pathCost += currentNode.Weight;
                    currentNode = currentNode.Parent;
                }
                totalPathLength += pathLength;
                totalPathCost += pathCost;
            }

            PathLength = totalPathLength;
            PathCost = totalPathCost;
        }
    }

    public enum DrawMode
    {
        Start,
        End,
        Wall,
        Grass,
        Mud,
        Erase
    }
}
