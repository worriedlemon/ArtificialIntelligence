using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public class GeneticAlgorithm
        : EvolutionAlgorithm
        , IGraphColoring
    {
        int totalColors = 0;

        public override void SolveFunctionOptimizationProblem(Function f) => SolveFunctionOptimization(f, null);

        public override void SolveLongestPathProblem(Graph g) => SolveLongestPath(g, null);

        public void SolveGraphColoringProblem(Graph g)
        {
            Agent<int> RandomAgent()
            {
                var randomColors = Enumerable.Repeat(g.VertexCount, g.VertexCount).Select(a => rand.Next(a)).ToList();
                return new()
                {
                    Values = randomColors,
                    Fitness = CalculateFitness(g, randomColors)
                };
            }

            List<Agent<int>> agents = new((int)AgentCount);
            for (int i = 0; i < AgentCount; i++)
            {
                agents.Add(RandomAgent());
            }

            Agent<int> best = agents.MinBy(b => b.Fitness)!;

            for (int i = 0; i < MaxIterations && !Stop; ++i)
            {
                OnUpdateGraphics(best.Values);

                // new generation
                List<Agent<int>> gen = new((int)AgentCount) { best };

                for (int j = 1; j < AgentCount; ++j)
                {
                    int p1 = rand.Next((int)AgentCount), p2 = rand.Next((int)AgentCount);

                    List<int> child;
                    if (rand.NextDouble() < CrossingoverPossibility)
                    {
                        child = new(g.VertexCount);
                        int crp = rand.Next(g.VertexCount);
                        for (int k = 0; k < g.VertexCount; k++)
                        {
                            child.Add(agents[k < crp ? p1 : p2].Values[i]);
                        }
                    }
                    else
                    {
                        child = new(agents[p1].Values);
                    }

                    if (rand.NextDouble() < MutationPossibility)
                    {
                        child[rand.Next(g.VertexCount)] = rand.Next(g.VertexCount);
                    }

                    double newFitness = CalculateFitness(g, child);
                    gen.Add(newFitness >= agents[j].Fitness ? agents[j] : new()
                    {
                        Values = child,
                        Fitness = newFitness
                    });
                }
                
                best = (agents = gen).MinBy(b => b.Fitness)!;

                // No need to continue
                if (best.Fitness == 0) break;
            }

            // Best
            bestSolutionI = best.Values;
            totalColors = bestSolutionI.Distinct().Count();

            OnUpdateGraphics(best.Values, true);
        }

        private double CalculateFitness(Graph g, List<int> colors)
        {
            double fitness = 0;
            for (int i = 0; i < g.VertexCount; ++i)
            {
                fitness += VertexConflicts(g, i, colors);
            }
            return fitness;
        }

        private int VertexConflicts(Graph g, int v, List<int> colors)
        {
            int conflicts = 0;
            for (int i = 0; i < g.VertexCount; ++i)
            {
                if (i != v && g[v, i] != Graph.NoPath && colors[v] == colors[i])
                {
                    ++conflicts;
                }
            }

            return conflicts;
        }

        public string GetGraphColoringResultString()
        {
            return $"Graph successfully colored!\nTotal colors: {totalColors}";
        }
    }
}
