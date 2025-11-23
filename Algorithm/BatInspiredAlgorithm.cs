using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public class BatInspiredAlgorithm
        : AbstractAlgorithm
        , IFunctionOptimization
    {
        [AlgorithmOption] public uint PopulationSize { get; set; } = 50;
        [AlgorithmOption] public uint MaxIterations { get; set; } = 100;
        [AlgorithmOption] public double Loudness { get; set; } = 0.75;
        [AlgorithmOption] public double LoudnessFade { get; set; } = 0.9;
        [AlgorithmOption] public double PulseRate { get; set; } = 0.8;
        [AlgorithmOption] public double PulseRateFade { get; set; } = 0.9;
        [AlgorithmOption] public double FrequencyMin { get; set; } = 0.5;
        [AlgorithmOption] public double FrequencyMax { get; set; } = 3.0;


        private List<double> bestPosition = [ 0, 0 ];
        private double bestValue = double.PositiveInfinity;

        private Random random = new();

        private class Bat
        {
            public List<double> Position { get; set; }
            public double[] Velocity { get; set; }
            public double Frequency { get; set; } = 0;
            public double Loudness { get; set; } = 0;
            public double PulseRate { get; set; } = 0;
            public double Fitness { get; set; }

            public Bat(in Random rnd, in Function f)
            {
                Position = [f.Bounds.Left + rnd.NextDouble() * f.Bounds.Width, f.Bounds.Top + rnd.NextDouble() * f.Bounds.Height];
                Velocity = [0, 0];
                Fitness = f.Evaluate(Position[0], Position[1]);
            }

            public static explicit operator PointF(Bat b) => new((float)b.Position[0], (float)b.Position[1]);
        }

        public void SolveFunctionOptimizationProblem(Function f)
        {
            // Creating population of bats
            Bat[] bats = new Bat[PopulationSize];
            for (int i = 0; i < PopulationSize; i++)
            {
                bats[i] = new Bat(random, f)
                {
                    Loudness = Loudness,
                    PulseRate = PulseRate,
                };
            }

            bestValue = bats.MinBy(b => b.Fitness)!.Fitness;

            for (int i = 0; i < MaxIterations && !Stop; i++)
            {
                OnUpdateGraphics(bats.Select(a => (PointF)a).ToList());

                // Update frequencies in a specified range
                foreach (var bat in bats)
                {
                    bat.Frequency = FrequencyMin + (FrequencyMax - FrequencyMin) * random.NextDouble();
                }

                // Update global positions
                ApplySearch(bats, f, (bat) => {
                    List<double> newPos = [bat.Position[0], bat.Position[1]];

                    for (int k = 0; k < 2; k++)
                    {
                        newPos[k] += (bestPosition[k] - bat.Position[k]) * bat.Frequency;
                        newPos[k] += bat.Velocity[k];
                    }
                    return newPos;
                });

                // Update local positions
                double avgLoudness = bats.Average(b => b.Loudness);
                ApplySearch(bats, f, (b) => [
                            bestPosition[0] + avgLoudness * (2 * random.NextDouble() - 1),
                            bestPosition[1] + avgLoudness * (2 * random.NextDouble() - 1) ]);

                Bat bestBat = bats.MinBy(x => x.Fitness)!;
                bestPosition = new(bestBat.Position);
                bestValue = bestBat.Fitness;

                foreach (var bat in bats)
                {
                    bat.Loudness *= LoudnessFade;
                    bat.PulseRate = PulseRate * (1 - Math.Exp(-PulseRateFade * i));
                }
            }

            OnUpdateGraphics(bats.Select(a => (PointF)a).ToList(), true);
        }

        private void ApplySearch(Bat[] bats, Function f, Func<Bat, List<double>> generatePosition)
        {
            foreach (Bat bat in bats)
            {
                if (random.NextDouble() > bat.PulseRate)
                {
                    List<double> newPos = generatePosition(bat);
                    f.Clamp(newPos);

                    double fitness = f.Evaluate(newPos[0], newPos[1]);
                    if (fitness <= bat.Fitness && random.NextDouble() < bat.Loudness)
                    {
                        bat.Position = newPos;
                        bat.Fitness = fitness;
                    }
                }
            }
        }

        public string GetFunctionOptimizationResultString()
        {
            return $"Found solution: f({bestPosition[0]:F3}, {bestPosition[1]:F3}) = {bestValue:F3}";
        }
    }
}
