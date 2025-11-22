using ArtificialIntelligenceIHW.Algorithm;
using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW
{
    public partial class MainForm : Form
    {
        private readonly List<AbstractAlgorithm> allAlgorithms;
        private readonly Dictionary<string, (Type type, Action<object?> updateGraphics)> problems;

        // Test Function
        private readonly Function function = new(
            (double x, double y) => 100 * (x * x - y) * (x * x - y) + (1 - x) * (1 - x),
            new(-6f, -6f, 12f, 12f)
            );

        // Test graph
        private readonly DisplayableGraph acyclicGraph = GraphParser.LoadFromString("""
            A,B,C,D,E,F,G

            0,80
            50,150
            120,150
            170,80
            120,10
            50,10
            85,80

            -,1,-,-,-,7,3
            -,-,3,-,-,-,-
            -,-,-,2,-,-,-
            -,-,-,-,-,-,-
            -,-,-,4,-,-,-
            -,-,-,-,1,-,4
            -,1,-,-,2,-,-
            """);

        // Test graph 2
        private readonly DisplayableGraph graph = GraphParser.LoadFromString("""
            A,B,C,D,E,F

            0,80
            50,150
            120,150
            170,80
            120,10
            50,10

            Unoriented
            -,1,3,7,7,7
            1,-,1,1,2,2
            3,1,-,2,3,3
            7,1,2,-,1,4
            7,2,3,1,-,1
            7,2,3,4,1,-
            """);

        private AbstractAlgorithm currentAlgorithm => (AbstractAlgorithm)algorithmComboBox.SelectedItem!;

        private Graphics graphics;

        private readonly Image functionPlot;
        private readonly Image graphImage;
        private readonly Image acyclicGraphImage;

        public MainForm()
        {
            InitializeComponent();

            // Adding algorithms is easy
            allAlgorithms = [
                    new GeneticAlgorithm(),
                    new GradientDescent(),
                    new BatInspiredAlgorithm(),
                    new DepthFirstSearch(),
                    new GreedyColoring(),
                ];

            // Adding problems is also easy, but there are three steps to add a working one:
            // 1) Create function to update graphics (you can use empty lambda)
            // 2) Create new interface and wrapper in ProblemWrapper.cs
            // 3) Create algorithm, that uses this interface or extend existing one, and add it
            List<(Type type, Action<object?> updateGraphics)> allProblems = [
                (typeof(IFunctionOptimization), FunctionProblemUpdateGraphics),
                (typeof(IGraphColoring), GraphColoringProblemUpdateGraphics),
                (typeof(ILongestPath), LongestPathProblemUpdateGraphics),
            ];

            // Some magic
            problems = allProblems
                .Select((val) => KeyValuePair.Create(Utility.AddSpaces(val.type.Name[1..]), val))
                .ToDictionary();

            graphics = mainPanel.CreateGraphics();

            // Images
            functionPlot = Plotter.DrawFunction(function, 1000);
            graphImage = GraphParser.DrawGraph(graph, 500);
            acyclicGraphImage = GraphParser.DrawGraph(acyclicGraph, 500);

            taskComboBox.Items.AddRange(problems.Keys.ToArray());
            taskComboBox.SelectedIndex = 0;
        }

        void OnTaskChange(object? sender, EventArgs? e)
        {
            var currentTask = problems[(taskComboBox.SelectedItem as string)!];

            algorithmComboBox.Items.Clear();
            algorithmComboBox.Items.AddRange(allAlgorithms.Where(x =>
                x.GetType().GetInterfaces().Contains(currentTask.type)).ToArray());
            currentTask.updateGraphics(null);

            if (algorithmComboBox.Items.Count == 0)
            {
                algorithmComboBox.Text = "";
                algorithmComboBox.Enabled = false;
                MessageBox.Show("No suitable algorithms!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                algorithmComboBox.SelectedIndex = 0;
                algorithmComboBox.Enabled = true;
            }
            runButton.Enabled = algorithmComboBox.Enabled;
        }

        void OnAlgorithmChange(object? sender, EventArgs? e)
        {
            optionsPanel.Controls.Clear();

            List<(string name, Type type)> opt = currentAlgorithm.GetOptions();
            if (opt.Count == 0)
            {
                optionsPanel.Text = $"No options for {currentAlgorithm.Name}";
                optionsPanel.ForeColor = Color.Red;
                return;
            }

            optionsPanel.Text = $"{currentAlgorithm.Name} options:";
            optionsPanel.ForeColor = Color.Black;

            Form form = new()
            {
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None
            };

            TableLayoutPanel p = new()
            {
                Dock = DockStyle.Fill,
                RowCount = opt.Count
            };
            form.Controls.Add(p);

            int row = 0;
            foreach ((string name, Type t) in opt)
            {
                Label label = new()
                {
                    AutoSize = true,
                    Text = $"{Utility.AddSpaces(name)}:"
                };

                Control control;
                if (t.IsEnum)
                {
                    ComboBox cb = new();
                    cb.Items.AddRange(t.GetEnumNames().Select(Utility.AddSpaces).ToArray());
                    cb.DataBindings.Add(nameof(cb.Text), currentAlgorithm, name);
                    cb.SelectedIndex = 0;
                    control = cb;
                }
                else if (t.Equals(typeof(bool)))
                {
                    CheckBox cb = new()
                    {
                        Text = "Checked",
                        CheckAlign = ContentAlignment.TopLeft,
                        TextAlign = ContentAlignment.TopLeft,
                    };
                    cb.DataBindings.Add(nameof(cb.Checked), currentAlgorithm, name);
                    control = cb;
                }
                else
                {
                    control = new TextBox();
                    control.DataBindings.Add(nameof(control.Text), currentAlgorithm, name);
                }
                control.Dock = DockStyle.Fill;

                p.RowStyles.Add(new(SizeType.Absolute, control.Size.Height));

                p.Controls.Add(label);
                p.Controls.Add(control);

                p.SetColumn(label, 0);
                p.SetRow(label, row);

                p.SetColumn(control, 1);
                p.SetRow(control, row);

                ++row;
            }
            form.Show();
            optionsPanel.Controls.Add(form);
        }

        async void OnRun(object? sender, EventArgs? e)
        {
            runButton.Enabled = false;
            try
            {
                IProblemWrapper pw = new ProblemWrapperFactory(
                    currentAlgorithm,
                    (function, FunctionProblemUpdateGraphics),
                    (acyclicGraph, LongestPathProblemUpdateGraphics),
                    (graph, GraphColoringProblemUpdateGraphics)
                    ).Create((taskComboBox.SelectedItem as string)!);
                await Task.Run(pw.Solve);
                MessageBox.Show(pw.GetResults(), "colors");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred during Run: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            runButton.Enabled = true;
        }

        void FunctionProblemUpdateGraphics(object? data)
        {
            graphics = mainPanel.CreateGraphics();
            graphics.DrawImage(functionPlot, 0, 0, mainPanel.Width, mainPanel.Height);

            if (data is not null)
            {
                Image pointsImage = new Bitmap(mainPanel.Width, mainPanel.Height);
                Graphics g = Graphics.FromImage(pointsImage);

                var realData = (List<PointF>)data;

                foreach (PointF p in realData)
                {
                    Plotter.DrawPoint(g, p, function.Bounds, pointsImage.Size, 8);
                }
                graphics.DrawImage(pointsImage, 0, 0, mainPanel.Width, mainPanel.Height);

                Thread.Sleep((int)drawDelay.Value);
            }
            else
            {
                mainPanel.Image = functionPlot;
            }
        }

        void LongestPathProblemUpdateGraphics(object? data)
        {
            DrawGraph(acyclicGraph);

            if (data is not null)
            {
                acyclicGraph.DrawPath(graphics, (List<int>)data);

                Thread.Sleep((int)drawDelay.Value);
            }
            else
            {
                mainPanel.Image = acyclicGraphImage;
            }
        }

        void GraphColoringProblemUpdateGraphics(object? data)
        {
            DrawGraph(graph);

            if (data is not null)
            {
                graph.ColorVertexes(graphics, (List<int>)data);

                Thread.Sleep((int)drawDelay.Value);
            }
            else
            {
                mainPanel.Image = graphImage;
            }
        }

        void DrawGraph(in DisplayableGraph g)
        {
            graphics = mainPanel.CreateGraphics();
            graphics.Clear(mainPanel.BackColor);
            g.FixGraphics(graphics, mainPanel.Bounds);
            g.Draw(graphics);
        }
    }
}
