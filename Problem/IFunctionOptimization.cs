namespace ArtificialIntelligenceIHW.Problem
{
    public class Function
    {
        Delegate callable;
        public (double min, double max) Bounds { get; }
        int dim;

        public Function(Delegate callable, (double min, double max) bounds)
        {
            if (callable.Method.ReturnType != typeof(double))
            {
                throw new InvalidCastException("Callable does not return fixed point value, expected double");
            }
            this.callable = callable;
            Bounds = bounds;
            dim = callable.Method.GetParameters().Length;
        }

        public double Evaluate(params double[] xs)
        {
            object[] values = new object[dim];
            int actDim = Math.Min(xs.Length, dim);
            for (int i = 0; i < actDim; i++)
            {
                values[i] = xs[i];
            }
            return (double)callable?.Method.Invoke(callable.Target, values)!;
        }

        public List<double> GetPoint(in Random rand)
        {
            List<double> values = new();
            for (int i = 0; i < dim; ++i)
            {
                values.Add(rand.NextDouble() * (Bounds.max - Bounds.min) + Bounds.min);
            }
            return values;
        }

        public void Clamp(List<double> value)
        {
            int actDim = Math.Min(value.Count, dim);
            for (int i = 0; i < actDim; ++i)
            {
                value[i] = Math.Clamp(value[i], Bounds.min, Bounds.max);
            }
        }
    }

    public interface IFunctionOptimization
    {
        public void SolveFunctionOptimizationProblem(Function f);
        public string GetFunctionOptimizationResultString();
    }
}
