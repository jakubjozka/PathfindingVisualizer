using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathfindingVisualizer.Models;

namespace PathfindingVisualizer.Algorithms
{
    public interface IPathfindingAlgorithm
    {
        Task<bool> FindPathAsync(Grid grid, Action<Node> onNodeVisited, int delayMs = 10);
        string AlgorithmName { get; }
    }
}
