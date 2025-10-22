using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace PathfindingVisualizer.Models
{
    // Represents a single node in the grid for pathfinding
    public class Node : INotifyPropertyChanged
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public int GCost { get; set; } // Distance from start
        public int HCost { get; set; } // Heuristic distance to end
        public int FCost => GCost + HCost; // Total cost

        public Node? Parent { get; set; }

        private NodeType _type;
        public NodeType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        public Node(int row, int col)
        {
            Row = row;
            Col = col;
            Type = NodeType.Empty;
            GCost = int.MaxValue;
            HCost = 0;
            Parent = null;
        }

        public void Reset()
        {
            if (Type != NodeType.Wall && Type != NodeType.Start && Type != NodeType.End)
            {
                Type = NodeType.Empty;
            }
            GCost = int.MaxValue;
            HCost = 0;
            Parent = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum NodeType
    {
        Empty,
        Wall,
        Start,
        End,
        Visited,
        Path
    }
}
