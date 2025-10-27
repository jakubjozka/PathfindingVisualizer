﻿using System;
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
                Node currentNode = queue.Dequeue();

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

                if (currentNode == endNode)
                {
                    ReconstructPath(endNode);
                    return true;
                }

                foreach (Node neighbor in grid.GetNeighbors(currentNode))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        neighbor.Parent = currentNode;
                        queue.Enqueue(neighbor);
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
