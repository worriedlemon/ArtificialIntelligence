using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public class BacteriaForaging
        : AbstractAlgorithm
        , IGraphColoring
    {
        [AlgorithmOption] public uint BacteriaCount { get; set; } = 20;
        [AlgorithmOption] public uint MaxGenerations { get; set; } = 15;
        [AlgorithmOption] public uint ChemotacticSteps { get; set; } = 10;
        [AlgorithmOption] public double StepSize { get; set; } = 0.1;
        [AlgorithmOption] public double DispersePossibility { get; set; } = 0.25;
        [AlgorithmOption] public double CrossingoverPossibility { get; set; } = 0.5;
        [AlgorithmOption] public double MutationPossibility { get; set; } = 0.15;

        private List<int> colors = new();
        int totalColors = 0;
        
        Random rand = new();

        // Bacteria, fitness equals to vertex conflicts
        private class Bacteria
        {
            public List<int> Values = new();
            public int Fitness;
        }

        public void SolveGraphColoringProblem(Graph g)
        {
            Bacteria RandomBacteria()
            {
                var randomColors = Enumerable.Repeat(g.VertexCount, g.VertexCount).Select(a => rand.Next(a)).ToList();
                return new()
                {
                    Values = randomColors,
                    Fitness = CalculateFitness(g, randomColors)
                };
            }

            List<Bacteria> bacteria = new((int)BacteriaCount);
            for (int i = 0; i < BacteriaCount; i++)
            {
                bacteria.Add(RandomBacteria());
            }

            Bacteria best = bacteria.MinBy(b => b.Fitness)!;

            for (int i = 0; i < MaxGenerations && !Stop; ++i)
            {
                OnUpdateGraphics(best.Values);

                for (int j = 0; j < ChemotacticSteps; ++j)
                {
                    foreach (var b in bacteria)
                    {
                        Chemotaxis(g, b);
                    }

                    // Sorting by fitness
                    bacteria = bacteria.OrderBy(b => b.Fitness).ToList();
                }

                ReproduceBacterias(g, bacteria);

                // Dispersion
                for (int j = 0; j < BacteriaCount; ++j)
                {
                    if (rand.NextDouble() < DispersePossibility)
                    {
                        bacteria[j] = RandomBacteria();
                    }
                }

                best = bacteria.MinBy(b => b.Fitness)!;
            }

            // Best
            colors = best.Values;
            totalColors = colors.Distinct().Count();

            OnUpdateGraphics(best.Values, true);
        }

        private void Chemotaxis(Graph g, Bacteria b)
        {
            List<int> newColors = new(b.Values);
            int nc = newColors.Count;

            // Taking some random vertices
            int changeCount = Math.Max(1, (int)(StepSize * nc));

            for (int i = 0; i < changeCount; i++)
            {
                int v = rand.Next(nc);
                if (rand.NextDouble() < MutationPossibility)
                {
                    // Random color
                    newColors[v] = rand.Next(0, nc);
                }
                else
                {
                    // Best color with minimum conflicts
                    newColors[v] = Enumerable.Range(0, g.VertexCount).Select(i =>
                    {
                        List<int> temp = new(b.Values);
                        temp[v] = i;
                        return temp;
                    }).MinBy(c => VertexConflicts(g, v, c))![v];
                }
            }

            int newFitness = CalculateFitness(g, newColors);
            if (newFitness < b.Fitness)
            {
                b.Values = newColors;
                b.Fitness = newFitness;
            }
        }

        private void ReproduceBacterias(Graph g, List<Bacteria> bacteria)
        {
            // Removing half of bacterias
            if (bacteria.Count > 1)
            {
                bacteria = bacteria.Take((int)BacteriaCount / 2).ToList();
            }
            for (int i = bacteria.Count; i < BacteriaCount; i++)
            {
                int p1 = rand.Next(bacteria.Count);
                int p2 = rand.Next(bacteria.Count);

                // New bacteria
                List<int> child = new();
                for (int j = 0; j < g.VertexCount; ++j)
                {
                    child.Add(bacteria[rand.NextDouble() < CrossingoverPossibility ? p1 : p2].Values[j]);
                }

                if (rand.NextDouble() < MutationPossibility)
                {
                    child[rand.Next(0, child.Count)] = rand.Next(0, rand.Next(0, child.Count));
                }

                bacteria.Add(new()
                {
                    Values = child,
                    Fitness = CalculateFitness(g, child)
                });
            }
        }

        private int CalculateFitness(Graph g, List<int> colors)
        {
            int fitness = 0;
            for (int i = 0; i < g.VertexCount; ++i)
            {
                fitness += VertexConflicts(g, i, colors);
            }
            return fitness + colors.Distinct().Count();
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
