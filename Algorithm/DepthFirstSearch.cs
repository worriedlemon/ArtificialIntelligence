using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public class DepthFirstSearch
        : AbstractAlgorithm
        , ILongestPath
    {
        private List<int> solution = new();
        private double length = 0;

        public void SolveLongestPathProblem(Graph g)
        {
            solution.Clear();

            double[] dp = new double[g.VertexCount];
            Array.Fill(dp, -1);
            int[] nextVertex = new int[g.VertexCount];
            Array.Fill(nextVertex, -1);

            Stack<(int vertex, bool isProcessing)> stack = new();
            stack.Push((0, true));

            while (stack.Count > 0 && !Stop)
            {
                var (vertex, isProcessing) = stack.Pop();

                if (isProcessing)
                {
                    if (dp[vertex] == -1)
                    {
                        stack.Push((vertex, false));

                        for (int neighbor = 0; neighbor < g.VertexCount; ++neighbor)
                        {
                            if (g[vertex, neighbor] != Graph.NoPath && dp[neighbor] == -1)
                            {
                                stack.Push((neighbor, true));
                            }
                        }
                    }
                }
                else
                {
                    double maxLength = 0;
                    int bestNext = -1;

                    for (int neighbor = 0; neighbor < g.VertexCount; ++neighbor)
                    {
                        if (g[vertex, neighbor] != Graph.NoPath && dp[neighbor] != -1)
                        {
                            if (dp[neighbor] + 1 > maxLength)
                            {
                                maxLength = dp[neighbor] + 1;
                                bestNext = neighbor;
                            }
                        }
                    }

                    dp[vertex] = maxLength;
                    nextVertex[vertex] = bestNext;
                }
            }
            for (int current = 0; current != -1; current = nextVertex[current])
            {
                solution.Add(current);
            }

            length = 0;
            for (int i = 1; i < solution.Count; ++i)
            {
                length += g[solution[i - 1], solution[i]];
            }

            OnUpdateGraphics(solution, true);
        }

        public string GetLongestPathResultString()
        {
            return new System.Text.StringBuilder()
                .Append("Found solution:\n[")
                .AppendJoin(Graph.GoesTo, solution.ToArray())
                .Append($"]\nwith overall length = {length:F3}")
                .ToString();
        }
    }
}
