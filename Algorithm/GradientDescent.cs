using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW.Algorithm
{
    public class GradientDescent
        : AbstractAlgorithm
        , IFunctionOptimization
    {
        [AlgorithmOption] public double InitialX { get; set; } = 0.1;
        [AlgorithmOption] public double InitialY { get; set; } = 3;
        [AlgorithmOption] public double StepSize { get; set; } = 0.0005;
        [AlgorithmOption] public uint MaxIterations { get; set; } = 30;
        [AlgorithmOption] public double DescentVelocity { get; set; } = 0.001;
        [AlgorithmOption] public bool SaveTrajectory { get; set; } = false;

        double x, y;
        double fr = double.PositiveInfinity;

        public string GetFunctionOptimizationResultString()
        {
            return $"colors: f({x:F3}, {y:F3}) = {fr:F3}";
        }

        public void SolveFunctionOptimizationProblem(Function f)
        {
            x = InitialX; y = InitialY;

            List<PointF> points = [ new((float)x, (float)y) ];

            (double dx, double dy) gradient()
            {
                double fin = f.Evaluate(x, y);
                return ((f.Evaluate(x + StepSize, y) - fin) / StepSize, (f.Evaluate(x, y + StepSize) - fin) / StepSize);
            }

            for (int i = 0; i < MaxIterations && !Stop; ++i)
            {
                if (!SaveTrajectory)
                {
                    points.Clear();
                }
                points.Add(new((float)x, (float)y));
                OnUpdateGraphics(points);

                (double dx, double dy) = gradient();
                x -= DescentVelocity * dx;
                y -= DescentVelocity * dy;
                fr = f.Evaluate(x, y);
            }

            OnUpdateGraphics(points, true);
        }
    }
}
