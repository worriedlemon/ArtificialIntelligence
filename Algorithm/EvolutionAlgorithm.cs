using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public abstract class EvolutionAlgorithm
        : AbstractAlgorithm
        , IFunctionOptimization
        , ILongestPath
    {
        // Algorithm Options
        [AlgorithmOption] public uint AgentCount { get; set; } = 100;
        [AlgorithmOption] public double MutationPossibility { get; set; } = 0.15;
        [AlgorithmOption] public double CrossingoverPossibility { get; set; } = 0.5;
        [AlgorithmOption] public double MaxIterations { get; set; } = 100;

        protected readonly Random rand = new();

        protected double bestSolutionValue = 0;
        protected List<double> bestSolution = [ 0.0, 0.0 ];
        protected List<int> bestSolutionI = new();

        // Special type for agent
        protected class Agent<T>
        {
            public List<T> Values = new();
            public double Fitness;
        }

        public abstract void SolveFunctionOptimizationProblem(Function f);

        protected void SolveFunctionOptimization(Function f, Action<List<Agent<double>>, Function>? applyLocalSearchDelegate)
        {
            bestSolutionValue = double.PositiveInfinity;

            // Generate new population
            List<Agent<double>> agents = new();
            for (int i = 0; i < AgentCount; ++i)
            {
                var p = f.GetPoint(rand);
                agents.Add(new()
                {
                    Values = p,
                    Fitness = f.Evaluate(p[0], p[1])
                });
            }

            // Local search
            applyLocalSearchDelegate?.Invoke(agents, f);

            // Casting agents to List<PointF> for graphics to work
            List<PointF> AgentsToPointF()
            {
                return agents.Select(a => new PointF((float)a.Values[0], (float)a.Values[1])).ToList();
            }

            for (int k = 0; k < MaxIterations && !Stop; ++k)
            {
                OnUpdateGraphics(AgentsToPointF());

                // Crossingover
                for (int i = 0; i < AgentCount; ++i)
                {
                    if (rand.NextDouble() > CrossingoverPossibility) continue;

                    var index = rand.Next(0, (int)AgentCount);
                    if (index == i) continue;

                    double newX = (agents[i].Values[0] + agents[index].Values[0]) / 2;
                    double newY = (agents[i].Values[1] + agents[index].Values[1]) / 2;
                    Agent<double> agent = new()
                    {
                        Values = [ newX, newY ],
                        Fitness = f.Evaluate(newX, newY)
                    };
                    agents.Add(agent);
                }

                // Mutation
                for (int i = 0; i < agents.Count; ++i)
                {
                    if (rand.NextDouble() < MutationPossibility)
                    {
                        agents[i].Values[0] += 2 * rand.NextDouble() - 1;
                        agents[i].Values[1] += 2 * rand.NextDouble() - 1;
                        f.Clamp(agents[i].Values);
                        agents[i].Fitness = f.Evaluate(agents[i].Values[0], agents[i].Values[1]);
                    }
                }

                // Local search
                applyLocalSearchDelegate?.Invoke(agents, f);

                // Sort by fitness function and leave only the best
                agents = agents.OrderBy(a => a.Fitness).ToList();
                agents.RemoveRange((int)AgentCount, agents.Count - (int)AgentCount);

                if (agents.First().Fitness < bestSolutionValue)
                {
                    bestSolutionValue = agents.First().Fitness;
                    bestSolution = new(agents.First().Values);
                }
            }

            OnUpdateGraphics(AgentsToPointF(), true);
        }

        public string GetFunctionOptimizationResultString()
        {
            return $"Found solution: f({bestSolution[0]:F3}, {bestSolution[1]:F3}) = {bestSolutionValue:F3}";
        }

        public abstract void SolveLongestPathProblem(Graph g);

        protected void SolveLongestPath(Graph g, Action<List<Agent<int>>, Graph>? applyLocalSearchDelegate)
        {
            List<Agent<int>> agents = new();
            for (int i = 0; i < AgentCount; i++)
            {
                var path = RandomPath(g);
                agents.Add(new(){
                    Values = path,
                    Fitness = PathLength(path, g)
                });
            }

            applyLocalSearchDelegate?.Invoke(agents, g);

            var overallBestAgent = agents.OrderByDescending(c => c.Fitness).First();

            OnUpdateGraphics(overallBestAgent.Values);

            for (int i = 0; i < MaxIterations && !Stop; i++)
            {
                // Saving global best as one of agents
                List<Agent<int>> newAgents = [overallBestAgent];

                // Adding new agents
                while (newAgents.Count < AgentCount)
                {
                    Agent<int> p1 = agents[rand.Next((int)AgentCount)], p2 = agents[rand.Next((int)AgentCount)];

                    Agent<int> child = p1 != p2 && rand.NextDouble() > CrossingoverPossibility ? PathsCrossingover(p1, p2, g) : p1;

                    Mutate(child, g, MutationPossibility);

                    newAgents.Add(child);
                }

                agents = newAgents;

                applyLocalSearchDelegate?.Invoke(agents, g);

                var localBestAgent = agents.OrderByDescending(c => c.Fitness).First();
                if (localBestAgent.Fitness > overallBestAgent.Fitness)
                {
                    overallBestAgent = localBestAgent;
                    OnUpdateGraphics(overallBestAgent.Values);
                }
            }

            bestSolutionI = overallBestAgent.Values;
            bestSolutionValue = PathLength(bestSolutionI, g);

            OnUpdateGraphics(bestSolutionI, true);
        }

        private List<int> RandomPath(Graph g)
        {
            List<int> path = [ 0 ];
            var available = Enumerable.Range(1, g.VertexCount - 1).OrderBy(x => rand.Next()).ToHashSet();

            int cur = 0;
            while (available.Count > 0)
            {
                List<int> possible = available.Where(v => g[cur, v] != Graph.NoPath).ToList();

                if (possible.Count == 0)
                {
                    break;
                }

                int nextVertex = possible[rand.Next(possible.Count)];
                path.Add(nextVertex);
                available.Remove(nextVertex);
                cur = nextVertex;
            }

            return path;
        }

        enum MutationType
        {
            Swap = 0, Move, Insert, Count
        }


        protected void Mutate(Agent<int> agent, Graph g, double possibility)
        {
            if (agent.Values.Count < 2 || rand.NextDouble() < possibility) return;

            MutationType mutation = (MutationType)rand.Next((int)MutationType.Count);

            var path = agent.Values;
            switch (mutation)
            {
                case MutationType.Swap:
                    if (path.Count < 3) return;

                    int i = rand.Next(1, path.Count - 1);
                    int j = rand.Next(1, path.Count - 1);
                    if (i != j &&
                        g[path[i], path[j + 1]] != Graph.NoPath && g[path[j - 1], path[i]] != Graph.NoPath &&
                        g[path[j], path[i + 1]] != Graph.NoPath && g[path[i - 1], path[j]] != Graph.NoPath)
                    {
                        (path[i], path[j]) = (path[j], path[i]);
                    }
                    break;

                case MutationType.Move:
                    if (path.Count < 3) return;

                    int ri = rand.Next(1, path.Count - 1);
                    int ai = rand.Next(1, path.Count - 1);
                    if (ri != ai &&
                        g[path[ri - 1], path[ri + 1]] != Graph.NoPath &&
                        g[path[ri], path[ai]] != Graph.NoPath && g[path[ai - 1], path[ri]] != Graph.NoPath)
                    {
                        path.RemoveAt(ri);
                        path.Insert(ai, path[ri]);
                    }
                    break;
                case MutationType.Insert:
                    var visited = path.ToHashSet();
                    var available = Enumerable.Range(0, g.VertexCount)
                        .Where(v => !visited.Contains(v) && g[path.Last(), v] != Graph.NoPath)
                        .ToList();

                    if (available.Count > 0)
                    {
                        path.Add(available[rand.Next(available.Count)]);
                    }
                    break;
            }

            agent.Fitness = PathLength(agent.Values, g, 0.2);
        }

        private Agent<int> PathsCrossingover(Agent<int> p1, Agent<int> p2, Graph g)
        {
            int minV = Math.Min(p1.Values.Count, p2.Values.Count);

            if (minV < 2)
            {
                return new() { Values = new(p1.Values), Fitness = p1.Fitness };
            }

            int crossoverPoint = rand.Next(1, minV - 1);

            List<int> newPath = new();
            for (int i = 0; i < crossoverPoint; i++)
            {
                newPath.Add(p1.Values[i]);
            }

            var visited = newPath.ToHashSet();
            for (int i = crossoverPoint; i < p2.Values.Count; i++)
            {
                int vertex = p2.Values[i];
                if (!visited.Contains(vertex) && g[newPath.Last(), vertex] != Graph.NoPath)
                {
                    newPath.Add(vertex);
                    visited.Add(vertex);
                }
            }

            if (newPath.Count < 2)
            {
                newPath = new List<int>(p1.Values);
            }

            return new() { Values = newPath, Fitness = PathLength(newPath, g, 0.2) };
        }

        private double PathLength(List<int> path, Graph g, double bonus = 0)
        {
            double length = 0;
            for (int i = 1; i < path.Count; i++)
            {
                length += g[path[i - 1], path[i]];
            }
            return length + bonus * path.Count;
        }

        public string GetLongestPathResultString()
        {
            return new System.Text.StringBuilder()
                .Append("Found solution:\n[")
                .AppendJoin(Graph.GoesTo, bestSolutionI.ToArray())
                .Append($"]\nwith overall length = {bestSolutionValue:F3}")
                .ToString();
        }
    }
}
