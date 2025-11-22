using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public class GreedyColoring
        : AbstractAlgorithm
        , IGraphColoring
    {
        private List<int> colors = new();
        int totalColors = 0;

        public void SolvegColoringProblem(Graph g)
        {
            // First create uncolored list except first vertex
            colors = Enumerable.Repeat(-1, g.VertexCount).ToList();
            colors[0] = 0;

            List<bool> available = Enumerable.Repeat(false, g.VertexCount).ToList();

            void SetAvailable(int index, bool a)
            {
                for (int j = 0; j < g.VertexCount; j++)
                {
                    if (g[index, j] != Graph.NoPath && colors[j] != -1)
                    {
                        available[colors[j]] = a;
                    }
                }
            }

            // Coloring rest vertices
            for (int i = 1; i < g.VertexCount; i++)
            {
                // Getting first available color
                SetAvailable(i, true);
                colors[i] = available.FindIndex(0, v => !v);
                SetAvailable(i, false);
            }

            totalColors = colors.Distinct().Count();

            OnUpdateGraphics(colors);
        }

        public string GetGraphColoringResultString()
        {
            return $"Graph successfully colored!\nTotal colors: {totalColors}";
        }
    }
}
