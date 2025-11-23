using Vector2 = System.Numerics.Vector2;

namespace ArtificialIntelligenceIHW
{
    static class Vector2Extension
    {
        public static Vector2 Normalized(this Vector2 a)
        {
            float len = a.Length();
            return new Vector2(a.X / len, a.Y / len);
        }

        public static Vector2 Rotate2(this Vector2 a, double angle)
        {
            return new(
                a.X * MathF.Cos((float)angle) - a.Y * MathF.Sin((float)angle),
                a.X * MathF.Sin((float)angle) + a.Y * MathF.Cos((float)angle)
                );
        }
    }

    public class Graph
    {
        private readonly double[,] data;

        public const string GoesTo = " \u2192 ";

        public double this[int i, int j]
        {
            get { return data[i, j]; }
            set { data[i, j] = value; }
        }

        public const double NoPath = double.PositiveInfinity;
        public int VertexCount { get => data.GetLength(0); }

        public Graph(int vertexCount, GraphEdge[] edges)
        {
            data = new double[vertexCount, vertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                for (int j = 0; j < VertexCount; j++)
                {
                    data[i, j] = NoPath;
                }
            }

            foreach (GraphEdge edge in edges)
            {
                this[edge.FirstVertex, edge.SecondVertex] = edge.Weight;

                if (edge.Orientation == GraphEdge.EdgeType.Unoriented) this[edge.SecondVertex, edge.FirstVertex] = edge.Weight;
            }
        }

        public static Graph Empty(int vertexCount) => new(vertexCount, Array.Empty<GraphEdge>());

        public List<int> AdjecentVerticesToVertex(int vertex)
        {
            List<int> colors = new();

            for (int i = 0; i < VertexCount; i++)
            {
                if (this[vertex, i] != NoPath) colors.Add(i);
            }

            return colors;
        }

        public int VertexDegree(int vertexIndex) => AdjecentVerticesToVertex(vertexIndex).Count;
    }

    public class GraphEdge
    {
        public enum EdgeType
        {
            Unoriented = 0,
            Oriented = 1
        }

        public int FirstVertex, SecondVertex;
        public double Weight;
        public EdgeType Orientation;

        public GraphEdge(int first, int second, EdgeType orientation, double weight)
        {
            FirstVertex = first;
            SecondVertex = second;
            Orientation = orientation;
            Weight = weight;
        }
    }

    public class DisplayableVertex
    {
        public int X, Y;
        public string Name;

        public DisplayableVertex(int x, int y, string name = "")
        {
            X = x;
            Y = y;
            Name = name;
        }

        public Point Point() => new(X, Y);

        public static explicit operator Point(DisplayableVertex p) => p.Point();
    }

    public class DisplayableGraph : Graph
    {
        public DisplayableVertex[] Vertices;

        private const int vertexSize = 10;
        private const int margin = 10;
        private double curveCoeff;

        private readonly Pen vertexPen;
        private readonly Pen pathPen;
        private readonly Pen edgePen;
        private readonly Brush textBrush;
        private readonly Brush valueBrush;

        public DisplayableGraph(DisplayableVertex[] vertices, GraphEdge[] edges) : base(vertices.Length, edges)
        {
            Vertices = vertices;
            vertexPen = new Pen(Color.Black);
            edgePen = new Pen(Color.Black)
            {
                Width = 0.75f
            };
            pathPen = new Pen(Color.Red)
            {
                Width = 1.75f,
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
            };
            textBrush = new SolidBrush(Color.Blue);
            valueBrush = new SolidBrush(Color.Orange);
            curveCoeff = 30.0 / VertexCount;
        }

