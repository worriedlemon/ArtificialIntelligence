namespace ArtificialIntelligenceIHW
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Parameters for plotter
            Plotter.FirstZoneEndValue = 2.75;
            Plotter.ColorPallete = [
                Color.LimeGreen,
                Color.LightGreen,
                Color.Yellow,
                Color.Red,
                Color.Pink,
                Color.White,
            ];

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occured during start:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
