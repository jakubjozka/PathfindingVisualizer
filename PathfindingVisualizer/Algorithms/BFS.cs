using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathfindingVisualizer.Models;

namespace PathfindingVisualizer.Algorithms
{
    public class BFS : IPathfindingAlgorithm
    {
        public string AlgorithmName => "BFS";

        public async Task<bool> FindPathAsync(Grid grid, Action<Node> onNodeVisited, int delayMs = 10)
        {
            // BFS (Breadth-First Search) - Explores nodes level by level
            // Uses a queue to explore all neighbors at current depth before going deeper
            // Guarantees shortest path in unweighted graphs
            // Does not use distance costs - treats all edges equally
            // Time Complexity: O(V + E)

            if (grid.StartNode == null || grid.EndNode == null)
                return false;

            grid.ResetForPathfinding();

            var startNode = grid.StartNode;
            var endNode = grid.EndNode;

            Queue<Node> queue = new Queue<Node>();
            HashSet<Node> visited = new HashSet<Node>();

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                if (current.Type != NodeType.Start && current.Type != NodeType.End)
                {
                    current.Type = NodeType.Visited;
                    onNodeVisited(current);
                    await Task.Delay(delayMs);
                }

                if (current == endNode)
                {
                    ReconstructPath(endNode);
                    return true;
                }

                foreach (Node neighbor in grid.GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        neighbor.Parent = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return false;
        }

        private void ReconstructPath(Node endNode)
        {
            Node? current = endNode.Parent;

            while (current != null && current.Type != NodeType.Start)
            {
                current.Type = NodeType.Path;
                current = current.Parent;
            }
        }
    }
}