        public void Draw(in Graphics g)
        {
            g.Clear(Color.White);
            for (int i = 0; i < VertexCount; i++)
            {
                for (int j = 0; j < VertexCount; j++)
                {
                    if (this[i, j] != NoPath)
                    {
                        DrawArrow(g, i, j, this[j, i] != NoPath);
                    }
                }
            }

            foreach (DisplayableVertex vertex in Vertices)
            {
                int tempX = vertex.X - vertexSize / 2;
                int tempY = vertex.Y - vertexSize / 2;
                g.FillEllipse(new SolidBrush(Color.White), new Rectangle(tempX, tempY, vertexSize, vertexSize));
                g.DrawEllipse(vertexPen, new Rectangle(tempX, tempY, vertexSize, vertexSize));
                g.DrawString(vertex.Name, new Font("Arial", vertexSize / 2), textBrush, tempX + 1f, tempY + 1f);
            }
        }

        public void FixGraphics(in Graphics g, in Rectangle bounds)
        {
            float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;
            for (int i = 0; i < VertexCount; i++)
            {
                DisplayableVertex v = Vertices[i];
                if (minX > v.X) minX = v.X;
                if (minY > v.Y) minY = v.Y;
                if (maxX < v.X) maxX = v.X;
                if (maxY < v.Y) maxY = v.Y;
            }
            float sx = bounds.Width / (maxX - minX + margin * 2), sy = bounds.Height / (maxY - minY + margin * 2);
            g.ScaleTransform(sx, sy);
            g.TranslateTransform(-minX + margin, -minY + margin);
        }

        public void DrawPath(in Graphics g, List<int> path, bool loop = false, bool arc = false)
        {
            if (arc)
            {
                for (int i = 1; i < path.Count + (loop ? 1 : 0); i++)
                {
                    DisplayableVertex first = Vertices[path[i - 1]], second = Vertices[path[i] % path.Count];
                    
                    Vector2 direction = new Vector2(second.X - first.X, second.Y - first.Y).Normalized();
                    Point midpoint = new((first.X + second.X) / 2 - (int)(curveCoeff * direction[1]), (first.Y + second.Y) / 2 + (int)(curveCoeff * direction[0]));
                    g.DrawCurve(pathPen, new Point[] { new Point(first.X, first.Y), midpoint, new Point(second.X, second.Y) });
                }
            }
            else
            {
                g.DrawLines(pathPen, path.Select(i => (Point)Vertices[i]).ToArray());
            }
        }

        public void ColorVertexes(in Graphics g, List<int> colorCodes)
        {
            var uniqueColors = colorCodes.Distinct().ToList();
            List<double> hues = Utility.ContrastHues(uniqueColors.Count);
            Dictionary<int, SolidBrush> codeToBrush = new();
            for (int c = 0; c < uniqueColors.Count; ++c)
            {
                codeToBrush.Add(uniqueColors[c], new(Utility.HueToRgb(hues[c])));
            }

            for (int i = 0; i < colorCodes.Count; i++)
            {
                DisplayableVertex vertex = Vertices[i];
                int tempX = vertex.X - vertexSize / 2;
                int tempY = vertex.Y - vertexSize / 2;
                g.FillEllipse(codeToBrush[colorCodes[i]], new Rectangle(tempX, tempY, vertexSize, vertexSize));
            }
        }

        private void DrawArrow(in Graphics g, int firstIndex, int secondIndex, bool arc)
        {
            const double angle = Math.PI / 6;
            const double length = 6.6;

            DisplayableVertex first = Vertices[firstIndex], second = Vertices[secondIndex];

            Vector2 direction = new Vector2(second.X - first.X, second.Y - first.Y).Normalized();
            Point realFirst = new(first.X + (int)(direction[0] * vertexSize / 2), first.Y + (int)(direction[1] * vertexSize / 2));
            Point realSecond = new(second.X - (int)(direction[0] * vertexSize / 2), second.Y - (int)(direction[1] * vertexSize / 2));
            Point midpoint;
            if (arc && this[firstIndex, secondIndex] != this[secondIndex, firstIndex])
            {
                midpoint = new((realFirst.X + realSecond.X) / 2 - (int)(curveCoeff * direction[1]), (realFirst.Y + realSecond.Y) / 2 + (int)(curveCoeff * direction[0]));
            }
            else
            {
                midpoint = new((realFirst.X + realSecond.X) / 2, (realFirst.Y + realSecond.Y) / 2);
            }

            g.DrawCurve(edgePen, new Point[] { realFirst, midpoint, realSecond });

            if (this[firstIndex, secondIndex] != this[secondIndex, firstIndex] && this[secondIndex, firstIndex] == NoPath)
            {
                Vector2 arrowLDirection = direction.Rotate2(angle), arrowRDirection = direction.Rotate2(-angle);

                Point arrowL = new(midpoint.X - (int)(arrowLDirection[0] * length), midpoint.Y - (int)(arrowLDirection[1] * length));
                Point arrowR = new(midpoint.X - (int)(arrowRDirection[0] * length), midpoint.Y - (int)(arrowRDirection[1] * length));
                g.DrawLine(edgePen, midpoint, arrowL);
                g.DrawLine(edgePen, midpoint, arrowR);
            }

            g.DrawString(this[firstIndex, secondIndex].ToString(), new Font("Arial", vertexSize / 3f), valueBrush, midpoint);
        }
    }

