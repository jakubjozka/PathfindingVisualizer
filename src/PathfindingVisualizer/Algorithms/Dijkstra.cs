using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathfindingVisualizer.Models;

namespace PathfindingVisualizer.Algorithms
{
    public class Dijkstra : IPathfindingAlgorithm
    {
        // Dijkstra's Algorithm - Finds the shortest path between two nodes
        // Uses a priority queue to explore nodes in order of distance from start
        // Guarantees the shortest path but explores many nodes
        // Time Complexity: O(V²) or O((V + E) log V) with proper priority queue
        public string AlgorithmName => "Dijkstra";
        public async Task<bool> FindPathAsync(Grid grid, Action<Node> onNodeVisited, int delayMs = 10)
        {
            if (grid.StartNode == null || grid.EndNode == null)
                return false;

            grid.ResetForPathfinding();

            var startNode = grid.StartNode;
            var endNode = grid.EndNode;

            startNode.GCost = 0;

            List<Node> openSet = new List<Node> { startNode };
            HashSet<Node> closedSet = new HashSet<Node>();

            while (openSet.Count > 0)
            {
                openSet.Sort((a, b) => a.GCost.CompareTo(b.GCost));
                Node currentNode = openSet[0];
                openSet.RemoveAt(0);

                if (currentNode.Type != NodeType.Start && currentNode.Type != NodeType.End)
                {
                    if (currentNode.Type == NodeType.Grass || currentNode.Type == NodeType.Mud)
                    {
                        currentNode.BaseTerrainType = currentNode.Type;
                    }

                        currentNode.Type = NodeType.Visited;
                    onNodeVisited(currentNode);
                    await Task.Delay(delayMs * currentNode.Weight);
                }

                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    ReconstructPath(endNode);
                    return true;
                }

                foreach (Node neighbor in grid.GetNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    int tentativeGCost = currentNode.GCost + neighbor.Weight;

                    if (tentativeGCost < neighbor.GCost)
                    {
                        neighbor.Parent = currentNode;
                        neighbor.GCost = tentativeGCost;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            return false;
        }

        private static void ReconstructPath(Node endNode)
        {
            Node? currentNode = endNode.Parent;

            while (currentNode != null && currentNode.Type != NodeType.Start)
            {
                if (currentNode.Type == NodeType.Grass || currentNode.Type == NodeType.Mud || currentNode.Type == NodeType.Visited)
                {
                    if (currentNode.BaseTerrainType == NodeType.Empty)
                    {
                        if (currentNode.Type == NodeType.Grass || currentNode.Type == NodeType.Mud)
                        {
                            currentNode.BaseTerrainType = currentNode.Type;
                        }
                    }
                }

                currentNode.Type = NodeType.Path;
                currentNode = currentNode.Parent;
            }
        }
    }
}
