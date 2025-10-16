using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PathfindingVisualizer.Models
{
    public class Grid
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Node[,] Nodes { get; private set; }

        public Node? StartNode { get; set; }
        public Node? EndNode { get; set; }
        public List<Node> EndNodes { get; set; }

        public Grid(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Nodes = new Node[rows, cols];
            EndNodes = new List<Node>();

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Nodes[row, col] = new Node(row, col);
                }
            }
        }

        public List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            int[] dRow = { -1, 1, 0, 0 };
            int[] dCol = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newRow = node.Row + dRow[i];
                int newCol = node.Col + dCol[i];

                if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Cols)
                {
                    Node neighbor = Nodes[newRow, newCol];

                    if (neighbor.Type != NodeType.Wall)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        public void ResetForPathfinding()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Nodes[row, col].Reset();
                }
            }
        }

        public void ClearGrid()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Nodes[row, col].Type = NodeType.Empty;
                    Nodes[row, col].Reset();
                }
            }

            StartNode = null;
            EndNode = null;
            EndNodes.Clear();
        }

        public bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Cols;
        }
    }
}
