using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public class MemeticAlgorithm : EvolutionAlgorithm
    {
        [AlgorithmOption] public uint LocalSearchIterations { get; set; } = 10;
        [AlgorithmOption] public double LocalSearchArea { get; set; } = 0.25;
        [AlgorithmOption] public double LocalSearchRate { get; set; } = 0.3;

        public override void SolveFunctionOptimizationProblem(Function f)
        {
            SolveFunctionOptimization(f, (List<Agent<double>> agents, Function f) => {
                List<Agent<double>> localAgents = agents
                    .OrderBy(c => c.Fitness)
                    .Take((int)(agents.Count * LocalSearchRate))
                    .ToList();

                foreach (var agent in localAgents)
                {
                    for (int i = 0; i < LocalSearchIterations; i++)
                    {
                        double newX = agent.Values[0] + LocalSearchArea * (rand.NextDouble() - 0.5);
                        double newY = agent.Values[1] + LocalSearchArea * (rand.NextDouble() - 0.5);
                        double newFitness = f.Evaluate(newX, newY);

                        if (newFitness < agent.Fitness)
                        {
                            agent.Values[0] = newX;
                            agent.Values[1] = newY;
                            agent.Fitness = newFitness;
                        }
                    }
                }
            });
        }
        public override void SolveLongestPathProblem(Graph g)
        {
            SolveLongestPath(g, (List<Agent<int>> agents, Graph g) => {
                List<Agent<int>> localAgents = agents
                    .OrderByDescending(c => c.Fitness)
                    .Take((int)(agents.Count * LocalSearchRate))
                    .ToList();

                foreach (var agent in localAgents)
                {
                    for (int i = 0; i < LocalSearchIterations; ++i)
                    {
                        Agent<int> newAgent = new()
                        {
                            Values = new(agent.Values),
                            Fitness = agent.Fitness
                        };
                        Mutate(newAgent, g, 1.0);

                        if (newAgent.Fitness < agent.Fitness)
                        {
                            agent.Values = newAgent.Values;
                            agent.Fitness = newAgent.Fitness;
                        }
                    }
                }
            });
        }
    }
}
