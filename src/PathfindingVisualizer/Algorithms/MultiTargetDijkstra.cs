using PathfindingVisualizer.Models;

namespace PathfindingVisualizer.Algorithms
{
    public class MultiTargetDijkstra : IPathfindingAlgorithm
    {
        // I wanted to make Floyd-Warshall work for multiple targets, but it's not ideal for visualization
        // Instead, I implemented a multi-target Dijkstra as it's more efficient and suitable for visualization
        public string AlgorithmName => "Multi-Target Dijkstra";
        public string ToolTip => "Finds shortest paths from start to multiple end nodes.\nExtension of Dijkstra that continues exploring until all targets are found.\nMore efficient than running single Dijkstra multiple times.\nUseful for finding paths to several destinations simultaneously.\nTime Complexity: O(V²) or O((V + E) log V)";
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
                Node currentNode = openSet[0];
                openSet.RemoveAt(0);

                if (visited.Contains(currentNode))
                    continue;

                visited.Add(currentNode);

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

                if (grid.EndNodes.Contains(currentNode))
                {
                    foundEndNodes++;
                    if (foundEndNodes >= grid.EndNodes.Count)
                    {
                        break;
                    }
                }

                foreach (Node neighbor in grid.GetNeighbors(currentNode))
                {
                    if (visited.Contains(neighbor))
                        continue;

                    int newDist = distances[currentNode] + neighbor.Weight;

                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        parents[neighbor] = currentNode;

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
            Node? currentNode = endNode;
            List<Node> path = new List<Node>();

            while (currentNode != null && parents.ContainsKey(currentNode))
            {
                path.Add(currentNode);
                currentNode = parents[currentNode];
            }

            foreach (var node in path)
            {
                if (node.Type != NodeType.Start && node.Type != NodeType.End)
                {
                    if (node.Type == NodeType.Grass || node.Type == NodeType.Mud || node.Type == NodeType.Visited)
                    {
                        if (node.BaseTerrainType == NodeType.Empty)
                        {
                            if (node.Type == NodeType.Grass || node.Type == NodeType.Mud)
                            {
                                node.BaseTerrainType = node.Type;
                            }
                        }
                    }

                    node.Type = NodeType.Path;
                }
            }
        }
    }
}