    public static class GraphParser
    {
        public const string graphsDirectory = @"..\..\..\Resources\gs\";

        private static DisplayableGraph Parse(StreamReader sr)
        {
            List<DisplayableVertex> vertices = new();
            List<GraphEdge> edges = new();

            GraphEdge.EdgeType? defaultEdge = null;

            int lineCount = 1;
            while (!sr.EndOfStream)
            {
                try
                {
                    string line = sr.ReadLine()!;

                    int commentIndex = line.IndexOf('#');
                    if (commentIndex != -1) line = line.Substring(0, commentIndex);

                    if (line == "") continue;

                    string[] args = line.Trim().Split(',');

                    if (lineCount == 1)
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            vertices.Add(new DisplayableVertex(0, 0, args[i]));
                        }
                    }
                    else if (lineCount <= vertices.Count + 1)
                    {
                        vertices[lineCount - 2].X = Convert.ToInt32(args[0]);
                        vertices[lineCount - 2].Y = Convert.ToInt32(args[1]);
                    }
                    else if (lineCount <= vertices.Count * 2 + 2)
                    {
                        if (args.Length < vertices.Count)
                        {
                            if (lineCount == vertices.Count + 2)
                            {
                                defaultEdge = (GraphEdge.EdgeType)Enum.Parse(typeof(GraphEdge.EdgeType), args[0]);
                                continue;
                            }
                            else throw new FormatException($"Graph file has incorrect values in line {lineCount}.");
                        }

                        int j = lineCount - vertices.Count - 2;
                        for (int i = 0; i < vertices.Count; i++)
                        {
                            double weight;
                            try
                            {
                                weight = Convert.ToDouble(args[i]);
                            }
                            catch
                            {
                                weight = Graph.NoPath;
                            }

                            edges.Add(new GraphEdge(j, i, defaultEdge ?? GraphEdge.EdgeType.Oriented, weight));
                        }
                    }
                    else continue;
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Graph syntax parsing failed (line {lineCount}).\n{ex.Message}");
                }

                lineCount++;
            }

            return new(vertices.ToArray(), edges.ToArray());
        }

        public static DisplayableGraph LoadFromFile(string path)
        {
            if (!File.Exists(path)) path = graphsDirectory + path;

            StreamReader sr = File.OpenText(path);

            return Parse(sr);
        }

        public static DisplayableGraph LoadFromString(string text)
        {
            MemoryStream stream = new();
            StreamWriter sw = new(stream);
            sw.Write(text);
            sw.Flush();
            stream.Position = 0;

            StreamReader sr = new(stream);

            return Parse(sr);
        }

        public static Image DrawGraph(DisplayableGraph g, uint size = 250)
        {
            Image gImage = new Bitmap((int)size, (int)size);
            Graphics gi = Graphics.FromImage(gImage);
            g.FixGraphics(gi, new Rectangle(0, 0, (int)size, (int)size));
            g.Draw(gi);
            return gImage;
        }
    }
}
