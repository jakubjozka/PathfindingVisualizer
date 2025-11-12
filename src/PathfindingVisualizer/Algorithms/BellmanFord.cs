using PathfindingVisualizer.Models;

namespace PathfindingVisualizer.Algorithms
{
    public class BellmanFord : IPathfindingAlgorithm
    {
        public string AlgorithmName => "Bellman-Ford (Multi-Target)";
        public string ToolTip => "Finds shortest paths from start to all nodes using edge relaxation.\nRepeatedly updates distances by checking all edges.\nCan detect negative cycles and works with negative weights.\nSlower than Dijkstra but more versatile.\nTime Complexity: O(V × E)";

        public async Task<bool> FindPathAsync(Grid grid, Action<Node> onNodeVisited, int delayMs = 10)
        {
            if (grid.StartNode == null || grid.EndNodes.Count == 0)
                return false;

            grid.ResetForPathfinding();

            var startNode = grid.StartNode;

            Dictionary<Node, int> distances = new Dictionary<Node, int>();
            Dictionary<Node, Node?> parents = new Dictionary<Node, Node?>();
            HashSet<Node> processedNodes = new HashSet<Node>();

            List<Node> allNodes = new List<Node>();
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Cols; col++)
                {
                    Node node = grid.Nodes[row, col];
                    if (node.Type != NodeType.Wall)
                    {
                        allNodes.Add(node);
                        distances[node] = int.MaxValue;
                        parents[node] = null;
                    }
                }
            }

            distances[startNode] = 0;
            int foundEndNodes = 0;
            int targetEndNodes = grid.EndNodes.Count;

            for (int iteration = 0; iteration < allNodes.Count - 1; iteration++)
            {
                bool relaxedAnyEdge = false;

                foreach (Node node in allNodes)
                {
                    if (distances[node] == int.MaxValue)
                        continue;

                    if (!processedNodes.Contains(node) &&
                        node.Type != NodeType.Start &&
                        node.Type != NodeType.End)
                    {
                        if (node.Type == NodeType.Grass || node.Type == NodeType.Mud)
                        {
                            node.BaseTerrainType = node.Type;
                        }

                        node.Type = NodeType.Visited;
                        onNodeVisited(node);
                        await Task.Delay(delayMs * node.Weight);
                        processedNodes.Add(node);
                    }

                    if (grid.EndNodes.Contains(node) && !processedNodes.Contains(node))
                    {
                        foundEndNodes++;
                        processedNodes.Add(node);

                        if (foundEndNodes >= targetEndNodes)
                        {
                            goto FinishEarly;
                        }
                    }

                    foreach (Node neighbor in grid.GetNeighbors(node))
                    {
                        int newDistance = distances[node] + neighbor.Weight;

                        if (newDistance < distances[neighbor])
                        {
                            distances[neighbor] = newDistance;
                            parents[neighbor] = node;
                            relaxedAnyEdge = true;
                        }
                    }
                }

                if (!relaxedAnyEdge)
                    break;
            }

        FinishEarly:

            bool foundAnyPath = false;
            foreach (var endNode in grid.EndNodes)
            {
                if (distances[endNode] < int.MaxValue)
                {
                    ReconstructPath(endNode, parents);
                    foundAnyPath = true;
                }
            }

            return foundAnyPath;
        }

        private void ReconstructPath(Node endNode, Dictionary<Node, Node?> parents)
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