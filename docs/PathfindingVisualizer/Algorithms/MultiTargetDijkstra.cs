using PathfindingVisualizer.Models;

namespace PathfindingVisualizer.Algorithms
{
    public class MultiTargetDijkstra : IPathfindingAlgorithm
    {
        // I wanted to make Floyd-Warshall work for multiple targets, but it's not ideal for visualization
        // Instead, I implemented a multi-target Dijkstra as it's more efficient and suitable for visualization

        // Multi-Target Dijkstra - Finds shortest paths from start to multiple end nodes
        // Extension of Dijkstra that continues exploring until all targets are found
        // More efficient than running single Dijkstra multiple times
        // Useful for finding paths to several destinations simultaneously
        // Time Complexity: O(V²) or O((V + E) log V) with proper priority queue
        public string AlgorithmName => "Multi-Target Dijkstra";
        public async Task<bool> FindPathAsync(Grid grid, Action<Node> onNodeVisited, int delayMs = 10)
        {
            if (grid.StartNode == null || grid.EndNodes.Count == 0)
                return false;

            grid.ResetForPathfinding();

            var startNode = grid.StartNode;

            Dictionary<Node, int> distances = new Dictionary<Node, int>();
            Dictionary<Node, Node?> parents = new Dictionary<Node, Node?>();
            HashSet<Node> visited = new HashSet<Node>();

            List<Node> openSet = new List<Node> { startNode };
            distances[startNode] = 0;
            parents[startNode] = null;

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Cols; col++)
                {
                    Node node = grid.Nodes[row, col];
                    if (node.Type != NodeType.Wall && node != startNode)
                    {
                        distances[node] = int.MaxValue;
                        parents[node] = null;
                    }
                }
            }

            int foundEndNodes = 0;

            while (openSet.Count > 0)
            {
                openSet.Sort((a, b) => distances[a].CompareTo(distances[b]));
                Node current = openSet[0];
                openSet.RemoveAt(0);

                if (visited.Contains(current))
                    continue;

                visited.Add(current);

                if (current.Type != NodeType.Start && current.Type != NodeType.End)
                {
                    current.Type = NodeType.Visited;
                    onNodeVisited(current);
                    await Task.Delay(delayMs * current.Weight);
                }

                if (grid.EndNodes.Contains(current))
                {
                    foundEndNodes++;
                    if (foundEndNodes >= grid.EndNodes.Count)
                    {
                        break;
                    }
                }

                foreach (Node neighbor in grid.GetNeighbors(current))
                {
                    if (visited.Contains(neighbor))
                        continue;

                    int newDist = distances[current] + neighbor.Weight;

                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        parents[neighbor] = current;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            foreach (var endNode in grid.EndNodes)
            {
                if (distances[endNode] < int.MaxValue)
                {
                    ReconstructPath(endNode, parents);
                }
            }

            return foundEndNodes > 0;
        }

        private static void ReconstructPath(Node endNode, Dictionary<Node, Node?> parents)
        {
            Node? current = endNode;
            List<Node> path = new List<Node>();

            while (current != null && parents.ContainsKey(current))
            {
                path.Add(current);
                current = parents[current];
            }

            foreach (var node in path)
            {
                if (node.Type != NodeType.Start && node.Type != NodeType.End)
                {
                    node.Type = NodeType.Path;
                }
            }
        }
    }
}