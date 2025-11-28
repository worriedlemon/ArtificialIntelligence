using ArtificialIntelligenceIHW.Algorithm;
using ArtificialIntelligenceIHW.Problem;
using static ArtificialIntelligenceIHW.Resources.GraphStrings;

namespace ArtificialIntelligenceIHW
{
    public partial class MainForm : Form
    {
        private readonly List<AbstractAlgorithm> allAlgorithms;
        private readonly Dictionary<string, (Type type, Action<object?, bool> updateGraphics)> problems;

        // Test Function
        private readonly Function function = new(
            (double x, double y) => 100 * (x * x - y) * (x * x - y) + (1 - x) * (1 - x),
            //(double x1, double x2, double x3, double x4, double x5, double x6) => x1*x1 + x2*x2 + x3*x3 + x4*x4 + x5*x5 + x6*x6,
            (-6.0, 6.0)
            );

        // Test graph
        private readonly DisplayableGraph acyclicGraph = GraphParser.LoadFromFile("../../../Resources/acyclicGraph20.txt");

        // Test graph 2
        private readonly DisplayableGraph graph = GraphParser.LoadFromFile("../../../Resources/unorientedGraph10.txt");

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
                    new MemeticAlgorithm(),
                    new BacteriaForaging(),
                ];

            // Adding problems is also easy, but there are three steps to add a working one:
            // 1) Create function to update graphics
            // 2) Create new interface and wrapper in ProblemWrapper.cs
            // 3) Create algorithm, that uses this interface or extend existing one, and add a pair here
            List<(Type type, Action<object?, bool> updateGraphics)> allProblems = [
                (typeof(IFunctionOptimization), FunctionProblemUpdateGraphics),
                (typeof(IGraphColoring), GraphColoringProblemUpdateGraphics),
                (typeof(ILongestPath), LongestPathProblemUpdateGraphics),
            ];

            // Some magic to get it work
            problems = allProblems.Select((val) => KeyValuePair.Create(Utility.AddSpaces(val.type.Name[1..]), val)).ToDictionary();

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
            currentTask.updateGraphics(null, false);

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
            runStopButton.Enabled = algorithmComboBox.Enabled;
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
                if (t.IsEnum) // for enums it is good to use ComboBox
                {
                    ComboBox cb = new();
                    cb.Items.AddRange(t.GetEnumNames());
                    cb.DataBindings.Add(nameof(cb.Text), currentAlgorithm, name);
                    cb.SelectedIndex = 0;
                    control = cb;
                }
                else if (t.Equals(typeof(bool))) // for bool we create a CheckBox
                {
                    CheckBox cb = new()
                    {
                        Text = "Enabled",
                        CheckAlign = ContentAlignment.TopLeft,
                        TextAlign = ContentAlignment.TopCenter,
                    };
                    cb.DataBindings.Add(nameof(cb.Checked), currentAlgorithm, name);
                    control = cb;
                }
                else // otherwise - TextBox
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
            void stopRun(object? sender, EventArgs? e)
            {
                currentAlgorithm.Stop = true;
            }

            currentAlgorithm.Stop = false;
            runStopButton.Click += stopRun;
            runStopButton.Click -= OnRun;
            runStopButton.Text = "Stop";

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

            runStopButton.Click -= stopRun;
            runStopButton.Click += OnRun;
            runStopButton.Text = "Run";
        }

        void FunctionProblemUpdateGraphics(object? data, bool forced)
        {
            if (data is null)
            {
                mainPanel.Image = functionPlot;
                return;
            }
            if (drawDelay.Value == 0 && !forced) return;

            graphics = mainPanel.CreateGraphics();
            graphics.DrawImage(functionPlot, 0, 0, mainPanel.Width, mainPanel.Height);

            Image pointsImage = new Bitmap(mainPanel.Width, mainPanel.Height);
            Graphics g = Graphics.FromImage(pointsImage);

            var realData = (List<PointF>)data;

            foreach (PointF p in realData)
            {
                Plotter.DrawPoint(g, p, new(
                    (float)function.Bounds.min,
                    (float)function.Bounds.min,
                    (float)function.Bounds.max - (float)function.Bounds.min,
                    (float)function.Bounds.max - (float)function.Bounds.min
                    ), pointsImage.Size, 8);
            }
            graphics.DrawImage(pointsImage, 0, 0, mainPanel.Width, mainPanel.Height);

            Thread.Sleep((int)drawDelay.Value);
        }

        void LongestPathProblemUpdateGraphics(object? data, bool forced)
        {
            if (data is null)
            {
                mainPanel.Image = acyclicGraphImage;
                return;
            }
            if (drawDelay.Value == 0 && !forced) return;

            DrawGraph(acyclicGraph);
            acyclicGraph.DrawPath(graphics, (List<int>)data);

            Thread.Sleep((int)drawDelay.Value);
        }

        void GraphColoringProblemUpdateGraphics(object? data, bool forced)
        {
            if (data is null)
            {
                mainPanel.Image = graphImage;
                return;
            }
            if (drawDelay.Value == 0 && !forced) return;

            DrawGraph(graph);
            graph.ColorVertexes(graphics, (List<int>)data);

            Thread.Sleep((int)drawDelay.Value);
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
