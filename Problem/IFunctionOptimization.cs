namespace ArtificialIntelligenceIHW.Problem
{
    public class Function
    {
        Func<double, double, double> callable;
        public RectangleF Bounds { get; }

        public Function(Func<double, double, double> callable, RectangleF bounds)
        {
            this.callable = callable;
            Bounds = bounds;
        }

        public double Evaluate(double x, double y) => callable.Invoke(x, y);

        public List<double> GetPoint(in Random rand)
        {
            double x = rand.NextDouble() * Bounds.Width + Bounds.Left;
            double y = rand.NextDouble() * Bounds.Height + Bounds.Top;
            return [x, y];
        }

        public void Clamp(List<double> value)
        {
            value[0] = Math.Clamp(value[0], Bounds.Left, Bounds.Left + Bounds.Width);
            value[1] = Math.Clamp(value[1], Bounds.Top, Bounds.Top + Bounds.Height);
        }
    }

    public interface IFunctionOptimization
    {
        public void SolveFunctionOptimizationProblem(Function f);
        public string GetFunctionOptimizationResultString();
    }
}
