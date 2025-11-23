using ArtificialIntelligenceIHW.Algorithm;

namespace ArtificialIntelligenceIHW.Problem
{
    public interface IProblemWrapper
    {
        void Solve();

        string GetResults();
    }

    public class FunctionOptimizationWrapper : IProblemWrapper
    {
        IFunctionOptimization fo;
        Function f;

        public FunctionOptimizationWrapper(IFunctionOptimization fo, Function f)
        {
            this.fo = fo;
            this.f = f;
        }

        public void Solve() => fo.SolveFunctionOptimizationProblem(f);

        public string GetResults() => fo.GetFunctionOptimizationResultString();
    }

    public class GraphColoringWrapper : IProblemWrapper
    {
        IGraphColoring gc;
        Graph g;

        public GraphColoringWrapper(IGraphColoring gc, Graph g)
        {
            this.gc = gc;
            this.g = g;
        }

        public void Solve() => gc.SolveGraphColoringProblem(g);

        public string GetResults() => gc.GetGraphColoringResultString();
    }

    public class LongestPathWrapper : IProblemWrapper
    {
        ILongestPath lp;
        Graph g;

        public LongestPathWrapper(ILongestPath lp, Graph g)
        {
            this.lp = lp;
            this.g = g;
        }

        public void Solve() => lp.SolveLongestPathProblem(g);

        public string GetResults() => lp.GetLongestPathResultString();
    }

    public class ProblemWrapperFactory
    {
        AbstractAlgorithm alg;
        (Function f, Action<object, bool>? upd) function;
        (Graph g, Action<object, bool>? upd) gLP;
        (Graph g, Action<object, bool>? upd) gColor;

        public ProblemWrapperFactory(
            AbstractAlgorithm alg,
            (Function func, Action<object, bool>? funcUpd) funcPair,
            (Graph g, Action<object, bool>? funcUpd) gLPPair,
            (Graph g, Action<object, bool>? funcUpd) gColorPair
            )
        {
            this.alg = alg;
            function = funcPair;
            gLP = gLPPair;
            gColor = gColorPair;
        }

        public IProblemWrapper Create(string name)
        {
            IProblemWrapper pw;
            alg.ResetGraphicsEvents();
            switch (name)
            {
                case "Function Optimization":
                    pw = new FunctionOptimizationWrapper(alg as IFunctionOptimization, function.f);
                    alg.SetGraphicsEvent(function.upd);
                    break;
                case "Graph Coloring":
                    pw = new GraphColoringWrapper(alg as IGraphColoring, gColor.g);
                    alg.SetGraphicsEvent(gColor.upd);
                    break;
                case "Longest Path":
                    pw = new LongestPathWrapper(alg as ILongestPath, gLP.g);
                    alg.SetGraphicsEvent(gLP.upd);
                    break;
                default:
                    throw new NotImplementedException("No such problem, please add it wrapper!");
            }
            return pw;
        }
    }
}
